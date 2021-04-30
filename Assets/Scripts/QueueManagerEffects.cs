using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManagerEffects : MonoBehaviour
{
    public PlanetManager planetManager;
    public QueueManager queueManager;
    public GameObject constructingPrefab;

    List<ConstructingEffect> constructs = new List<ConstructingEffect>();

    // Start is called before the first frame update
    void Start()
    {
        queueManager.onQueueChanged += updateDisplay;
    }

    void updateDisplay(List<Pod> pods)
    {
        constructs.ForEach(con => Destroy(con.gameObject));
        constructs.Clear();
        foreach (Pod pod in pods)
        {
            if (pod.Completed)
            {
                //don't process pods that have been completed
                continue;
            }
            GameObject construct = Instantiate(
                constructingPrefab,
                pod.pos,
                Quaternion.identity,
                transform
                );
            construct.transform.up = planetManager.upDir(construct.transform.position);
            ConstructingEffect effect = construct.GetComponent<ConstructingEffect>();
            effect.pod = pod;
            Color color = pod.podType.podPrefab.GetComponent<SpriteRenderer>().color;
            construct.GetComponent<SpriteRenderer>()
                .color = color;
            effect.fillSR
                .color = color;
            constructs.Add(effect);
        }
    }
}
