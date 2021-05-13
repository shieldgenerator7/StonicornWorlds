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
        float minX = 0;
        float minY = 0;
        float maxX = 0;
        float maxY = 0;
        if (posList.Count > 0)
        {
            minX = posList.Min(pos => pos.x);
            minY = posList.Min(pos => pos.y);
            maxX = posList.Max(pos => pos.x);
            maxY = posList.Max(pos => pos.y);
        }
        Vector2 min = new Vector2(minX, minY);
        Vector2 max = new Vector2(maxX, maxY);
        List<Vector2> boundList = new List<Vector2>()
        {
            min,
            max
        };
        transform.position = ((Vector3)center) + camOffset;
        Camera.main.orthographicSize = minOrthoSize;
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
