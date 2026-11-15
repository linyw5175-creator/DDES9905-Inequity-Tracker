using UnityEngine;

public class ButtonHighlights : MonoBehaviour
{
    [SerializeField] private InequityMultiplier inequityType;
    [SerializeField] private GameObject raceHighlight;
    [SerializeField] private GameObject genderHighlight;
    [SerializeField] private GameObject educationHighlight;
    [SerializeField] private GameObject disabilityHighlight;
    [SerializeField] private GameObject housingHighlight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHighlights()
    {
        if (inequityType.raceInequity)
        {
            raceHighlight.SetActive(true);
        } else
        {
            raceHighlight.SetActive(false);
        }

        if (inequityType.genderInequity)
        {
            genderHighlight.SetActive(true);
        } else
        {
            genderHighlight.SetActive(false);
        }

        if (inequityType.housingInequity)
        {
            housingHighlight.SetActive(true);
        } else
        {
            housingHighlight.SetActive(false);
        }
        
        if (inequityType.disabilityInequity)
        {
            disabilityHighlight.SetActive(true);
        } else
        {
            disabilityHighlight.SetActive(false);
        }
        
        if (inequityType.educationInequity)
        {
            educationHighlight.SetActive(true);
        } else
        {
            educationHighlight.SetActive(false);
        }

    }
}
