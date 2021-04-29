using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManagerEffects : MonoBehaviour
{
    public QueueManager queueManager;
    public GameObject constructingPrefab;

    List<GameObject> constructs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        queueManager.onQueueChanged += updateDisplay;
    }

    void updateDisplay(Queue<Pod> pods)
    {
        constructs.ForEach(con => Destroy(con));
        constructs.Clear();
        foreach (Pod pod in pods)
        {
            if (pod.Completed)
            {
                //don't process completed pods
                continue;
            }
            GameObject construct = Instantiate(
                constructingPrefab,
                pod.pos,
                Quaternion.identity,
                transform
                );
            ConstructingEffect effect = construct.GetComponent<ConstructingEffect>();
            effect.pod = pod;
            Color color = pod.podType.podPrefab.GetComponent<SpriteRenderer>().color;
            construct.GetComponent<SpriteRenderer>()
                .color = color;
            effect.fillSR
                .color = color;
            constructs.Add(construct);
        }
    }
}
