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
//<<<<<<< HEAD
        int valueAmount = baseValue + incrementValue * cubeNumber;
        float cubeValue = Mathf.Round(valueAmount * inequityAdjustment);
        GameObject spawnedCube = Instantiate(moneyCube, transform.position, Quaternion.identity);
//=======
        int valueAmount = baseValue + incrementValue* cubeNumber;
        int cubeValue = Mathf.RoundToInt(valueAmount * inequityAdjustment);
        GameObject spawnedCube = Instantiate(moneyCube,transform.position,Quaternion.identity);
//>>>>>>> 14e460a1123c0fa51f5f978f7fefec188ebc21b4
        spawnedCube.GetComponent<MoneyValue>().SetValue(cubeValue);
        spawnedCube.GetComponentInChildren<ValueText>().SetValueText(cubeValue);
        cubeNumber++;
    }
    
    public void SetInequity()
    {
        inequityAdjustment = inequityTracker.InequityAdjust();
    }
}
