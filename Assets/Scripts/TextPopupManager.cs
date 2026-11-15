using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextPopupManager : MonoBehaviour
{
    [System.Serializable]

    public class Popup
    {
        public GameObject popupText;
        public int popupTime;
        public bool requireAnswer;
    }

    public List<Popup> popups;

    [SerializeField] SpawnCube cubeCount;
    [SerializeField] GameObject spawnButton;

    public bool hideSpawnButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (popups.Any())
        {
            foreach (Popup p in popups)
            {
                p.popupText.SetActive(false);
            }
        }

        hideSpawnButton = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (popups.Any())
            {
                foreach (Popup p in popups)
                {
                    if(cubeCount.cubeNumber == p.popupTime)
                    {
                        p.popupText.SetActive(true);

                        if (p.requireAnswer && hideSpawnButton)
                        {
                        spawnButton.SetActive(false);
                        }

                    } else
                    {
                        p.popupText.SetActive(false);
                    }

                    
                }
            }
    }

    public void ShowSpawnButton()
    {
        hideSpawnButton = false;
    }

    public void ResetSpawnButton()
    {
        hideSpawnButton = true;
    }
}
