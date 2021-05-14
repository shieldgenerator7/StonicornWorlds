using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdgeManager : MonoBehaviour
{
    public List<Vector2> validPosList;

    // Start is called before the first frame update
    void Awake()
    {
        Managers.Planet.onFuturePlanetStateChanged += (fp) => calculateValidPosList();
        Managers.Queue.onQueueChanged += (tasks) => calculateValidPosList();
        Managers.Input.onPlanetObjectTypeChanged += (pot) => calculateValidPosList();
        Managers.Input.onToolActionChanged += (ta) => calculateValidPosList();
        if (Managers.Planet.FuturePlanet != null)
        {
            calculateValidPosList();
        }
    }

    private void Start()
    {
        if (Managers.Planet.FuturePlanet != null)
        {
            calculateValidPosList();
        }
    }

    int i = 0;
    private void Update()
    {
        if (i < 5)
        {
            i++;
            Start();
        }
    }

    private void calculateValidPosList()
    {
        validPosList = Managers.Planet.FuturePlanet.PodsAll
            .ConvertAll(pod => pod.pos)
            .FindAll(pos => Managers.Input.ToolAction.isActionValidAt(pos));
        onValidPositionListChanged?.Invoke(validPosList);
    }
    public delegate void OnValidPositionListChanged(List<Vector2> posList);
    public event OnValidPositionListChanged onValidPositionListChanged;
}
