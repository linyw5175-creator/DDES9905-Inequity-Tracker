using UnityEngine;
using TMPro;
using System;

public class ControlComparisonText : MonoBehaviour
{
private TextMeshPro text;
    [SerializeField] private DepositMoney balance;
    [SerializeField] private DepositMoney controlBalance;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
                text = GetComponent<TextMeshPro>();
                float percentage = (float)balance.currentBalance/controlBalance.currentBalance;
                int percentageRounded = Mathf.RoundToInt(percentage * 100);
                Debug.Log(balance.currentBalance);
                Debug.Log(controlBalance.currentBalance);
                Debug.Log(percentage);
                                Debug.Log(percentageRounded);

        text.text = "In comparison, a person who wasn't facing any inequity made $" + controlBalance.currentBalance.ToString() +". You made " + percentageRounded.ToString() +"% of what they did.";
    }
}
