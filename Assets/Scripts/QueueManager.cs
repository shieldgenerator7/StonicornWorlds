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
        if (!task.taskObject.constructible)
        {
            Debug.LogError("task has non-constructible type!: " + task + ", " + task.taskObject.name);
        }
        if (queue.Any(t => t.pos == task.pos && t.taskObject == task.taskObject))
        {
            //don't add tasks that are already in the queue
            return;
        }
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
    public float workOnTask(QueueTask task, float workRate, float resources, float timeDelta)
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
        resources = task.makeProgress(workRate * timeDelta, resources);
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
        if (true || planPods.Count != planetPods.Count)
        {
            //Check for pods that need added
            planPods
                .FindAll(pod => !planet.hasPod(pod.worldPos))
                .ForEach(pod =>
                {
                    QueueTask task = new QueueTask(pod.podType, pod.worldPos, QueueTask.Type.CONSTRUCT);
                    addToQueue(task);
                });
            //Check for pod contents that need added
            plans.PodsAll.ForEach(pod =>
            {
                pod.forEachContent(content =>
                {
                    Pod planetPod = planet.getPod(pod.worldPos);
                    if (content.contentType.constructible &&
                        (!planetPod || !planetPod.hasContent(content.contentType))
                    )
                    {
                        QueueTask task = new QueueTask(content.contentType, pod.worldPos, QueueTask.Type.PLANT);
                        addToQueue(task);
                    }
                });
            });
            //Check for pods that need destroyed
            planetPods
                .FindAll(pod => !plans.hasPod(pod.worldPos))
                .ForEach(pod =>
                {
                    QueueTask task = new QueueTask(pod.podType, pod.worldPos, QueueTask.Type.DESTRUCT);
                    addToQueue(task);
                });
            //TODO: Check for pod contents that need destroyed
            //planetPods.ForEach(pod =>
            //{
            //    pod.forEachContent(content =>
            //    {
            //        Pod plansPod = plans.getPod(pod.worldPos);
            //        if (!plansPod || !plansPod.hasContent(content.contentType))
            //        {
            //            QueueTask task = new QueueTask(content.contentType, pod.worldPos, QueueTask.Type.PLANT);
            //            addToQueue(task);
            //        }
            //    });
            //});
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
                case QueueTask.Type.DESTRUCT:
                    fp.removePod(task.pos);
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
