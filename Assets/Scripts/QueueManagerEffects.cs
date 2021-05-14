using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManagerEffects : MonoBehaviour
{
    public GameObject constructingPrefab;

    List<ConstructingEffect> constructs = new List<ConstructingEffect>();

    // Start is called before the first frame update
    void Start()
    {
        Managers.Queue.onQueueChanged += updateDisplay;
    }

    void updateDisplay(List<QueueTask> tasks)
    {
        constructs.ForEach(con => Destroy(con.gameObject));
        constructs.Clear();
        foreach (QueueTask task in tasks)
        {
            if (task.Completed)
            {
                //don't process pods that have been completed
                continue;
            }
            GameObject construct = Instantiate(
                constructingPrefab,
                task.pos,
                Quaternion.identity,
                transform
                );
            construct.transform.up = Managers.Planet.Planet.getUpDirection(construct.transform.position);
            ConstructingEffect effect = construct.GetComponent<ConstructingEffect>();
            effect.init(task);
            Color color = Color.white;
            if (task.taskObject is PodType pt)
            {
                color = pt.uiColor;
            }
            else if (task.taskObject is PodContentType pct)
            {
                color = pct.uiColor;
            }
            construct.GetComponent<SpriteRenderer>()
                .color = color;
            effect.fillSR
                .color = color;
            constructs.Add(effect);
        }
    }
}
