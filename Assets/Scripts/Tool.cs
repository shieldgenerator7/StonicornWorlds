using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public abstract void inputDown(Vector2 pos);

    public abstract void inputMove(Vector2 pos);

    public abstract void inputUp(Vector2 pos);
}
