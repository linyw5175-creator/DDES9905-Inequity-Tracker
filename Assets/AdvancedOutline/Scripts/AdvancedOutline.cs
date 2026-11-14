using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ITISKIRUHERE
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class AdvancedOutline : MonoBehaviour
    {
        public const string Version = "1.1.1";

        public enum Mode
        {
            OutlineAll,
            OutlineVisible,
            OutlineHidden,
            OutlineAndSilhouette,
            SilhouetteOnly
        }

        [Serializable]
        class ListVector3
        {
            public List<Vector3> data;
        }

        [Header("References")]
        [SerializeField] Shader _outlineMaskShader;
        [SerializeField] Shader _outlineFillShader;

        [Header("General Settings")]
        [SerializeField] Mode _outlineMode = Mode.OutlineVisible;
        [SerializeField] Color _outlineColor = Color.cyan;
        [SerializeField] [Range( 0f, 10f )] float _outlineWidth = 4f;

        [Header("Aesthetic Effects")]
        [SerializeField] bool _pulseWidth;
        [SerializeField] [Range( 0.1f, 10f )] float _pulseSpeed = 2f;
        [SerializeField] [Range( 0f, 10f )] float _pulseMinWidth = 1.5f;
        [SerializeField] [Range( 0f, 10f )] float _pulseMaxWidth = 5f;

        [Space]
        [SerializeField] bool _rainbowColor;
        [SerializeField] [Range( 0.1f, 5f )] float _rainbowSpeed = 0.5f;

        [Space]
        [SerializeField] bool _dashedPattern;
        [SerializeField] [Range( 0.001f, 0.1f )] float _dashScale = 0.03f;
        [SerializeField] [Range( -50f, 50f )] float _dashSpeed = 15f;

        [Header("Performance Settings")]
        [SerializeField] bool _precomputeOutline;

        [SerializeField] [HideInInspector] List<Mesh> _bakeKeys = new List<Mesh>();
        [SerializeField] [HideInInspector] List<ListVector3> _bakeValues = new List<ListVector3>();

        Renderer[] _renderers;
        MeshFilter[] _meshFilters;
        SkinnedMeshRenderer[] _skinnedMeshRenderers;
        Material _outlineMaskMaterial;
        Material _outlineFillMaterial;
        MaterialPropertyBlock _propertyBlock;

        bool _needsUpdate;

        int _outlineColorId;
        int _outlineWidthId;
        int _zTestId;
        int _dashScaleId;
        int _dashSpeedId;

        List<Material> _tempMaterials = new List<Material>();
        Dictionary<Mesh, List<Vector3>> _runtimeBakeCache = new Dictionary<Mesh, List<Vector3>>();
        internal Dictionary<MeshFilter, Mesh> _clonedMeshes = new Dictionary<MeshFilter, Mesh>();
        internal Dictionary<MeshFilter, Mesh> _originalMeshes = new Dictionary<MeshFilter, Mesh>();
        internal Dictionary<SkinnedMeshRenderer, Mesh> _clonedSkinnedMeshes = new Dictionary<SkinnedMeshRenderer, Mesh>();
        internal Dictionary<SkinnedMeshRenderer, Mesh> _originalSkinnedMeshes = new Dictionary<SkinnedMeshRenderer, Mesh>();

        public Mode OutlineMode
        {
            get => _outlineMode;
            set
            {
                _outlineMode = value;
                _needsUpdate = true;
            }
        }

        public Color OutlineColor
        {
            get => _outlineColor;
            set
            {
                _outlineColor = value;
                _needsUpdate = true;
            }
        }

        public float OutlineWidth
        {
            get => _outlineWidth;
            set
            {
                _outlineWidth = value;
                _needsUpdate = true;
            }
        }

        public bool PulseWidth { get => _pulseWidth; set => _pulseWidth = value; }
        public bool RainbowColor { get => _rainbowColor; set => _rainbowColor = value; }
        public bool DashedPattern { get => _dashedPattern; set => _dashedPattern = value; }

        public int RendererCount => _renderers != null ? _renderers.Length : 0;
        public int MeshCount => ( _meshFilters != null ? _meshFilters.Length : 0 ) + ( _skinnedMeshRenderers != null ? _skinnedMeshRenderers.Length : 0 );

        public Shader OutlineMaskShader { get => _outlineMaskShader; set { _outlineMaskShader = value; _needsUpdate = true; } }
        public Shader OutlineFillShader { get => _outlineFillShader; set { _outlineFillShader = value; _needsUpdate = true; } }

        void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
            CachePropertyIDs();
            InitializeBakeCache();
            InitializeMaterials();
            RefreshRenderers();
        }

        void OnEnable()
        {
            if ( _propertyBlock == null )
            {
                _propertyBlock = new MaterialPropertyBlock();
            }
            CachePropertyIDs();
            InitializeBakeCache();
            InitializeMaterials();
            RefreshRenderers();
        }

        void OnDisable()
        {
            CleanAllOutlineMaterialsFromRenderers();
            RestoreOriginalMeshes();
        }

        void OnDestroy()
        {
            CleanAllOutlineMaterialsFromRenderers();
            RestoreOriginalMeshes();

            if ( _outlineMaskMaterial )
            {
                DestroyOutlineObject( _outlineMaskMaterial );
                _outlineMaskMaterial = null;
            }
            if ( _outlineFillMaterial )
            {
                DestroyOutlineObject( _outlineFillMaterial );
                _outlineFillMaterial = null;
            }
            DestroyClonedMeshes();
        }

        void OnTransformChildrenChanged()
        {
            RefreshRenderers();
        }

        void Update()
        {
            if ( _needsUpdate )
            {
                _needsUpdate = false;
                UpdateStaticMaterialProperties();
            }

            HandleDynamicEffects();
        }

        void OnValidate()
        {
            _needsUpdate = true;
            InitializeMaterials();

            if ( _pulseMinWidth > _pulseMaxWidth )
            {
                _pulseMinWidth = _pulseMaxWidth;
            }
            if ( _pulseMaxWidth < _pulseMinWidth )
            {
                _pulseMaxWidth = _pulseMinWidth;
            }
            if ( _dashScale < 0f )
            {
                _dashScale = 0f;
            }
            #if UNITY_EDITOR
            if ( !_precomputeOutline && _bakeKeys.Count != 0 || _bakeKeys.Count != _bakeValues.Count )
            {
                _bakeKeys.Clear();
                _bakeValues.Clear();
            }
            #endif
        }

        public void RefreshRenderers()
        {
            CacheComponents();
            CleanAllOutlineMaterialsFromRenderers();
            LoadSmoothNormals();
            ApplyMaterialsToRenderers();
            _needsUpdate = true;
        }

        #if UNITY_EDITOR
        public void EditorPrepareForSave()
        {
            CleanAllOutlineMaterialsFromRenderers();
            RestoreOriginalMeshes();
        }

        public void EditorRestoreAfterSave()
        {
            if ( this != null && enabled && gameObject.activeInHierarchy )
            {
                RefreshRenderers();
            }
        }
        #endif

        void CacheComponents()
        {
            _renderers = GetComponentsInChildren<Renderer>( true );
            _meshFilters = GetComponentsInChildren<MeshFilter>( true );
            _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>( true );
        }

        void CachePropertyIDs()
        {
            _outlineColorId = Shader.PropertyToID( "_OutlineColor" );
            _outlineWidthId = Shader.PropertyToID( "_OutlineWidth" );
            _zTestId = Shader.PropertyToID( "_ZTest" );
            _dashScaleId = Shader.PropertyToID( "_DashScale" );
            _dashSpeedId = Shader.PropertyToID( "_DashSpeed" );
        }

        void InitializeBakeCache()
        {
            _runtimeBakeCache.Clear();
            if ( _bakeKeys != null && _bakeValues != null )
            {
                int count = Mathf.Min( _bakeKeys.Count, _bakeValues.Count );
                for ( int i = 0; i < count; i++ )
                {
                    if ( _bakeKeys[i] != null && _bakeValues[i] != null )
                    {
                        _runtimeBakeCache[_bakeKeys[i]] = _bakeValues[i].data;
                    }
                }
            }
        }

        void InitializeMaterials()
        {
            if ( _outlineMaskMaterial && _outlineFillMaterial ) return;

            bool isURP = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null;

            if ( !_outlineMaskShader )
            {
                _outlineMaskShader = Shader.Find( isURP ? "Custom/Advanced Outline Mask URP" : "Custom/Advanced Outline Mask" );
            }
            if ( !_outlineFillShader )
            {
                _outlineFillShader = Shader.Find( isURP ? "Custom/Advanced Outline Fill URP" : "Custom/Advanced Outline Fill" );
            }

            if ( !_outlineMaskMaterial && _outlineMaskShader )
            {
                _outlineMaskMaterial = new Material( _outlineMaskShader );
                _outlineMaskMaterial.name = "AdvancedOutlineMask (Instance)";
                _outlineMaskMaterial.hideFlags = HideFlags.DontSave;
            }

            if ( !_outlineFillMaterial && _outlineFillShader )
            {
                _outlineFillMaterial = new Material( _outlineFillShader );
                _outlineFillMaterial.name = "AdvancedOutlineFill (Instance)";
                _outlineFillMaterial.hideFlags = HideFlags.DontSave;
            }
        }

        void ApplyMaterialsToRenderers()
        {
            if ( _renderers == null ) return;

            foreach ( Renderer renderer in _renderers )
            {
                if ( !renderer ) continue;

                _tempMaterials.Clear();
                renderer.GetSharedMaterials( _tempMaterials );

                _tempMaterials.RemoveAll( mat => mat != null &&
                    ( mat.shader == _outlineMaskShader || mat.shader == _outlineFillShader ) );

                if ( _outlineMaskMaterial )
                {
                    _tempMaterials.Add( _outlineMaskMaterial );
                }
                if ( _outlineFillMaterial )
                {
                    _tempMaterials.Add( _outlineFillMaterial );
                }

                renderer.sharedMaterials = _tempMaterials.ToArray();
            }
            _tempMaterials.Clear();
        }

        void CleanAllOutlineMaterialsFromRenderers()
        {
            if ( _renderers == null ) return;

            foreach ( Renderer renderer in _renderers )
            {
                if ( !renderer ) continue;

                _tempMaterials.Clear();
                renderer.GetSharedMaterials( _tempMaterials );

                _tempMaterials.RemoveAll( mat => mat != null &&
                    ( mat.shader == _outlineMaskShader || mat.shader == _outlineFillShader ) );

                renderer.sharedMaterials = _tempMaterials.ToArray();
            }
            _tempMaterials.Clear();
        }

        Mesh ResolveOriginalMesh( MeshFilter meshFilter )
        {
            if ( !meshFilter ) return null;

            if ( _originalMeshes.TryGetValue( meshFilter, out Mesh cached ) && cached != null && !cached.name.Contains( "(Outline Clone)" ) )
            {
                return cached;
            }

            Mesh current = meshFilter.sharedMesh;
            if ( !current ) return null;

            if ( !current.name.Contains( "(Outline Clone)" ) )
            {
                _originalMeshes[meshFilter] = current;
                return current;
            }

            #if UNITY_EDITOR
            MeshFilter prefabFilter = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource( meshFilter );
            if ( prefabFilter != null && prefabFilter.sharedMesh != null && !prefabFilter.sharedMesh.name.Contains( "(Outline Clone)" ) )
            {
                _originalMeshes[meshFilter] = prefabFilter.sharedMesh;
                return prefabFilter.sharedMesh;
            }

            #if UNITY_2023_1_OR_NEWER
            var allOutlines = UnityEngine.Object.FindObjectsByType<AdvancedOutline>( FindObjectsInactive.Include, FindObjectsSortMode.None );
            #else
            var allOutlines = UnityEngine.Object.FindObjectsOfType<AdvancedOutline>( true );
            #endif
            foreach ( var other in allOutlines )
            {
                if ( other == null ) continue;
                foreach ( var pair in other._clonedMeshes )
                {
                    if ( pair.Value == current && other._originalMeshes.TryGetValue( pair.Key, out Mesh orig ) && orig != null && !orig.name.Contains( "(Outline Clone)" ) )
                    {
                        _originalMeshes[meshFilter] = orig;
                        return orig;
                    }
                }
            }
            #endif

            _originalMeshes[meshFilter] = current;
            return current;
        }

        Mesh ResolveOriginalSkinnedMesh( SkinnedMeshRenderer smr )
        {
            if ( !smr ) return null;

            if ( _originalSkinnedMeshes.TryGetValue( smr, out Mesh cached ) && cached != null && !cached.name.Contains( "(Outline Clone)" ) )
            {
                return cached;
            }

            Mesh current = smr.sharedMesh;
            if ( !current ) return null;

            if ( !current.name.Contains( "(Outline Clone)" ) )
            {
                _originalSkinnedMeshes[smr] = current;
                return current;
            }

            #if UNITY_EDITOR
            SkinnedMeshRenderer prefabSmr = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource( smr );
            if ( prefabSmr != null && prefabSmr.sharedMesh != null && !prefabSmr.sharedMesh.name.Contains( "(Outline Clone)" ) )
            {
                _originalSkinnedMeshes[smr] = prefabSmr.sharedMesh;
                return prefabSmr.sharedMesh;
            }

            #if UNITY_2023_1_OR_NEWER
            var allOutlines = UnityEngine.Object.FindObjectsByType<AdvancedOutline>( FindObjectsInactive.Include, FindObjectsSortMode.None );
            #else
            var allOutlines = UnityEngine.Object.FindObjectsOfType<AdvancedOutline>( true );
            #endif
            foreach ( var other in allOutlines )
            {
                if ( other == null ) continue;
                foreach ( var pair in other._clonedSkinnedMeshes )
                {
                    if ( pair.Value == current && other._originalSkinnedMeshes.TryGetValue( pair.Key, out Mesh orig ) && orig != null && !orig.name.Contains( "(Outline Clone)" ) )
                    {
                        _originalSkinnedMeshes[smr] = orig;
                        return orig;
                    }
                }
            }
            #endif

            _originalSkinnedMeshes[smr] = current;
            return current;
        }

        void LoadSmoothNormals()
        {
            if ( _meshFilters != null )
            {
                foreach ( MeshFilter meshFilter in _meshFilters )
                {
                    if ( !meshFilter ) continue;

                    Mesh originalMesh = ResolveOriginalMesh( meshFilter );
                    if ( !originalMesh ) continue;

                    Mesh meshCopy;
                    if ( !_clonedMeshes.TryGetValue( meshFilter, out meshCopy ) || meshCopy == null )
                    {
                        meshCopy = Instantiate( originalMesh );
                        meshCopy.name = originalMesh.name + " (Outline Clone)";
                        meshCopy.hideFlags = HideFlags.DontSave;
                        _clonedMeshes[meshFilter] = meshCopy;
                        meshFilter.sharedMesh = meshCopy;
                    }

                    List<Vector3> smoothNormals;
                    if ( !_runtimeBakeCache.TryGetValue( originalMesh, out smoothNormals ) )
                    {
                        smoothNormals = SmoothNormals( originalMesh );
                        _runtimeBakeCache[originalMesh] = smoothNormals;
                    }

                    meshCopy.SetUVs( 3, smoothNormals );

                    Renderer renderer = meshFilter.GetComponent<Renderer>();
                    if ( renderer )
                    {
                        CombineSubmeshes( meshCopy, renderer.sharedMaterials );
                    }
                }
            }

            if ( _skinnedMeshRenderers != null )
            {
                foreach ( SkinnedMeshRenderer skinnedMeshRenderer in _skinnedMeshRenderers )
                {
                    if ( !skinnedMeshRenderer ) continue;

                    Mesh originalMesh = ResolveOriginalSkinnedMesh( skinnedMeshRenderer );
                    if ( !originalMesh ) continue;

                    Mesh meshCopy;
                    if ( !_clonedSkinnedMeshes.TryGetValue( skinnedMeshRenderer, out meshCopy ) || meshCopy == null )
                    {
                        meshCopy = Instantiate( originalMesh );
                        meshCopy.name = originalMesh.name + " (Outline Clone)";
                        meshCopy.hideFlags = HideFlags.DontSave;
                        _clonedSkinnedMeshes[skinnedMeshRenderer] = meshCopy;
                        skinnedMeshRenderer.sharedMesh = meshCopy;
                    }

                    List<Vector3> smoothNormals;
                    if ( !_runtimeBakeCache.TryGetValue( originalMesh, out smoothNormals ) )
                    {
                        smoothNormals = SmoothNormals( originalMesh );
                        _runtimeBakeCache[originalMesh] = smoothNormals;
                    }

                    meshCopy.SetUVs( 3, smoothNormals );

                    CombineSubmeshes( meshCopy, skinnedMeshRenderer.sharedMaterials );
                }
            }
        }

        List<Vector3> SmoothNormals( Mesh mesh )
        {
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            List<Vector3> smoothNormals = new List<Vector3>( normals );

            Dictionary<Vector3, List<int>> groups = new Dictionary<Vector3, List<int>>( vertices.Length );

            for ( int i = 0; i < vertices.Length; i++ )
            {
                Vector3 vertex = vertices[i];
                List<int> group;
                if ( !groups.TryGetValue( vertex, out group ) )
                {
                    group = new List<int>();
                    groups[vertex] = group;
                }
                group.Add( i );
            }

            foreach ( KeyValuePair<Vector3, List<int>> group in groups )
            {
                if ( group.Value.Count == 1 ) continue;

                Vector3 smoothNormal = Vector3.zero;
                for ( int i = 0; i < group.Value.Count; i++ )
                {
                    smoothNormal += normals[group.Value[i]];
                }

                smoothNormal.Normalize();

                for ( int i = 0; i < group.Value.Count; i++ )
                {
                    smoothNormals[group.Value[i]] = smoothNormal;
                }
            }

            return smoothNormals;
        }

        void CombineSubmeshes( Mesh mesh, Material[] materials )
        {
            if ( mesh.subMeshCount == materials.Length + 1 ) return;
            if ( mesh.subMeshCount > materials.Length ) return;

            mesh.subMeshCount++;
            mesh.SetTriangles( mesh.triangles, mesh.subMeshCount - 1 );
        }

        #if UNITY_EDITOR
        public void EditorBake()
        {
            Bake();
            InitializeBakeCache();
            RefreshRenderers();
        }

        void Bake()
        {
            var bakedMeshes = new HashSet<Mesh>();
            _bakeKeys.Clear();
            _bakeValues.Clear();

            CacheComponents();
            if ( _meshFilters == null ) return;

            foreach ( MeshFilter meshFilter in _meshFilters )
            {
                if ( !meshFilter || !meshFilter.sharedMesh ) continue;
                Mesh orig = ResolveOriginalMesh( meshFilter );
                if ( !orig || !bakedMeshes.Add( orig ) ) continue;

                List<Vector3> smoothNormals = SmoothNormals( orig );
                _bakeKeys.Add( orig );
                _bakeValues.Add( new ListVector3() { data = smoothNormals } );
            }
        }

        [ContextMenu("Rebake Normals")]
        void ContextMenuRebake() => EditorBake();
        #endif

        void UpdateStaticMaterialProperties()
        {
            if ( !_outlineMaskMaterial || !_outlineFillMaterial ) return;

            switch ( _outlineMode )
            {
                case Mode.OutlineAll:
                    _outlineMaskMaterial.SetFloat( _zTestId, (float)UnityEngine.Rendering.CompareFunction.Always );
                    _outlineFillMaterial.SetFloat( _zTestId, (float)UnityEngine.Rendering.CompareFunction.Always );
                    break;

                case Mode.OutlineVisible:
                    _outlineMaskMaterial.SetFloat( _zTestId, (float)UnityEngine.Rendering.CompareFunction.Always );
                    _outlineFillMaterial.SetFloat( _zTestId, (float)UnityEngine.Rendering.CompareFunction.LessEqual );
                    break;

                case Mode.OutlineHidden:
                    _outlineMaskMaterial.SetFloat( _zTestId, (float)UnityEngine.Rendering.CompareFunction.Always );
                    _outlineFillMaterial.SetFloat( _zTestId, (float)UnityEngine.Rendering.CompareFunction.Greater );
                    break;

                case Mode.OutlineAndSilhouette:
                    _outlineMaskMaterial.SetFloat( _zTestId, (float)UnityEngine.Rendering.CompareFunction.LessEqual );
                    _outlineFillMaterial.SetFloat( _zTestId, (float)UnityEngine.Rendering.CompareFunction.Always );
                    break;

                case Mode.SilhouetteOnly:
                    _outlineMaskMaterial.SetFloat( _zTestId, (float)UnityEngine.Rendering.CompareFunction.LessEqual );
                    _outlineFillMaterial.SetFloat( _zTestId, (float)UnityEngine.Rendering.CompareFunction.Greater );
                    break;
            }

            if ( _propertyBlock == null )
            {
                _propertyBlock = new MaterialPropertyBlock();
            }

            _propertyBlock.Clear();
            _propertyBlock.SetColor( _outlineColorId, _outlineColor );
            _propertyBlock.SetFloat( _outlineWidthId, _outlineMode == Mode.SilhouetteOnly ? 0f : _outlineWidth );
            _propertyBlock.SetFloat( _dashScaleId, _dashedPattern ? _dashScale : 0f );
            _propertyBlock.SetFloat( _dashSpeedId, _dashedPattern ? _dashSpeed : 0f );

            ApplyPropertyBlock();
        }

        void HandleDynamicEffects()
        {
            if ( !_outlineFillMaterial ) return;

            bool hasAnimations = _pulseWidth || _rainbowColor || _dashedPattern;
            if ( !hasAnimations ) return;

            Color color = _outlineColor;
            if ( _rainbowColor )
            {
                float hue = ( Time.time * _rainbowSpeed ) % 1.0f;
                color = Color.HSVToRGB( hue, 1f, 1f );
            }

            float width = _outlineWidth;
            if ( _outlineMode == Mode.SilhouetteOnly )
            {
                width = 0f;
            }
            else if ( _pulseWidth )
            {
                float lerpVal = ( Mathf.Sin( Time.time * _pulseSpeed ) + 1f ) / 2f;
                width = Mathf.Lerp( _pulseMinWidth, _pulseMaxWidth, lerpVal );
            }

            float scale = _dashedPattern ? _dashScale : 0f;
            float speed = _dashedPattern ? _dashSpeed : 0f;

            if ( _propertyBlock == null )
            {
                _propertyBlock = new MaterialPropertyBlock();
            }

            _propertyBlock.Clear();
            _propertyBlock.SetColor( _outlineColorId, color );
            _propertyBlock.SetFloat( _outlineWidthId, width );
            _propertyBlock.SetFloat( _dashScaleId, scale );
            _propertyBlock.SetFloat( _dashSpeedId, speed );

            ApplyPropertyBlock();
        }

        void ApplyPropertyBlock()
        {
            if ( _renderers == null ) return;

            foreach ( Renderer renderer in _renderers )
            {
                if ( renderer )
                {
                    renderer.SetPropertyBlock( _propertyBlock );
                }
            }
        }

        void RestoreOriginalMeshes()
        {
            foreach ( KeyValuePair<MeshFilter, Mesh> pair in _originalMeshes )
            {
                if ( pair.Key && pair.Value )
                {
                    pair.Key.sharedMesh = pair.Value;
                }
            }

            foreach ( KeyValuePair<SkinnedMeshRenderer, Mesh> pair in _originalSkinnedMeshes )
            {
                if ( pair.Key && pair.Value )
                {
                    pair.Key.sharedMesh = pair.Value;
                }
            }
        }

        void DestroyClonedMeshes()
        {
            foreach ( KeyValuePair<MeshFilter, Mesh> pair in _clonedMeshes )
            {
                if ( pair.Value )
                {
                    DestroyOutlineObject( pair.Value );
                }
            }
            _clonedMeshes.Clear();
            _originalMeshes.Clear();

            foreach ( KeyValuePair<SkinnedMeshRenderer, Mesh> pair in _clonedSkinnedMeshes )
            {
                if ( pair.Value )
                {
                    DestroyOutlineObject( pair.Value );
                }
            }
            _clonedSkinnedMeshes.Clear();
            _originalSkinnedMeshes.Clear();
        }

        void DestroyOutlineObject( UnityEngine.Object obj )
        {
            if ( !obj ) return;
            if ( Application.isPlaying )
            {
                Destroy( obj );
            }
            else
            {
                DestroyImmediate( obj );
            }
        }
    }
}