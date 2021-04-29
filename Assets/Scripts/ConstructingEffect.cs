using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructingEffect : MonoBehaviour
{
    public SpriteRenderer fillSR;

    public Pod pod;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<QueueManager>().onPodProgressed += updateDisplay;
        updateDisplay(pod, 0);
    }

    private void OnDestroy()
    {
        FindObjectOfType<QueueManager>().onPodProgressed -= updateDisplay;
    }

    void updateDisplay(Pod pod, float progress)
    {
        if (pod == this.pod)
        {
            Vector2 size = fillSR.size;
            size.y = progress / 100;
            fillSR.size = size;
        }
    }
}
