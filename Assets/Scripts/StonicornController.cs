using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StonicornController : PlanetProcessor
{
    private void Start()
    {
        Managers.Queue.onPlansChanged += clearStonicornTaskPriorities;
    }

    // Update is called once per frame
    public override void update(float timeDelta)
    {
        for (int i = 0; i < Managers.Planet.Planet.residents.Count; i++)
        {
            update(Managers.Planet.Planet.residents[i], timeDelta);
        }
    }
    void update(Stonicorn stonicorn, float timeDelta)
    {
        if (!stonicorn.currentActivity)
        {
            stonicorn.currentActivity = stonicorn.activities.FirstOrDefault(act => act.canStart);
            if (stonicorn.currentActivity)
            {
                stonicorn.action = stonicorn.currentActivity.action;
                stonicorn.currentActivity.goToActivity();
                stonicorn.aboveLoI = getAboveLoI(stonicorn);
            }
            else
            {
                goIdle(stonicorn);
            }
        }
        if (stonicorn.position != stonicorn.aboveLoI)
        {
            moveToLocationOfInterest(stonicorn, timeDelta);
        }
        if (stonicorn.currentActivity)
        {
            if (stonicorn.currentActivity.isInRange)
            {
                if (stonicorn.currentActivity.canContinue)
                {
                    stonicorn.currentActivity.doActivity(timeDelta);
                    if (stonicorn.currentActivity.isDone)
                    {
                        goIdle(stonicorn);
                    }
                }
                else
                {
                    goIdle(stonicorn);
                }
            }
        }
        if (stonicorn.action != Stonicorn.Action.REST)
        {
            stonicorn.Rest -= stonicorn.passiveExhaustRate * timeDelta;
        }
    }

    void goIdle(Stonicorn stonicorn)
    {
        stonicorn.currentActivity = null;
        stonicorn.action = Stonicorn.Action.IDLE;
        stonicorn.locationOfInterest = stonicorn.homePosition;
        stonicorn.aboveLoI = getAboveLoI(stonicorn);
    }

    Vector2 getAboveLoI(Stonicorn stonicorn)
    {
        Vector2 aboveLoI = stonicorn.locationOfInterest;
        if (stonicorn.currentActivity)
        {
            float range = stonicorn.currentActivity.ActivityRange;
            if (range != 0)
            {
                aboveLoI = Managers.Planet.Planet.getCeilingPos(stonicorn.locationOfInterest);
                if (range != 1)
                {
                    aboveLoI = stonicorn.locationOfInterest +
                        ((aboveLoI - stonicorn.locationOfInterest).normalized
                        * (range - 0.1f));
                }
            }
        }
        return aboveLoI;
    }

    void moveToLocationOfInterest(Stonicorn stonicorn, float timeDelta)
    {
        if (Vector2.Distance(stonicorn.position, stonicorn.aboveLoI) > 1.0f)
        {
            stonicorn.position +=
                (stonicorn.aboveLoI - stonicorn.position).normalized
                * stonicorn.moveSpeed * timeDelta;
        }
        else
        {
            stonicorn.position = Vector2.Lerp(
                stonicorn.position,
                stonicorn.aboveLoI,
                timeDelta * stonicorn.moveSpeed
                );
        }
        if (Vector2.Distance(stonicorn.position, stonicorn.aboveLoI) <= 0.01f)
        {
            stonicorn.position = stonicorn.aboveLoI;
        }
    }

    void clearStonicornTaskPriorities(Planet planet)
    {
        planet.residents.ForEach(
            resident =>
            {
                resident.task = null;
                resident.taskPriorities = null;
            });
    }
}
