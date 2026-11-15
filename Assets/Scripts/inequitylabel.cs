using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class inequitylabel : MonoBehaviour
{
    [SerializeField] private InequityMultiplier inequityTracker;

    private TextMeshPro text;
    private List<string> inequityLabels = new List<string>();
    private string inequityString;

    void Start()
    {
        text = GetComponent<TextMeshPro>();
        text.text = "";
    }


    private void RefreshLabels()
    {
        if (inequityTracker.raceInequity && !inequityLabels.Contains("Racial"))
        {
            inequityLabels.Add("Racial");
        } else if (!inequityTracker.raceInequity)
        {
            inequityLabels.Remove("Racial");
        }

        if (inequityTracker.genderInequity && !inequityLabels.Contains("Gender"))
        {
            inequityLabels.Add("Gender");
        } else if (!inequityTracker.genderInequity)
        {
            inequityLabels.Remove("Gender");
        }

        if (inequityTracker.housingInequity && !inequityLabels.Contains("Housing"))
        {
            inequityLabels.Add("Housing");
        } else if (!inequityTracker.housingInequity)
        {
            inequityLabels.Remove("Housing");
        }

        if (inequityTracker.disabilityInequity && !inequityLabels.Contains("Disability"))
        {
            inequityLabels.Add("Disability");
        } else if (!inequityTracker.disabilityInequity)
        {
            inequityLabels.Remove("Disability");
        }

        if (inequityTracker.educationInequity && !inequityLabels.Contains("Education"))
        {
            inequityLabels.Add("Education");
        } else if (!inequityTracker.educationInequity)
        {
            inequityLabels.Remove("Education");
        }
    }

    private void UpdateLabelText()
    {
        inequityString = "";
        if (inequityLabels.Any())
        {
            for (int i=0; i<inequityLabels.Count; i++)
            {
                Debug.Log(inequityLabels[i]);
                if (i == 0)
                {
                    inequityString += inequityLabels[i];

                } else if (i==inequityLabels.Count - 1 && i == 1)
                {
                    inequityString += " and " + inequityLabels[i];

                } else if (i==inequityLabels.Count - 1)
                {
                    inequityString += ", and " + inequityLabels[i];

                }
                else
                {
                    inequityString += ", " + inequityLabels[i];
                }
            }

            text.text = "I am facing " + inequityString + " inequity.";
        } else
        {
            text.text = "I am not facing inequity.";
        }
        

        
    }

    public void ChangeLabel()
    {
        RefreshLabels();
        UpdateLabelText();
    }
}
