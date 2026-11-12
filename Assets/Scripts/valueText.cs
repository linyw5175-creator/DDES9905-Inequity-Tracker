using TMPro;
using UnityEngine;

public class ValueText : MonoBehaviour
{
    private TextMeshPro text;

    public void SetValueText(float value)
    {
        text = GetComponent<TextMeshPro>();
        text.text = "$" + value.ToString();
    }
}
