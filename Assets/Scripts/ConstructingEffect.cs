using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructingEffect : MonoBehaviour
{
    public SpriteRenderer fillSR;
    public bool reverseEffect = false;

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
        if (this.task.type == QueueTask.Type.DESTRUCT)
        {
            reverseEffect = true;
            fillSR.color = new Color(1, 1, 1, 0.5f);
        }
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
        if (reverseEffect)
        {
            percent = 1 - percent;
        }
    }
}
