using UnityEngine;

public class ChangeDepositAreaPosition : MonoBehaviour
{
    [SerializeField] private Transform directDepositPos;
    [SerializeField] private Transform manualDepositPos;
    void Start()
    {
        transform.position = manualDepositPos.position;
    }

    public void SetDirectDeposit()
    {
        transform.position = directDepositPos.position;
    }

    public void SetManualDeposit()
    {
        transform.position = manualDepositPos.position;
    }
}
