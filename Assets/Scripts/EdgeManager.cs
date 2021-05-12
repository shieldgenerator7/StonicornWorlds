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
        Managers.Planet.planet.onStateChanged += (pods) => calculateValidPosList();
        Managers.Queue.onQueueChanged += (tasks) => calculateValidPosList();
        Managers.Input.onPlanetObjectTypeChanged += (pot) => calculateValidPosList();
        Managers.Input.onToolActionChanged += (ta) => calculateValidPosList();
        calculateValidPosList();
    }

    private void calculateValidPosList()
    {
        validPosList = Managers.Planet.futurePlanet.PodsAll
            .ConvertAll(pod => pod.pos)
            .FindAll(pos => Managers.Input.ToolAction.isActionValidAt(pos));
        onValidPositionListChanged?.Invoke(validPosList);
    }
    public delegate void OnValidPositionListChanged(List<Vector2> posList);
    public event OnValidPositionListChanged onValidPositionListChanged;
}
