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
        float maxX = posList.Max(pos => pos.x);
        float maxY = posList.Max(pos => pos.y);
        Camera.main.transform.position = new Vector3(
            (minX + maxX) / 2,
            (minY + maxY) / 2,
            Camera.main.transform.position.z
            );
    }
}
