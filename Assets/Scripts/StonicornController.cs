using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonicornController : MonoBehaviour
{
    public Stonicorn stonicorn;

    // Update is called once per frame
    void Update()
    {
        if (stonicorn.atHome)
        {
            stonicorn.Rest += stonicorn.restSpeed * Time.deltaTime;
            if (stonicorn.Rest == stonicorn.maxRest)
            {
                stonicorn.resting = false;
            }
        }
        if (!stonicorn.resting)
        {
            if (stonicorn.task == null)
            {
                stonicorn.task = Managers.Queue.getClosestTask(stonicorn.position);
                if (stonicorn.task)
                {
                    stonicorn.locationOfInterest = stonicorn.task.pos;
                }
            }
            else if (stonicorn.locationOfInterest != stonicorn.task.pos)
            {
                stonicorn.locationOfInterest = stonicorn.task.pos;
            }
        }
        if (stonicorn.position != stonicorn.locationOfInterest)
        {
            moveToLocationOfInterest();
        }
        bool atWorkSite = !stonicorn.atHomeOrGoing && stonicorn.isAtLocationOfInterest;
        if (atWorkSite && stonicorn.task != null)
        {
            //Work
            bool completed = Managers.Queue.workOnTask(stonicorn.task, stonicorn.workRate);
            if (completed || !stonicorn.task.Started)
            {
                stonicorn.goHome();
            }
            else
            {
                stonicorn.Rest -= stonicorn.workRate * Time.deltaTime;
            }
            if (stonicorn.Rest == 0)
            {
                stonicorn.resting = true;
                stonicorn.goHome();
            }
        }
    }

    void moveToLocationOfInterest()
    {
        Vector2 aboveLoI = stonicorn.locationOfInterest;
        if (!stonicorn.atHomeOrGoing)
        {
            aboveLoI = Managers.Planet.Planet.getCeilingPos(stonicorn.locationOfInterest);
        }
        if (Vector2.Distance(stonicorn.position, aboveLoI) > 1.0f)
        {
            stonicorn.position +=
                (aboveLoI - stonicorn.position).normalized
                * stonicorn.moveSpeed * Time.deltaTime;
        }
        else
        {
            stonicorn.position = Vector2.Lerp(
                stonicorn.position,
                aboveLoI,
                Time.deltaTime * stonicorn.moveSpeed
                );
        }
        if (Vector2.Distance(stonicorn.position, aboveLoI) <= 0.01f)
        {
            stonicorn.position = aboveLoI;
        }
    }
}
