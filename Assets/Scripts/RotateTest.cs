using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTest : MonoBehaviour
{
    public float angle = 60;
    public bool rotateNow = false;
    public Vector2 up = Vector2.up;
    public Vector2 Up
    {
        get => transform.up;
        set
        {
            up = value;
            transform.up = up;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        up = Vector2.up;
    }

    // Update is called once per frame
    void Update()
    {
        if (rotateNow)
        {
            rotateNow = false;
            rotate();
        }
    }

    public void rotate()
    {
        Up = rotateDirection(up, angle);
    }

    public Vector2 rotateDirection(Vector2 dir, float angle)
    {
        //2020-05-03: written with help from https://stackoverflow.com/a/14609567/2336212
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);
        return new Vector2(
            (dir.x * cos) - (dir.y * sin),
            (dir.x * sin) + (dir.y * cos)
            ); ;
    }
}
