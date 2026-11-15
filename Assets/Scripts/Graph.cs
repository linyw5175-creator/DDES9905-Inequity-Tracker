using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] private GameObject bar;
    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;
    [SerializeField] private Transform upperBound;
    [SerializeField] private Transform lowerBound;

    [SerializeField] private DepositMoney balance;

    public float yScaleMultiplier;
    public float heightScale =1;
    public bool billionaireCompare;
    public bool controlCompare;
    [SerializeField] private Graph billionaireGraph;
    [SerializeField] private Graph controlGraph;
    private List<GameObject> bars = new List<GameObject>();

    [SerializeField] private Color barColor;


    void Start()
    {
        heightScale =1;
    }
    public void AddBar()
    {
        GameObject newBar = Instantiate(bar,lowerBound.position,Quaternion.identity,transform);
        newBar.transform.GetChild(0).localScale = new Vector3(1,balance.withInterest*yScaleMultiplier,1);
        newBar.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", barColor);

        bars.Add(newBar);
        
    }

    public void UpdateBars()
    {
        float listLength = bars.Count;
        if (bars.Any())
        {
            for (int i = 0; i<bars.Count; i++)
            {
                        
            bars[i].transform.position = Vector3.Lerp(leftBound.position,rightBound.position,i/listLength);
            bars[i].transform.GetChild(0).position = new Vector3(bars[i].transform.position.x,lowerBound.position.y +(bars[i].transform.GetChild(0).localScale.y/2*heightScale),transform.position.z);

            
            float graphWidth = Vector3.Distance(leftBound.position, rightBound.position);
                if (bars.Count >= graphWidth)
                {
                float xScaleMultiplier = graphWidth/bars.Count;
                float spacing = xScaleMultiplier * 0.1f;
                bars[i].transform.localScale = new Vector3(xScaleMultiplier-spacing, bars[i].transform.localScale.y, bars[i].transform.localScale.z);
                }
            }

    


        }
        
    }

    void Update()
    {
        ScaleHeight();
        UpdateBars();
    }

    public void UpdateHeight(float scale)
    {
        transform.localScale = new Vector3(1,scale,1);
    }

    public void ScaleHeight()
    {
         if (billionaireCompare)
        {
            heightScale = billionaireGraph.heightScale;

        } else if (controlCompare)
        {
            heightScale = controlGraph.heightScale;

        } else
        if (balance.withInterest * yScaleMultiplier > upperBound.position.y)
        {
            heightScale = upperBound.position.y/(balance.withInterest * yScaleMultiplier);
        }
            
            transform.localScale = new Vector3(1,heightScale,1);

    }
}
