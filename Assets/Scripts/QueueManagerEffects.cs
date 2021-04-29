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
            GameObject construct = Instantiate(
                constructingPrefab,
                pod.pos,
                Quaternion.identity,
                transform
                );
            Color color = pod.podType.podPrefab.GetComponent<SpriteRenderer>().color;
            construct.GetComponent<SpriteRenderer>()
                .color = color;
            construct.GetComponentInChildren<SpriteRenderer>()
                .color = color;
            constructs.Add(construct);
        }
    }
}
