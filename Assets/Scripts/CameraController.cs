using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public EdgeManager edgeManager;

    // Start is called before the first frame update
    void Start()
    {
        edgeManager.onEdgeListChanged += autoFrame;
    }

    public void autoFrame(List<Vector2> posList)
    {
        float minX = posList.Min(pos => pos.x);
        float minY = posList.Min(pos => pos.y);
        Vector2 min = new Vector2(minX, minY);
        float maxX = posList.Max(pos => pos.x);
        float maxY = posList.Max(pos => pos.y);
        Vector2 max = new Vector2(maxX, maxY);
        List<Vector2> boundList = new List<Vector2>()
        {
            min,
            max
        };
        Camera.main.orthographicSize = 5;
        while (!areVectorsInFrame(boundList, 1))
        {
            Camera.main.orthographicSize += 0.1f;
        }
    }

    public bool areVectorsInFrame(List<Vector2> vectors, float buffer)
    {
        Bounds b = new Bounds();
        b.min = (Vector2)Camera.main.ViewportToWorldPoint(Vector2.zero);
        b.min += (Vector3)Vector2.one * buffer;
        b.max = (Vector2)Camera.main.ViewportToWorldPoint(Vector2.one);
        b.max -= (Vector3)Vector2.one * buffer;
        return vectors.TrueForAll(v => b.Contains(v));
    }
}
