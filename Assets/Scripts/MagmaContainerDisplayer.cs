using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaContainerDisplayer : PodContentDisplayer
{
    public float circleRadius = 0.13f;
    public List<SpriteRenderer> srs;
    private List<Vector2> originalPosList;

    public override void setup(PodContent content)
    {
        originalPosList = srs.ConvertAll(sr => (Vector2)sr.transform.position);
        for (int i = 0; i < srs.Count; i++)
        {
            Vector2 dir = (Managers.Planet.Planet.position - originalPosList[i]).normalized;
            srs[i].transform.up = -dir;
            srs[i].transform.position = originalPosList[i] + (dir * circleRadius);
        }
        //
        content.onVarChanged += updateDisplay;
        updateDisplay(content.Var);
    }

    void updateDisplay(float val)
    {
        float max = Managers.Resources.magmaCapPerCore;
        float share = max / srs.Count;
        for (int i = 0; i < srs.Count; i++)
        {
            SpriteRenderer fillSR = srs[i];
            Vector2 size = fillSR.size;
            size.y = Mathf.Clamp(val / share, 0.01f, 1);
            fillSR.size = size;
            val -= share;
        }
    }
}
