using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stonicorn
{
    //Appearance
    public Color bodyColor = Color.white;
    public Color hairColor = Color.gray;
    public Color eyeColor = Color.black;
    //Stats
    public float workRate = 20;
    public float workRange = 1;
    public float moveSpeed = 2;
    public float maxRest = 1000;
    public float restSpeed = 40;
    public float passiveExhaustRate = 4;
    public float toolbeltSize = 300;
    public float transferRate = 100;
    //Personal Data
    public string name;
    public Vector2 homePosition;
    public TaskPriority taskPriority;
    public TaskPriority taskPriority2;
    public QueueTask.Type favoriteJobType;
    //Runtime Vars
    public Vector2 position;
    public Vector2 locationOfInterest;
    public float rest = 500;
    public float toolbeltResources = 0;
    public Action action;
    //Controller Vars
    [System.NonSerialized]
    public List<Activity> activities = new List<Activity>();
    [System.NonSerialized]
    public Activity currentActivity;
    [System.NonSerialized]
    public Vector2 aboveLoI;

    public enum Action
    {
        IDLE,
        WORK,
        REST,
        PICKUP,
        DROPOFF,
    }
    public enum TaskPriority
    {
        CLOSE,//nearby
        FAR,//must travel long way to get there
        CHEAP,//costs hardly any resources
        EXPENSIVE,//costs a lot of resources
        FAST,//requires not a lot of progress
        SLOW,//requires a lot of progress
        EMPTY,//not started
        STARTED,//some progress on it has been made
        NEXT,//next task in the queue
        LAST,//last task in the queue
        SOLO,//no one else is working on it
        GROUP,//at least one other pony is working on it
    }

    [System.NonSerialized]
    public QueueTask task;

    public bool atHomeOrGoing
        => locationOfInterest == homePosition;

    public bool hasHome() => isHomeCore() || isHomeHouse();

    public bool isHomeCore()
    {
        Pod pod = Managers.Planet.Planet.getPod(homePosition);
        return pod.podType == Managers.Constants.corePodType;
    }

    public bool isHomeHouse()
    {
        Pod pod = Managers.Planet.Planet.getPod(homePosition);
        return pod.podType == Managers.Constants.spacePodType
            && pod.hasContent(Managers.Constants.getPodContentType("House"));
    }

    List<Action> magicActions = new List<Action>() {
            Action.WORK,
            Action.PICKUP,
            Action.DROPOFF
        };
    public bool isUsingMagic
       => Vector2.Distance(position, locationOfInterest) <= workRange
        && magicActions.Contains(action);

    public float Rest
    {
        get => rest;
        set => rest = Mathf.Clamp(value, 0, maxRest);
    }

    public bool Sleepy => rest <= maxRest * 0.1f;

    public void inflate()
    {
        task = (atHomeOrGoing)
            ? null
            : Managers.Queue.getClosestTask(locationOfInterest, homePosition);
        //backwards compatibility
        if (eyeColor == Color.black)
        {
            eyeColor = hairColor;
        }
        //
        if (taskPriority == taskPriority2)
        {
            taskPriority = TaskPriority.STARTED;
            taskPriority2 = TaskPriority.NEXT;
        }
        //init activities
        initActivities();
    }

    void initActivities()
    {
        PickupActivity pickup = new PickupActivity(this);
        DropoffActivity dropoff = new DropoffActivity(this);
        activities.Add(new DeliverActivity(this, pickup, dropoff));
        activities.Add(new WorkActivity(this));
        activities.Add(pickup);
        activities.Add(new RestActivity(this));
        activities.Add(dropoff);
    }
    public Stonicorn()
    {
        initActivities();
    }
}
