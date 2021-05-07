using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float size = 0.5f;
    public float newSize = 0.5f;
    float SQRT_3 = Mathf.Sqrt(3);
    public Vector2 testVector;
    private Vector2 prevTestVector;
    public Vector3Int testVector3;
    private Vector3Int prevTestVector3;

    private void Update()
    {
        if (prevTestVector != testVector)
        {
            prevTestVector = testVector;
            Vector3Int inV = worldToGrid(testVector);
            Vector2 outV = gridToWorld(inV);
            Vector3Int inV2 = worldToGrid(outV);
            Vector2 outV2 = gridToWorld(inV2);
            Debug.Log("TEST 232: " + testVector + " -> " + inV + " -> " + outV
                + " -> " + inV2 + " -> " + outV2);
        }
        if (prevTestVector3 != testVector3)
        {
            prevTestVector3 = testVector3;
            Vector2 inV = gridToWorld(testVector3);
            Vector3Int outV = worldToGrid(inV);
            Vector2 inV2 = gridToWorld(outV);
            Vector3Int outV2 = worldToGrid(inV2);
            Debug.Log("TEST 323: " + testVector3 + " -> " + inV + " -> " + outV
                + " -> " + inV2 + " -> " + outV2);
        }
        if (newSize != size)
        {
            size = newSize;
            grid.grid.ToList().ForEach(
                entry => entry.Value.pos = gridToWorld(entry.Key)
            );
        }
    }

    private HexagonGrid<Pod> grid;
    private HexagonGrid<Pod> futureGrid;

    private void Awake()
    {
        grid = new HexagonGrid<Pod>();
        futureGrid = new HexagonGrid<Pod>();
        newSize = size;
    }

    #region Write State
    public delegate void OnStateChanged(Planet p);
    public event OnStateChanged onStateChanged;

    public void addPod(Pod pod, Vector2 pos)
    {
        Vector3Int hexpos = worldToGrid(pos);
        pod.pos = gridToWorld(hexpos);
        grid.add(pod, worldToGrid(pos));
        onStateChanged?.Invoke(this);
    }

    public void removePod(Vector2 pos)
    {
        grid.removeAt(worldToGrid(pos));
        onStateChanged?.Invoke(this);
    }
    #endregion

    #region Read State
    public Pod getPod(Vector2 pos)
        => grid.get(worldToGrid(pos));

    public Pod getGroundPod(Vector2 pos)
        => grid.getGround(worldToGrid(pos));

    public Vector2 getGroundPos(Vector2 pos)
        => gridToWorld(HexagonUtility.getGroundPos(worldToGrid(pos)));

    public Neighborhood<Pod> getNeighborhood(Vector2 pos)
        => grid.getNeighborhood(worldToGrid(pos));

    public List<Pod> Pods
        => grid;

    public List<Vector2> Border
        => grid.getBorder().ConvertAll(v => gridToWorld(v));
    #endregion

    #region Grid Conversion
    private Vector2 gridToWorld(Vector3Int hexpos)
    {
        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#hex-to-pixel-axial
        float x = size * (3 * hexpos.x / 2);
        float y = size * SQRT_3 * (hexpos.x / 2 + hexpos.z);
        return new Vector2(x, y) + (Vector2)transform.position;
    }
    private Vector3Int worldToGrid(Vector2 pos)
    {
        pos -= (Vector2)transform.position;
        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#pixel-to-hex
        float q = (2 * pos.x) / (size * 3);
        float r = (-1 * pos.x + SQRT_3 * pos.y) / (size * 3);
        float s = -(q + r);

        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#rounding
        float rx = Mathf.Round(q);
        float ry = Mathf.Round(s);
        float rz = Mathf.Round(r);

        float x_diff = Mathf.Abs(rx - q);
        float y_diff = Mathf.Abs(ry - s);
        float z_diff = Mathf.Abs(rz - r);

        if (x_diff > y_diff && x_diff > z_diff)
        {
            rx = -ry - rz;
        }
        else if (y_diff > z_diff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }
        return new Vector3Int((int)rx, (int)ry, (int)rz);
    }
    #endregion
}
