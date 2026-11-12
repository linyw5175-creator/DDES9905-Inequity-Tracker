using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InequityMultiplier : MonoBehaviour
{
    [Header("Inequities")]
    public bool genderInequity;
    public bool raceInequity, disabilityInequity, housingInequity, educationInequity;

    [Header("Inequity Percentage Range (Min - Max)")]
    public Vector2 genderInequityRange; 
    public Vector2 raceInequityRange, disabilityInequityRange, housingInequityRange, educationInequityRange;

    [Header("Multiple Inequity Penalty")]
    public bool multInequityPenalty;
    public float multInequityPenaltyValue;


 
public float InequityAdjust()
    {
        bool [] inequities = {genderInequity, raceInequity, disabilityInequity, housingInequity, educationInequity};
        int inequityCount = inequities.Count(t=>t);


        float genderAdjustment = 100 - Random.Range(genderInequityRange.x, genderInequityRange.y);
        float raceAdjustment = 100 - Random.Range(raceInequityRange.x, raceInequityRange.y);
        float disabilityAdjustment = 100 - Random.Range(disabilityInequityRange.x, disabilityInequityRange.y);
        float housingAdjustment = 100 - Random.Range(housingInequityRange.x, housingInequityRange.y);
        float educationAdjustment = 100 - Random.Range(educationInequityRange.x, educationInequityRange.y);

        float [] inequityAdjustments = {genderAdjustment, raceAdjustment, disabilityAdjustment, housingAdjustment, educationAdjustment};


        List<float> adjustments = new List<float>{};
        
        for(int i = 0; i<inequities.Length; i++)
        {
            if (inequities[i])
            {
                adjustments.Add(inequityAdjustments[i]);
            }
        }

        float adjustmentAmount;
        float multInequityPenaltyAmount;
        if(adjustments.Any())
        {
            adjustmentAmount = adjustments.Max();
        }
        else
        {
            adjustmentAmount = 0;
        }

        if (!multInequityPenalty || inequityCount == 0)
        {
            multInequityPenaltyAmount = 0;
        } else
        {
            multInequityPenaltyAmount = multInequityPenaltyValue * (inequityCount -1);
        }


        float adjustmentMultiplier = Mathf.Round(100 - adjustmentAmount - multInequityPenaltyAmount)/100;

        return adjustmentMultiplier;
    }

    public void ToggleInequity(string inequity)
    {
        if (inequity == "race")
        {
            if (!raceInequity)
            {
                raceInequity = true;
            } else
            {
                raceInequity = false;
            }
        }

        if (inequity == "gender")
        {
            if (!genderInequity)
            {
                genderInequity = true;
            } else
            {
                genderInequity = false;
            }
        }

        if (inequity == "education")
        {
            if (!educationInequity)
            {
                educationInequity = true;
            } else
            {
                educationInequity = false;
            }
        }

        if (inequity == "housing")
        {
            if (!housingInequity)
            {
                housingInequity = true;
            } else
            {
                housingInequity = false;
            }
        }

        if (inequity == "disability")
        {
            if (!disabilityInequity)
            {
                disabilityInequity = true;
            } else
            {
                disabilityInequity = false;
            }
        }

        if (inequity == "none")
        {
            raceInequity = false;
            genderInequity = false;
            educationInequity = false;
            housingInequity = false;
            disabilityInequity = false;
        }

        if (inequity == "random")
        {
            raceInequity = Random.value < 0.5f;
            genderInequity = Random.value < 0.5f;
            educationInequity = Random.value < 0.5f;
            housingInequity = Random.value < 0.5f;
            disabilityInequity = Random.value < 0.5f;
        }
    }
}
