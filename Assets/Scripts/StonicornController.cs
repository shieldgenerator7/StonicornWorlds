using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StonicornController : MonoBehaviour
{
    public Stonicorn stonicorn;
    List<Activity> activities = new List<Activity>();

    Activity currentActivity;

    private void Start()
    {
        PickupActivity pickup = new PickupActivity(stonicorn);
        DropoffActivity dropoff = new DropoffActivity(stonicorn);
        activities.Add(new DeliverActivity(stonicorn, pickup, dropoff));
        activities.Add(new WorkActivity(stonicorn));
        activities.Add(pickup);
        activities.Add(new RestActivity(stonicorn));
        activities.Add(dropoff);
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentActivity)
        {
            currentActivity = activities.FirstOrDefault(act => act.canStart);
            if (currentActivity)
            {
                stonicorn.action = currentActivity.action;
                currentActivity.goToActivity();
            }
            else
            {
                goIdle();
            }
        }
        if (stonicorn.position != stonicorn.locationOfInterest)
        {
            moveToLocationOfInterest();
        }
        if (currentActivity)
        {
            if (currentActivity.isInRange)
            {
                if (currentActivity.canContinue)
                {
                    currentActivity.doActivity();
                    if (currentActivity.isDone)
                    {
                        goIdle();
                    }
                }
                else
                {
                    goIdle();
                }
            }
        }
        if (stonicorn.action != Stonicorn.Action.REST)
        {
            stonicorn.Rest -= stonicorn.passiveExhaustRate * Time.deltaTime;
        }
    }

    void goIdle()
    {
        currentActivity = null;
        stonicorn.action = Stonicorn.Action.IDLE;
        stonicorn.locationOfInterest = stonicorn.homePosition;
    }

    void moveToLocationOfInterest()
    {
        Vector2 aboveLoI = stonicorn.locationOfInterest;
        if (currentActivity)
        {
            float range = currentActivity.ActivityRange;
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
