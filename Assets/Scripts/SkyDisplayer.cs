using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDisplayer : MonoBehaviour
{
    public PodContentType contentType;

    private PodContent content;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        content = Managers.Planet.Planet
            .getPod(transform.position)
            .getContent(contentType);
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
