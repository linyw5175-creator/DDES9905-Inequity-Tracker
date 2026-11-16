using TMPro;
using UnityEngine;

public class FinalBalanceNote : MonoBehaviour
{
    private TextMeshPro text;
    [SerializeField] private DepositMoney balance;
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

        text.text = "Over the past 45 years, you made $" + balance.currentBalance.ToString() + ".";
    }
}
