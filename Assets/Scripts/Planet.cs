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
    [NonSerialized]
    private MultiGroupedList<PodContentType, Pod> podMultiLists = new MultiGroupedList<PodContentType, Pod>(
        pod => pod.getContentTypes()
        );
    [NonSerialized]
    private Dictionary<GasDiffuser, HexagonGrid<float>> gasGrids = new Dictionary<GasDiffuser, HexagonGrid<float>>();

    public List<Stonicorn> residents = new List<Stonicorn>();

    public void inflate()
    {
        this.pods.ForEach(p =>
        {
            p.worldPos = gridToWorld(p.gridPos);
            p.inflate();
            grid.add(p.gridPos, p);
            podLists.Add(p);
            podMultiLists.Add(p);
        });
        this.tasks.ForEach(t => t.inflate());
        //0.020 save backwards compatibility
        if (residents == null)
        {
            residents = new List<Stonicorn>();
            Managers.Planet.Planet.Pods(Managers.Constants.corePodType)
                .ForEach(pod => residents.Add(
                    GameObject.FindObjectOfType<StonicornGenerator>().generate()
                    ));
        }
        //Inflat residents
        residents.ForEach(t => t.inflate());
    }

    public void prepareForSave()
    {
        gasGrids.Keys.ToList().ForEach(pct => pushGasGrid(pct));
    }

    #region Write State
    public delegate void OnStateChanged(Planet p);
    public event OnStateChanged onStateChanged;

    public void updatePlanet(QueueTask task)
    {
        switch (task.type)
        {
            case QueueTask.Type.CONSTRUCT:
                addPod(new Pod(task.pos, (PodType)task.taskObject), task.pos);
                break;
            case QueueTask.Type.CONVERT:
                addPod(new Pod(task.pos, (PodType)task.taskObject), task.pos);
                break;
            case QueueTask.Type.DESTRUCT:
                removePod(task.pos);
                break;
            case QueueTask.Type.PLANT:
                new PodContent((PodContentType)task.taskObject, getPod(task.pos)
                );
                break;
        }
    }
    public void addPod(Pod pod, Vector2 pos)
    {
        pod.gridPos = worldToGrid(pos);
        pod.worldPos = gridToWorld(pod.gridPos);
        pods.Add(pod);
        Pod oldPod = grid.get(pod.gridPos);
        grid.add(pod.gridPos, pod);
        podLists.Add(pod);
        podMultiLists.Add(pod);
        if (oldPod)
        {
            pods.Remove(oldPod);
            podLists.Remove(oldPod);
            podMultiLists.Remove(oldPod);
        }
        pod.onPodContentChanged += podContentChanged;
        fillSpaceAround(pod.worldPos);
        updateGasGrids(pod);
        //Stonicorn
        if (pod.podType == Managers.Constants.corePodType)
        {
            addResident(pod.worldPos);
        }
        //Call Delegate
        onStateChanged?.Invoke(this);
    }

    public void removePod(Vector2 pos)
    {
        Pod pod = grid.get(worldToGrid(pos));
        removePod(pod);
    }
    public void removePod(Pod pod)
    {
        pod.onPodContentChanged -= podContentChanged;
        pods.Remove(pod);
        grid.removeAt(pod.gridPos);
        podLists.Remove(pod);
        podMultiLists.Remove(pod);
        Pod fillPod = fillSpace(pod.gridPos);
        updateGasGrids(fillPod);
        onStateChanged?.Invoke(this);
    }

    private void podContentChanged(Pod p)
    {
        podMultiLists.Update(p);
        onStateChanged?.Invoke(this);
    }

    public Stonicorn addResident(Vector2 homePos)
    {
        Stonicorn resident = GameObject.FindObjectOfType<StonicornGenerator>().generate();
        resident.homePosition = homePos;
        resident.position = resident.homePosition;
        resident.locationOfInterest = resident.homePosition;
        resident.rest = 0;
        residents.Add(resident);
        onStateChanged?.Invoke(this);
        return resident;
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
            emptySpaces.ForEach(v => fillSpace(v));
        }
    }
    public Pod fillSpace(Vector3Int v)
    {
        PodType space = Managers.Constants.spacePodType;
        Pod pod = new Pod(gridToWorld(v), space);
        pod.gridPos = v;
        pods.Add(pod);
        grid.add(v, pod);
        podLists.Add(pod);
        podMultiLists.Add(pod);
        pod.onPodContentChanged += podContentChanged;
        return pod;
    }
    #endregion

    #region Read State
    public Vector2 getHexPos(Vector3Int v)
        => gridToWorld(v);
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
    public Pod getPod(Vector3Int v)
        => grid[v];

    public bool hasPod(Vector2 pos)
    {
        Pod pod = getPod(pos);
        return pod && pod.podType != Managers.Constants.spacePodType;
    }

    public Pod getClosestPod(Vector2 pos, PodType podType)
        => Pods(podType)
            .OrderBy(pod => Vector2.Distance(pos, pod.worldPos))
            .ToList()[0];

    public Pod getGroundPod(Vector2 pos)
        => grid.get(worldToGrid(getGroundPos(pos)));

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
    public Vector2 getUpDirectionClosestCore(Vector2 pos)
    {
        Vector2 corePos = Pods(Managers.Constants.corePodType)
            .OrderBy(pod => Vector2.Distance(pod.worldPos, pos))
            .ToList()[0].worldPos;
        return getUpDirection(pos - corePos);
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

    public List<Pod> Pods(PodContentType podContentType)
    {
        if (!podContentType)
        {
            Debug.LogError("podContentType is null!: " + podContentType);
        }
        return podMultiLists.GetList(podContentType);
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

    public float PlanetSize
        => HexagonUtility.maxRing(PodsNotEmpty.ConvertAll(pod => pod.gridPos))
            * 2 * size;
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

    #region Gas Grids
    public HexagonGrid<float> getGasGrid(PodContentType podContentType)
    {
        GasDiffuser key = gasGrids.Keys.FirstOrDefault(gd => gd.gasPodContentType == podContentType);
        if (key == null)
        {
            Debug.LogError("No grid found for content type!: " + podContentType);
            return null;
        }
        return gasGrids[key];
    }
    public HexagonGrid<float> getGasGrid(GasDiffuser diffuser)
    {
        if (!gasGrids.ContainsKey(diffuser))
        {
            gasGrids.Add(diffuser, new HexagonGrid<float>());
        }
        return gasGrids[diffuser];
    }
    public float getGasPressure(PodContentType podContentType, Vector3Int v)
    {
        return getGasGrid(podContentType)[v];
    }
    public void pullGasGrid(GasDiffuser diffuser)
    {
        HexagonGrid<float> grid = getGasGrid(diffuser);
        grid.clear();
        Managers.Planet.Planet.PodsAll
            .ForEach(pod =>
            {
                float pressure = 0;
                //Valid place for gas to be
                if (pod.podType == Managers.Constants.spacePodType)
                {
                    PodContent content = pod.getContent(diffuser.gasPodContentType);
                    if (content)
                    {
                        pressure = content.Var;
                    }
                    else
                    {
                        pressure = 0;
                    }
                }
                //Gas emitter
                else if (pod.podType == diffuser.emitterPodType)
                {
                    pressure = diffuser.emitterPressure;
                }
                //Invalid place for gas to be
                else
                {
                    pressure = -1;
                }
                grid.add(pod.gridPos, pressure);
            });
    }
    void updateGasGrids(Pod pod)
    {
        gasGrids.Keys.ToList().ForEach(
            diffuser => pullGasGrid(pod, diffuser));
    }
    void pullGasGrid(Pod pod, GasDiffuser diffuser)
    {
        HexagonGrid<float> grid = getGasGrid(diffuser);
        float pressure;
        //Valid place for gas to be
        if (pod.podType == Managers.Constants.spacePodType)
        {
            pressure = 0;
        }
        //Gas emitter
        else if (pod.podType == diffuser.emitterPodType)
        {
            pressure = diffuser.emitterPressure;
        }
        //Invalid place for gas to be
        else
        {
            pressure = -1;
        }
        grid[pod.gridPos] = pressure;
    }
    void pushGasGrid(GasDiffuser diffuser)
    {
        HexagonGrid<float> grid = getGasGrid(diffuser);
        grid.Keys
            .FindAll(v => grid[v] >= 0)
            .ForEach(
            v =>
            {
                Pod pod = Managers.Planet.Planet.getPod(v);
                if (!pod && grid[v] > 0)
                {
                    pod = Managers.Planet.Planet.fillSpace(v);
                }
                if (pod)
                {
                    PodContent content = pod.getContent(diffuser.gasPodContentType);
                    if (!content && grid[v] > 0)
                    {
                        content = new PodContent(diffuser.gasPodContentType, pod);
                        pod.addContent(content);
                    }
                    if (content)
                    {
                        content.Var = grid[v];
                    }
                }
            }
            );
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
