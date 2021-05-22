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

    private Vector3 Up;
    private Vector3 Position;
    private float ZoomLevel;

    private int pixelWidth;
    private int pixelHeight;

    public void setup()
    {
        camOffset = transform.position - Vector3.zero;
        checkScreenSize();
    }

    private void LateUpdate()
    {
        float deltaTime = 3 * Time.unscaledDeltaTime;
        //Rotate Transform
        if (transform.up != Up)
        {
            //2021-05-13: copied from Stonicorn.CameraController
            transform.up = Vector3.Lerp(transform.up, Up, deltaTime);
            //Fix special case where screen goes black when Camera goes upside down
            if (transform.rotation.eulerAngles.y != 0)
            {
                transform.eulerAngles = new Vector3(0, 0, -180);
            }
            onRotationChanged?.Invoke(transform.up);
        }
        if (transform.position != Position)
        {
            transform.position = Vector3.Lerp(transform.position, Position, deltaTime);
        }
        if (Camera.main.orthographicSize != ZoomLevel)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, ZoomLevel, deltaTime);
        }
        checkScreenSize();
    }
    public delegate void OnRotationChanged(Vector2 up);
    public event OnRotationChanged onRotationChanged;

    void checkScreenSize()
    {
        int spw = Camera.main.scaledPixelWidth;
        int sph = Camera.main.scaledPixelHeight;
        if (pixelWidth != spw || pixelHeight != sph)
        {
            pixelWidth = spw;
            pixelHeight = sph;
            onScreenSizeChanged?.Invoke(pixelWidth, pixelHeight);
        }
    }
    public delegate void OnScreenSizeChanged(int width, int height);
    public event OnScreenSizeChanged onScreenSizeChanged;

    public void autoFrame(List<Vector2> posList)
    {
        if (Locked)
        {
            return;
        }
        //override the posList value
        posList.AddRange(Managers.Planet.Planet.PodsAll.ConvertAll(pod => pod.worldPos));
        //
        autoFrame(Vector2.zero, posList);
    }
    public void autoFrame(Vector2 center, List<Vector2> posList)
    {
        if (posList.Count == 0)
        {
            posList.Add(Vector2.zero);
        }
        Position = ((Vector3)center) + camOffset;
        if (center != Vector2.zero)
        {
            Up = Managers.Planet.Planet.getUpDirection((Vector2)transform.position);
        }
        float maxDist = posList.Max(pos => Vector2.Distance(pos, center));
        ZoomLevel = Mathf.Max(
            minOrthoSize,
            (maxDist * 3.0f / 2.555f) + 0.5f
            );
    }
}
