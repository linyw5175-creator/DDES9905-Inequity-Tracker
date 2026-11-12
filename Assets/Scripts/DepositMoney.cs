using Unity.VisualScripting;
using UnityEngine;

public class DepositMoney : MonoBehaviour
{
    [SerializeField] private Graph graph;
    public int startingBalance;
    public int currentBalance;
    public int withInterest;
    public float interestRate;

    void Start()
    {
        currentBalance = startingBalance;
        withInterest = startingBalance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Money"))
        {
        GameObject depositedMoney = other.GameObject();
        currentBalance += Mathf.RoundToInt(depositedMoney.GetComponent<MoneyValue>().valueAmount);
        withInterest += Mathf.RoundToInt(depositedMoney.GetComponent<MoneyValue>().valueAmount);
        withInterest += Mathf.RoundToInt(withInterest * interestRate);
        graph.AddBar();
        Destroy(depositedMoney);
        }
        
    }
}
