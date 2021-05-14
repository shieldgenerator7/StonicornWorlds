using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minOrthoSize = 5;
    private Vector3 camOffset;

    public bool Locked = false;

    // Start is called before the first frame update
    void Start()
    {
        Managers.Edge.onValidPositionListChanged += autoFrame;
        camOffset = transform.position - Vector3.zero;
    }

    public void autoFrame(List<Vector2> posList)
    {
        if (Locked)
        {
            return;
        }
        //override the posList value
        posList.AddRange(Managers.Planet.Planet.PodsAll.ConvertAll(pod => pod.pos));
        //
        autoFrame(Vector2.zero, posList);
    }
    public void autoFrame(Vector2 center, List<Vector2> posList)
    {
        if (posList.Count == 0)
        {
            posList.Add(Vector2.zero);
        }
        transform.position = ((Vector3)center) + camOffset;
        if (center != Vector2.zero)
        {
            Vector2 upDir = Managers.Planet.Planet.getUpDirection((Vector2)transform.position);
            transform.up = upDir;
        }
        float maxDist = posList.Max(pos => Vector2.Distance(pos, center));
        Camera.main.orthographicSize = Mathf.Max(
            minOrthoSize,
            (maxDist * 3.0f / 2.555f) + 0.5f
            );
    }
}
