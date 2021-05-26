using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stonicorn
{
    public Color bodyColor = Color.white;
    public Color hairColor = Color.white;
    public Vector2 position;
    public Vector2 locationOfInterest;
    public float workRate = 20;
    public float moveSpeed = 2;

    public bool isAtLocationOfInterest
        => Vector2.Distance(position, locationOfInterest) <= 1.0f;
}
