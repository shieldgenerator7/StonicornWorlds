using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [System.NonSerialized]
    public List<QueueTask> taskPriorities;

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

    #region Activities

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
    public List<QueueTask> getTaskPriorities()
    {
        if (taskPriorities == null || taskPriorities.Count == 0)
        {
            List<QueueTask> tasks = Managers.Queue.queue.ToList();
            if (tasks.Any(task => task.type == favoriteJobType))
            {
                tasks.RemoveAll(task => task.type != favoriteJobType);
            }
            tasks = sortTasks(tasks, taskPriority2);
            tasks = sortTasks(tasks, taskPriority);
            taskPriorities = tasks;
        }
        taskPriorities.RemoveAll(task => task.Completed);
        return taskPriorities;
    }
    static List<Stonicorn.TaskPriority> descendList = new List<Stonicorn.TaskPriority>() {
        Stonicorn.TaskPriority.FAR,
        Stonicorn.TaskPriority.EXPENSIVE,
        Stonicorn.TaskPriority.SLOW,
        Stonicorn.TaskPriority.STARTED,
        Stonicorn.TaskPriority.LAST,
        Stonicorn.TaskPriority.GROUP,
    };
    private List<QueueTask> sortTasks(List<QueueTask> tasks, Stonicorn.TaskPriority taskPriority)
    {
        System.Func<QueueTask, float> sortFunction = task => 0;
        switch (taskPriority)
        {
            case Stonicorn.TaskPriority.CLOSE:
            case Stonicorn.TaskPriority.FAR:
                sortFunction = task => Vector2.Distance(task.pos, position);
                break;
            case Stonicorn.TaskPriority.CHEAP:
            case Stonicorn.TaskPriority.EXPENSIVE:
                sortFunction = task => task.StartCost;
                break;
            case Stonicorn.TaskPriority.FAST:
            case Stonicorn.TaskPriority.SLOW:
                sortFunction = task => task.taskObject.progressRequired;
                break;
            case Stonicorn.TaskPriority.EMPTY:
            case Stonicorn.TaskPriority.STARTED:
                sortFunction = task => task.Percent;
                break;
            case Stonicorn.TaskPriority.NEXT:
            case Stonicorn.TaskPriority.LAST:
                sortFunction = task => Managers.Queue.queue.IndexOf(task);
                break;
            case Stonicorn.TaskPriority.SOLO:
            case Stonicorn.TaskPriority.GROUP:
                sortFunction = task => Managers.Planet.Planet.residents
                  .FindAll(stncrn => stncrn.task == task)
                  .Count;
                break;
            default:
                Debug.LogError("Unknown TaskPriority!: " + taskPriority);
                break;
        }
        bool descend = descendList.Contains(taskPriority);
        if (descend)
        {
            return tasks.OrderByDescending(sortFunction).ToList();
        }
        else
        {
            return tasks.OrderBy(sortFunction).ToList();
        }
    }
    #endregion

    public Stonicorn()
    {
        initActivities();
    }
}
