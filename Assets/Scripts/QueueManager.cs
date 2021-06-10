using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueueManager : Manager
{
    public float defaultWorkRate = 20;

    public List<QueueTask> queue => Managers.Planet.Planet.tasks;

    public Planet plans;
    public delegate void OnPlansChanged(Planet p);
    public event OnPlansChanged onPlansChanged;

    public void addToQueue(QueueTask task)
    {
        if (queue.Contains(task))
        {
            Debug.LogError("Task already in queue");
            return;
        }
        queue.Add(task);
        plans.updatePlanet(task);
        callOnQueueChanged();
        callOnPlansChanged();
    }
    private void callOnQueueChanged()
    {
        onQueueChanged?.Invoke(queue);
    }
    private void callOnPlansChanged()
    {
        onPlansChanged?.Invoke(plans);
    }
    public delegate void OnQueueChanged(List<QueueTask> queue);
    public event OnQueueChanged onQueueChanged;

    public bool isTaskAvailable(QueueTask task)
    {
        //Context
        switch (task.type)
        {
            case QueueTask.Type.CONSTRUCT:
                return Managers.Planet.canBuildAtPosition(
                    (PodType)task.taskObject,
                    task.pos
                    );
            case QueueTask.Type.CONVERT:
                return Managers.Planet.Planet.hasPod(task.pos) &&
                    Managers.Planet.canBuildAtPosition(
                        (PodType)task.taskObject,
                        task.pos
                        );
            case QueueTask.Type.PLANT:
                return Managers.Planet.canPlantAtPosition(
                    (PodContentType)task.taskObject,
                    task.pos
                    );
            case QueueTask.Type.DESTRUCT:
                return true;
            default:
                Debug.LogError("Unknown task type!: " + task.type);
                return false;
        }
    }

    public QueueTask getClosestTask(Vector2 pos, Vector2 exclude)
        => queue.FindAll(task => isTaskAvailable(task))
            .OrderBy(task => Vector2.Distance(task.pos, pos)).ToList()
            .FindAll(task => task.pos != exclude)
            .FirstOrDefault();

    public List<QueueTask> getAvailableTasks()
        => queue.FindAll(task => isTaskAvailable(task));

    public bool hasEmptyTaskAt(Vector2 pos)
        => queue.Any(task => task.pos == pos && !task.Started);

    public void cancelEmptyTasksAt(Vector2 pos)
    {
        queue
            .FindAll(task => task.pos == pos && !task.Started)
            .ForEach(task =>
            {
                Pod pod = plans.getPod(task.pos);
                if (task.taskObject is PodType pt)
                {
                    plans.removePod(pod);
                    Pod planetPod = Managers.Planet.Planet.getPod(task.pos);
                    if (planetPod && planetPod.podType != Managers.Constants.spacePodType)
                    {
                        pod = JsonUtility.FromJson<Pod>(JsonUtility.ToJson(planetPod));
                        pod.inflate();
                        plans.addPod(pod, pod.worldPos);
                    }
                }
                else if (task.taskObject is PodContentType pct)
                {
                    pod.removeContent(pct);
                }
                queue.Remove(task);
            });
        callOnQueueChanged();
        callOnPlansChanged();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="worker"></param>
    /// <param name="task"></param>
    /// <returns>amount of resources left after working on the task</returns>
    public float workOnTask(QueueTask task, float workRate, float resources)
    {
        if (task == null)
        {
            return resources;
        }
        if (task.Completed)
        {
            //don't work on a task that's already completed
            return resources;
        }
        resources = task.makeProgress(workRate * Time.deltaTime, resources);
        if (task.Completed)
        {
            taskCompleted(task);
        }
        return resources;
    }

    void taskCompleted(QueueTask task)
    {
        queue.Remove(task);
        callOnQueueChanged();
        onTaskCompleted?.Invoke(task);
    }
    public delegate void TaskEvent(QueueTask task);
    public event TaskEvent onTaskCompleted;

    public override void setup()
    {
        callOnQueueChanged();
        calculatePlannedStateFromTasks();
    }

    public void scheduleTasksFromPlans()
    {
        Planet planet = Managers.Planet.Planet;
        List<Pod> planPods = plans.PodsNotEmpty;
        List<Pod> planetPods = planet.PodsNotEmpty;
        if (planPods.Count != planetPods.Count)
        {
            //Check for pods that need added
            planPods
                .FindAll(pod => !planet.hasPod(pod.worldPos))
                .ForEach(pod =>
                {
                    QueueTask task = new QueueTask(pod.podType, pod.worldPos, QueueTask.Type.CONSTRUCT);
                    addToQueue(task);
                });
            //TODO: check for pod contents that need added
            //TODO: check for pods that need destroyed
            //TODO: check for pod contents that need destroyed
        }
    }

    public void calculatePlannedStateFromTasks()
    {
        Planet fp = Managers.Planet.Planet.deepCopy();
        queue.ForEach(task =>
        {
            switch (task.type)
            {
                case QueueTask.Type.CONSTRUCT:
                case QueueTask.Type.CONVERT:
                    fp.addPod(
                        new Pod(task.pos, (PodType)task.taskObject),
                        task.pos
                        );
                    break;
                case QueueTask.Type.PLANT:
                    Pod pod = fp.getPod(task.pos);
                    pod.addContent(
                        new PodContent((PodContentType)task.taskObject, pod)
                        );
                    break;
                default:
                    Debug.LogError("No case for value: " + task.type);
                    break;
            }
        });
        plans = fp;
        callOnPlansChanged();
    }
}
