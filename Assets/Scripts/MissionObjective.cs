using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            GetComponent<SpriteRenderer>().enabled = false;
        }
        if (canDestroyOnClick && Managers.Planet.Planet.PodsAll.Count > 7)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canDestroyOnClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Managers.Processor.FastForwardPercentDone == 1)
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
                    GetComponent<SpriteRenderer>().enabled = true;
                    canDestroyOnClick = true;
                }
            }
        }
    }
}
