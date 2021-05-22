using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDisplayer : MonoBehaviour
{
    public PodContentType contentType;

    private PodContent content;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    public void setup(PodContent content)
    {
        sr = GetComponent<SpriteRenderer>();
        content.onVarChanged += updateDisplay;
        updateDisplay(content.Var);
    }

    private void OnDestroy()
    {
        if (content)
        {
            content.onVarChanged -= updateDisplay;
        }
    }

    void updateDisplay(float val)
    {
        Color color = sr.color;
        color.a = val / 100;
        sr.color = color;
    }
}
