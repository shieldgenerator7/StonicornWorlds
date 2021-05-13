using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagnifyAction : ToolAction
{
    public override Color color => Color.white;

    public override bool isActionValidAt(Vector2 pos) => true;

    public override void takeAction(List<Vector2> posList)
    {
        Vector2 center = posList.Aggregate((sum, v) => sum + v) / posList.Count;
        FindObjectOfType<CameraController>().Locked = true;
        FindObjectOfType<CameraController>().autoFrame(center, posList);
    }
}
