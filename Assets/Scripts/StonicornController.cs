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
            rest();
        }
        if (stonicorn.action == Stonicorn.Action.IDLE)
        {
            focusTask();
        }
        if (stonicorn.position != stonicorn.locationOfInterest)
        {
            moveToLocationOfInterest();
        }
        if (AtWorkSite)
        {
            work();
            checkRest();
        }
    }

    void moveToLocationOfInterest()
    {
        Vector2 aboveLoI = stonicorn.locationOfInterest;
        if (!stonicorn.atHomeOrGoing)
        {
            aboveLoI = Managers.Planet.Planet.getCeilingPos(stonicorn.locationOfInterest);
            if (stonicorn.workRange != 1)
            {
                aboveLoI = stonicorn.locationOfInterest +
                    ((aboveLoI - stonicorn.locationOfInterest).normalized
                    * (stonicorn.workRange - 0.1f));
            }
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

    void rest()
    {
        float prevRest = stonicorn.Rest;
        stonicorn.Rest += stonicorn.restSpeed * Time.deltaTime;
        if (stonicorn.Rest == stonicorn.maxRest)
        {
            stonicorn.action = Stonicorn.Action.IDLE;
            //if just now finished resting
            if (prevRest != stonicorn.maxRest)
            {
                if (stonicorn.isHomeHouse())
                {
                    stonicorn.restSpeed += 1;
                    stonicorn.maxRest += 10;
                    stonicorn.Rest = stonicorn.maxRest;
                }
            }
        }
    }

    void focusTask()
    {
        if (stonicorn.task == null)
        {
            stonicorn.task = Managers.Queue.getClosestTask(
                stonicorn.position,
                stonicorn.homePosition
                );
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

    bool AtWorkSite
        => !stonicorn.atHomeOrGoing
        && stonicorn.isAtLocationOfInterest
        && stonicorn.task != null;

    void work()
    {
        bool completed = Managers.Queue.workOnTask(stonicorn.task, stonicorn.workRate);
        if (completed || !stonicorn.task.Started)
        {
            stonicorn.goHome();
        }
        else
        {
            stonicorn.Rest -= stonicorn.workRate * Time.deltaTime;
        }
    }

    void checkRest()
    {
        if (stonicorn.Rest == 0)
        {
            stonicorn.action = Stonicorn.Action.REST;
            stonicorn.goHome();
        }
    }
}
