using UnityEngine;

public class SpawnCube : MonoBehaviour
{
    [SerializeField] private GameObject moneyCube;
    [SerializeField] private InequityMultiplier inequityTracker;
    public int baseValue;
    public int incrementValue;
    public int cubeNumber;
    public float inequityAdjustment;

    void Start()
    {
        cubeNumber = 1;
    }
    public void SpawnMoney()
    {
        int valueAmount = baseValue + incrementValue* cubeNumber;
        int cubeValue = Mathf.RoundToInt(valueAmount * inequityAdjustment);
        GameObject spawnedCube = Instantiate(moneyCube,transform.position,Quaternion.identity);
        spawnedCube.GetComponent<MoneyValue>().SetValue(cubeValue);
        spawnedCube.GetComponentInChildren<ValueText>().SetValueText(cubeValue);
        cubeNumber++;
    }
    
    public void SetInequity()
    {
        inequityAdjustment = inequityTracker.InequityAdjust();
    }
}
