using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InequityPopupText : MonoBehaviour
{
    [SerializeField] private InequityMultiplier inequityCheck;

    [SerializeField] private InequityStringManager inequityStrings;
    
   
   void OnEnable()
    {
        TextMeshPro text = GetComponent<TextMeshPro>();
        
        if (inequityStrings.activeInequityPopups.Any())
        {
            String displayText = inequityStrings.activeInequityPopups[UnityEngine.Random.Range(0, inequityStrings.activeInequityPopups.Count)];
            inequityStrings.activeInequityPopups.Remove(displayText);
        
            text.text = displayText;

        } else
        {
            text.text = "";
        }
    }
}
