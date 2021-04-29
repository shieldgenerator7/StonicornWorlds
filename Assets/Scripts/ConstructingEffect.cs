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
        pod.onProgressChanged += updateDisplay;
        updateDisplay(pod.Progress);
    }

    private void OnDestroy()
    {
        pod.onProgressChanged -= updateDisplay;
    }

    public void updateDisplay(float progress)
    {
        Vector2 size = fillSR.size;
        size.y = progress / pod.podType.progressRequired;
        fillSR.size = size;
    }
}
