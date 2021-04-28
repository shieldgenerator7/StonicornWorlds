using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlanetManager planetManager;

    // Start is called before the first frame update
    void Start()
    {
        planetManager.addPosListChanged += autoFrame;
    }

    public void autoFrame(List<Vector2> posList)
    {
        float minX = posList.Min(pos => pos.x);
        float minY = posList.Min(pos => pos.y);
        Vector2 min = new Vector2(minX, minY);
        float maxX = posList.Max(pos => pos.x);
        float maxY = posList.Max(pos => pos.y);
        Vector2 max = new Vector2(maxX, maxY);

        Vector2 avg = posList.Aggregate((sum, pos) => pos + sum) / posList.Count;
        Camera.main.transform.position = new Vector3(
            avg.x,
            avg.y,
            Camera.main.transform.position.z
            );
        Camera.main.orthographicSize = 5;
        while (!areVectorsInFrame(posList, 1))
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
