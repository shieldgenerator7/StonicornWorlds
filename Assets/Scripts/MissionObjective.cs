using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionObjective : MonoBehaviour
{
    public PodContentType goalObject;
    public int goalReq = 10;
    public bool canDestroyOnClick = false;
    // Start is called before the first frame update
    void Start()
    {
        if (goalObject == null)
        {
            canDestroyOnClick = true;
        }
        else
        {
            GetComponent<Image>().enabled = false;
        }
        if (canDestroyOnClick && Managers.Planet.Planet.PodsAll.Count > 7)
        {
            Destroy(gameObject);
        }
        if (Managers.Processor.FastForwardPercentDone < 1)
        {
            GetComponent<Image>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (canDestroyOnClick)
        {
            if (Managers.Processor.FastForwardPercentDone == 1)
            {
                GetComponent<Image>().enabled = true;
                if (Input.GetMouseButtonDown(0))
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            if (goalObject)
            {
                if (Managers.Planet.Planet.Pods(goalObject).Count > goalReq)
                {
                    GetComponent<Image>().enabled = true;
                    canDestroyOnClick = true;
                }
            }
        }
    }
}
