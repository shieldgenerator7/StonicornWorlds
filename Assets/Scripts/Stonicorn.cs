using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stonicorn
{
    //Appearance
    public Color bodyColor = Color.white;
    public Color hairColor = Color.white;
    //Stats
    public float workRate = 20;
    public float moveSpeed = 2;
    public float maxRest = 1000;
    public float restSpeed = 40;
    //Personal Data
    public string name;
    public Vector2 homePosition;
    //Runtime Vars
    public Vector2 position;
    public Vector2 locationOfInterest;
    public float rest = 500;
    public bool resting = true;

    [System.NonSerialized]
    public QueueTask task;

    public bool atHome
        => position == homePosition;

    public bool atHomeOrGoing
        => locationOfInterest == homePosition;

    public void goHome()
    {
        locationOfInterest = homePosition;
        task = null;
    }

    public bool isAtLocationOfInterest
        => Vector2.Distance(position, locationOfInterest) <= 1.0f;

    public float Rest
    {
        get => rest;
        set => rest = Mathf.Clamp(value, 0, maxRest);
    }

    public void inflate()
    {
        task = (atHomeOrGoing)
            ? null
            : Managers.Queue.getClosestTask(locationOfInterest);
    }
}
