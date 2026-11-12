using UnityEngine;
using TMPro;

public class BalanceTracker : MonoBehaviour
{
    [SerializeField] private DepositMoney balance;
    private TextMeshPro text;

    void Start()
    {
        text = GetComponent<TextMeshPro>();
    }
    // Update is called once per frame
    void Update()
    {
        text.text = "Total Deposited: $" + balance.currentBalance.ToString() + "\nWith Interest: $"+balance.withInterest.ToString();
    }
}
