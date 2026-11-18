using System.Collections.Generic;
using UnityEngine;

public class InequityStringManager : MonoBehaviour
{
    [SerializeField] private InequityMultiplier inequityCheck;
    public List<string> raceInequityPopups;
    public List<string> genderInequityPopups;
    public List<string> disabilityInequityPopups;
    public List<string> educationInequityPopups;
    public List<string> housingInequityPopups;
    public List<string> activeInequityPopups;

    void Start()
    {
        activeInequityPopups = new List<string>();
    }
    public void SetInequityStringList()
    {
        if (inequityCheck.raceInequity)
        {
            activeInequityPopups.AddRange(raceInequityPopups);
        }

        if (inequityCheck.genderInequity)
        {
            activeInequityPopups.AddRange(genderInequityPopups);
        }

        if (inequityCheck.disabilityInequity)
        {
            activeInequityPopups.AddRange(disabilityInequityPopups);
        }

        if (inequityCheck.educationInequity)
        {
            activeInequityPopups.AddRange(educationInequityPopups);
        }

        if (inequityCheck.housingInequity)
        {
            activeInequityPopups.AddRange(housingInequityPopups);
        }
    }
}
