using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructingEffect : MonoBehaviour
{
    public SpriteRenderer fillSR;

    private QueueTask task;

    // Start is called before the first frame update
    public void init(QueueTask task)
    {
        if (this.task)
        {
            this.task.onProgressChanged -= updateDisplay;
        }
        this.task = task;
        this.task.onProgressChanged += updateDisplay;
        updateDisplay(this.task.Percent);
    }

    private void OnDestroy()
    {
        task.onProgressChanged -= updateDisplay;
    }

    public void updateDisplay(float percent)
    {
        Vector2 size = fillSR.size;
        size.y = percent;
        fillSR.size = size;
    }
}
