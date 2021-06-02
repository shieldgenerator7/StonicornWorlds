using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class QueueRequirement : Requirement
{
    public int taskCount;

    public override bool isRequirementMet()
        => Managers.Queue.queue.Count >= taskCount;
}
