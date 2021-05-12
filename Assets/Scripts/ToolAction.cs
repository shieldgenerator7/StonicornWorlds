using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolAction : MonoBehaviour
{
    public abstract void takeAction(List<Vector2> posList);

    public abstract bool isActionValidAt(Vector2 pos);
}
