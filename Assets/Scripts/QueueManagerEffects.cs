using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueueManagerEffects : MonoBehaviour
{
    public GameObject constructingPrefab;

    Dictionary<QueueTask, ConstructingEffect> constructs = new Dictionary<QueueTask, ConstructingEffect>();
    public bool showEffects = true;

    public void updateDisplay(List<QueueTask> tasks)
    {
        //Find new tasks
        tasks
            .FindAll(task => !constructs.ContainsKey(task))
            .ForEach(task =>
            {
                GameObject constructPod = Instantiate(
                   constructingPrefab,
                   task.pos,
                   Quaternion.identity,
                   transform
                   );
                constructPod.SetActive(showEffects);
                ConstructingEffect construct = constructPod.GetComponent<ConstructingEffect>();
                construct.transform.up = Managers.Planet.Planet.getUpDirection(task.pos);
                construct.init(task);
                Color color = task.taskObject.uiColor;
                constructPod.GetComponent<SpriteRenderer>()
                    .color = color;
                construct.fillSR
                    .color = color;
                constructs.Add(
                    task,
                    constructPod.GetComponent<ConstructingEffect>()
                    );
            });
        //Remove missing tasks
        constructs.Keys.ToList()
            .FindAll(task => !tasks.Contains(task))
            .ForEach(task =>
            {
                Destroy(constructs[task].gameObject);
                constructs.Remove(task);
            });
    }

    public void setShowEffects(bool active)
    {
        showEffects = active;
        constructs.Values.ToList().ForEach(
            ce => ce.gameObject.SetActive(showEffects)
            );
        Managers.Input.checkAllButtons();
    }
}
