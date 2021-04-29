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
        updateDisplay(pod);
    }

    private void OnDestroy()
    {
        FindObjectOfType<QueueManager>().onPodProgressed -= updateDisplay;
    }

    public void updateDisplay(Pod pod)
    {
        if (pod == this.pod)
        {
            Vector2 size = fillSR.size;
            size.y = pod.progress / Pod.PROGRESS_REQUIRED;
            fillSR.size = size;
        }
    }
}
