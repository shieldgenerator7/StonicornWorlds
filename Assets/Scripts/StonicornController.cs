using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            checkRest();
            if (stonicorn.action == Stonicorn.Action.IDLE)
            {
                if (stonicorn.toolbeltResources == 0)
                {
                    goToPickupResources();
                }
                else
                {
                    focusTask();
                }
            }
        }
        if (stonicorn.position != stonicorn.locationOfInterest)
        {
            moveToLocationOfInterest();
        }
        if (stonicorn.position == stonicorn.locationOfInterest)
        {
            switch (stonicorn.action)
            {
                case Stonicorn.Action.PICKUP:
                    pickupResources();
                    break;
                case Stonicorn.Action.DROPOFF:
                    dropoffResources();
                    break;
            }
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
            if (Managers.Queue.queue.Count > 0)
            {
                stonicorn.task = Managers.Queue.queue
                    .OrderBy(task => Vector2.Distance(task.pos, stonicorn.position))
                    .ToList()[0];
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
        float prevResources = stonicorn.toolbeltResources;
        stonicorn.toolbeltResources = Managers.Queue.workOnTask(
            stonicorn.task,
            stonicorn.workRate,
            stonicorn.toolbeltResources
            );
        stonicorn.Rest -= stonicorn.workRate * Time.deltaTime;
        //if task finished
        if (prevResources == stonicorn.toolbeltResources)
        {
            stonicorn.goHome();
        }
        //if ran out of resources
        else if (stonicorn.toolbeltResources == 0)
        {
            goToPickupResources();
        }
    }

    void checkRest()
    {
        if (stonicorn.Rest == 0)
        {
            //if (stonicorn.toolbeltResources > 0 &&
            //        Managers.Planet.Planet.getPod(stonicorn.position).podType !=
            //        Managers.Constants.corePodType)
            //{
            //    goToDropoffResources();
            //    stonicorn.task = null;
            //}
            //else
            //{
                stonicorn.action = Stonicorn.Action.REST;
                stonicorn.goHome();
            //}
        }
    }

    void goToPickupResources()
    {
        Vector2 corePos = Managers.Resources.getClosestCore(stonicorn.position);
        stonicorn.locationOfInterest = corePos;
        stonicorn.action = Stonicorn.Action.PICKUP;
    }

    void pickupResources()
    {
        float need = stonicorn.toolbeltSize - stonicorn.toolbeltResources;
        PodContent magma = Managers.Planet.Planet.getPod(stonicorn.position)
            .getContent(Managers.Constants.getPodContentType("MagmaContainer"));
        float give = Mathf.Clamp(need, 0, magma.Var);
        stonicorn.toolbeltResources += give;
        magma.Var -= give;
        stonicorn.action = Stonicorn.Action.IDLE;
    }

    void goToDropoffResources()
    {
        Vector2 corePos = Managers.Resources.getClosestCore(stonicorn.position);
        stonicorn.locationOfInterest = corePos;
        stonicorn.action = Stonicorn.Action.DROPOFF;
    }

    void dropoffResources()
    {
        float excess = stonicorn.toolbeltResources - 0;
        PodContent magma = Managers.Planet.Planet.getPod(stonicorn.position)
            .getContent(Managers.Constants.getPodContentType("MagmaContainer"));
        float take = Mathf.Clamp(excess, 0, Managers.Resources.magmaCapPerCore - magma.Var);
        stonicorn.toolbeltResources = 0;//-= take;
        magma.Var += take;
        stonicorn.action = Stonicorn.Action.IDLE;
    }
}
