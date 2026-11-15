using TMPro;
using UnityEngine;

public class BalanceLabel : MonoBehaviour
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
        text.text = "$"+balance.withInterest.ToString();
    }
}
