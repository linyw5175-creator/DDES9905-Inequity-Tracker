using TMPro;
using UnityEngine;

public class YearsTracker : MonoBehaviour
{
    [SerializeField] SpawnCube cubeCounter;
    private TextMeshPro text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        text.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Years Worked: " + (cubeCounter.cubeNumber-1).ToString();
    }
}
