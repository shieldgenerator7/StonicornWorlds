using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stonicorn
{
    public Color bodyColor = Color.white;
    public Color hairColor = Color.white;
    public Vector2 position;
    public int queueTaskIndex = -1;
    public float workRate = 20;
}
