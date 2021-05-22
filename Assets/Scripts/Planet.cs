using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class Planet
{
    public Vector2 position;
    public float size = 0.5f;
    [NonSerialized]
    float SQRT_3 = Mathf.Sqrt(3.0f);

    [SerializeField]
    private List<Pod> pods = new List<Pod>();
    [NonSerialized]
    private HexagonGrid<Pod> grid = new HexagonGrid<Pod>();
    [NonSerialized]
    private GroupedList<PodType, Pod> podLists = new GroupedList<PodType, Pod>(
        pod => pod.podType
        );
    public void init()
    {
        this.pods.ForEach(p =>
        {
            p.worldPos = gridToWorld(p.gridPos);
            p.inflate();
            grid.add(p.gridPos, p);
            podLists.add(p);
        });
    }

    #region Write State
    public delegate void OnStateChanged(Planet p);
    public event OnStateChanged onStateChanged;

    public void addPod(Pod pod, Vector2 pos)
    {
        pod.gridPos = worldToGrid(pos);
        pod.worldPos = gridToWorld(pod.gridPos);
        pods.Add(pod);
        grid.add(pod.gridPos, pod);
        podLists.add(pod);
        pod.onPodContentChanged += podContentAdded;
        fillSpaceAround(pod.worldPos);
        onStateChanged?.Invoke(this);
    }

    public void removePod(Vector2 pos)
    {
        Vector3Int v = worldToGrid(pos);
        Pod pod = grid.get(v);
        grid.removeAt(v);
        podLists.remove(pod);
        pod.onPodContentChanged -= podContentAdded;
        onStateChanged?.Invoke(this);
    }

    private void podContentAdded(Pod p)
    {
        onStateChanged?.Invoke(this);
    }

    private void fillSpaceAround(Vector2 pos)
    {
        Vector3Int hexpos = worldToGrid(pos);
        List<Vector3Int> emptySpaces = HexagonUtility.getNeighborhood(hexpos).neighbors.ToList()
            .FindAll(
                v => grid.get(v) == null
            );
        if (emptySpaces.Count > 0)
        {
            PodType space = Managers.Constants.spacePodType;
            emptySpaces.ForEach(v =>
            {
                Pod pod = new Pod(gridToWorld(v), space);
                pod.gridPos = v;
                pods.Add(pod);
                grid.add(v, pod);
                podLists.add(pod);
                pod.onPodContentChanged += podContentAdded;
            }
            );
        }
    }
    #endregion

    #region Read State
    public Vector2 getHexPos(Vector2 pos)
        => gridToWorld(worldToGrid(pos));

    public Pod getPod(Vector2 pos)
        => grid.get(worldToGrid(pos));

    public bool hasPod(Vector2 pos)
    {
        Pod pod = getPod(pos);
        return pod && pod.podType != Managers.Constants.spacePodType;
    }

    public Pod getGroundPod(Vector2 pos)
        => grid.getGround(worldToGrid(pos));

    public Vector2 getGroundPos(Vector2 pos)
        => gridToWorld(HexagonUtility.getGroundPos(worldToGrid(pos)));

    public Vector2 getUpDirection(Vector2 pos)
        => (getHexPos(pos) - getGroundPos(pos)).normalized;

    public Neighborhood<Pod> getNeighborhood(Vector2 pos)
        => grid.getNeighborhood(worldToGrid(pos));

    public List<Vector2> getEmptyNeighborhood(Vector2 pos)
        => grid.getEmptyNeighborhood(worldToGrid(pos))
            .ConvertAll(v => gridToWorld(v));

    public List<Pod> PodsAll
        => pods.ToList();

    public List<Pod> Pods(PodType podType)
        => podLists.getList(podType);

    public List<Vector2> Border
        => Pods(Managers.Constants.spacePodType)
        .ConvertAll(pod => pod.worldPos);
    //=> grid.getBorder().ConvertAll(v => gridToWorld(v));
    #endregion

    #region Grid Conversion
    private Vector2 gridToWorld(Vector3Int hexpos)
    {
        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#hex-to-pixel-axial
        float x = size * (3.0f * hexpos.x / 2.0f);
        float y = size * SQRT_3 * (hexpos.x / 2.0f + hexpos.z);
        return new Vector2(x, y) + position;
    }
    private Vector3Int worldToGrid(Vector2 pos)
    {
        pos -= position;
        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#pixel-to-hex
        float q = (2.0f * pos.x) / (size * 3.0f);
        float r = (-1 * pos.x + SQRT_3 * pos.y) / (size * 3.0f);
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

    public Planet deepCopy()
    {
        string json = JsonUtility.ToJson(this);
        Planet planet = JsonUtility.FromJson<Planet>(json);
        planet.init();
        return planet;
    }
}
