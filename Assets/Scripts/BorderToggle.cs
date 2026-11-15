using UnityEngine;

public class BorderToggle : MonoBehaviour
{
    private GameObject border;
    void Start()
    {
        border = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        
    }

    public void ToggleBorder()
    {
        if (border.activeSelf)
        {
            border.SetActive(false);
        } else
        {
            border.SetActive(true);
        }
    }
}
