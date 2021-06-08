using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StonicornController : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        Managers.Planet.Planet.residents
            .ForEach(resident => Update(resident));
    }
    void Update(Stonicorn stonicorn)
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
            moveToLocationOfInterest(stonicorn);
        }
        if (stonicorn.currentActivity)
        {
            if (stonicorn.currentActivity.isInRange)
            {
                if (stonicorn.currentActivity.canContinue)
                {
                    stonicorn.currentActivity.doActivity();
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
            stonicorn.Rest -= stonicorn.passiveExhaustRate * Time.deltaTime;
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

    void moveToLocationOfInterest(Stonicorn stonicorn)
    {
        if (Vector2.Distance(stonicorn.position, stonicorn.aboveLoI) > 1.0f)
        {
            stonicorn.position +=
                (stonicorn.aboveLoI - stonicorn.position).normalized
                * stonicorn.moveSpeed * Time.deltaTime;
        }
        else
        {
            stonicorn.position = Vector2.Lerp(
                stonicorn.position,
                stonicorn.aboveLoI,
                Time.deltaTime * stonicorn.moveSpeed
                );
        }
        if (Vector2.Distance(stonicorn.position, stonicorn.aboveLoI) <= 0.01f)
        {
            stonicorn.position = stonicorn.aboveLoI;
        }
    }
}
