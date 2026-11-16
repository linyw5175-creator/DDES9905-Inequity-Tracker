using UnityEngine;

public class MoneyValue : MonoBehaviour
{
    public float valueAmount;
    
    public void SetValue(float amount)
    {
        valueAmount = amount;

        if (valueAmount <= 0)
        {
            valueAmount = 0;
        }
/*
        if(1 <= valueAmount && valueAmount <= 5000)
        {
            transform.localScale = new Vector3(.05f,.05f,.05f);
        } else 
        if(501 <= valueAmount && valueAmount <= 150000)
            {
                transform.localScale = new Vector3(valueAmount * .00001f,valueAmount * .00001f,valueAmount * .00001f);
            } else
        if(valueAmount > 150000)
            {
                transform.localScale = new Vector3(1.5f,1.5f,1.5f);
            }*/
    }
}
