using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Planet
{
    public Vector2 position;
    public float size = 0.5f;
    [NonSerialized]
    float SQRT_3 = Mathf.Sqrt(3.0f);

    [SerializeField]
    private List<Pod> pods = new List<Pod>();
    public List<QueueTask> tasks = new List<QueueTask>();
    [NonSerialized]
    private HexagonGrid<Pod> grid = new HexagonGrid<Pod>();
    [NonSerialized]
    private GroupedList<PodType, Pod> podLists = new GroupedList<PodType, Pod>(
        pod => pod.podType
        );

    public List<Stonicorn> residents = new List<Stonicorn>();

    public void inflate()
    {
        this.pods.ForEach(p =>
        {
            p.worldPos = gridToWorld(p.gridPos);
            p.inflate();
            grid.add(p.gridPos, p);
            podLists.Add(p);
        });
        this.tasks.ForEach(t => t.inflate());
        //0.020 save backwards compatibility
        if (residents == null)
        {
            residents = new List<Stonicorn>();
            Managers.Planet.Planet.Pods(Managers.Constants.corePodType)
                .ForEach(pod => residents.Add(new Stonicorn()));
        }
        //Inflat residents
        residents.ForEach(t => t.inflate());
    }

    #region Write State
    public delegate void OnStateChanged(Planet p);
    public event OnStateChanged onStateChanged;

    public void addPod(Pod pod, Vector2 pos)
    {
        pod.gridPos = worldToGrid(pos);
        pod.worldPos = gridToWorld(pod.gridPos);
        pods.Add(pod);
        Pod oldPod = grid.get(pod.gridPos);
        grid.add(pod.gridPos, pod);
        podLists.Add(pod);
        if (oldPod)
        {
            pods.Remove(oldPod);
            podLists.Remove(oldPod);
        }
        pod.onPodContentChanged += podContentAdded;
        fillSpaceAround(pod.worldPos);
        //Stonicorn
        if (pod.podType == Managers.Constants.corePodType)
        {
            Stonicorn stonicorn = new Stonicorn();
            stonicorn.bodyColor = Managers.Constants.bodyColors[Random.Range(0, Managers.Constants.bodyColors.Count)];
            stonicorn.hairColor = Managers.Constants.hairColors[Random.Range(0, Managers.Constants.hairColors.Count)];
            stonicorn.homePosition = pod.worldPos;
            stonicorn.position = stonicorn.homePosition;
            residents.Add(stonicorn);
        }
        //Call Delegate
        onStateChanged?.Invoke(this);
    }

    public void removePod(Vector2 pos)
    {
        Vector3Int v = worldToGrid(pos);
        Pod pod = grid.get(v);
        grid.removeAt(v);
        podLists.Remove(pod);
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
                podLists.Add(pod);
                pod.onPodContentChanged += podContentAdded;
            }
            );
        }
    }
    #endregion

    #region Read State
    public Vector2 getHexPos(Vector2 pos)
        => gridToWorld(worldToGrid(pos));

    public List<Vector2> getHexPosBetween(Vector2 start, Vector2 end)
    {
        Vector3Int startHex = worldToGrid(start);
        Vector3Int endHex = worldToGrid(end);
        return HexagonUtility.getLine(startHex, endHex)
            .ConvertAll(v => gridToWorld(v));
    }

    public List<Vector2> getHexPosRing(Vector2 sample)
    {
        return HexagonUtility.getRing(HexagonUtility.ring(worldToGrid(sample)))
           .ConvertAll(v => gridToWorld(v));
    }

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

    public Vector2 getCeilingPos(Vector2 pos)
        => gridToWorld(HexagonUtility.getCeilingPos(worldToGrid(pos)));

    public Vector2 getUpDirection(Vector2 pos)
    {
        Vector2 hexPos = getHexPos(pos);
        if (hexPos == Vector2.zero)
        {
            return Vector2.up;
        }
        return (hexPos - getGroundPos(pos)).normalized;
    }

    public Neighborhood<Pod> getNeighborhood(Vector2 pos)
        => grid.getNeighborhood(worldToGrid(pos));

    public List<Vector2> getEmptyNeighborhood(Vector2 pos)
        => grid.getEmptyNeighborhood(worldToGrid(pos))
            .ConvertAll(v => gridToWorld(v));

    public List<Pod> PodsAll
        => pods.ToList();

    public List<Pod> Pods(PodType podType)
    {
        if (!podType)
        {
            Debug.LogError("podType is null!: " + podType);
        }
        return podLists.GetList(podType);
    }

    public List<Pod> PodsNotEmpty
        => PodsNot(Managers.Constants.spacePodType);

    public List<Pod> PodsNot(PodType podType)
    {
        if (!podType)
        {
            Debug.LogError("podType is null!: " + podType);
        }
        return pods.FindAll(pod => pod.podType != podType);
    }

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

        return HexagonUtility.round(q, s, r);
    }
    #endregion

    public Planet deepCopy()
    {
        string json = JsonUtility.ToJson(this);
        Planet planet = JsonUtility.FromJson<Planet>(json);
        planet.inflate();
        return planet;
    }
}
