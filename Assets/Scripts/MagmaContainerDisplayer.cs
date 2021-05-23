using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaContainerDisplayer : PodContentDisplayer
{
    public float val = 0;
    public float circleRadius = 0.13f;
    public List<SpriteRenderer> srs;
    private List<Vector2> originalPosList;
    private float prevVal = 0;

    public override void setup(PodContent content)
    {
        originalPosList = srs.ConvertAll(sr => (Vector2)sr.transform.position);
        //
        content.onVarChanged += updateDisplay;
        updateDisplay(content.Var);
    }

    void updateDisplay(float val)
    {
        this.val = val;
        for (int i = 0; i < srs.Count; i++)
        {
            Vector2 dir = Managers.Planet.Planet.position - originalPosList[i];
            srs[i].transform.up = -dir;
            srs[i].transform.position = originalPosList[i] + (dir * circleRadius);
        }

        float max = Managers.Resources.magmaCapPerCore;
        float share = max / srs.Count;
        for (int i = 0; i < srs.Count; i++)
        {
            SpriteRenderer fillSR = srs[i];
            Vector2 size = fillSR.size;
            size.y = Mathf.Clamp(val / share, 0.01f, 1);
            fillSR.size = size;
            val -= share;
            Debug.Log("set size: " + size.y + ", " + share);
        }
    }

    private void Update()
    {
        if (prevVal != val)
        {
            prevVal = val;
            updateDisplay(val);
        }
    }
}
