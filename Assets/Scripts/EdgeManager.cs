using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdgeManager : MonoBehaviour
{
    private List<Vector2> validPosList;
    public List<Vector2> ValidPosList => validPosList.ToList();

    {

    public void calculateValidPosList()
    {
        if (Managers.Planet.FuturePlanet != null)
        {
            validPosList = Managers.Planet.FuturePlanet.PodsAll
                .ConvertAll(pod => pod.pos)
                .FindAll(pos => Managers.Input.ToolAction.isActionValidAt(pos));
            onValidPositionListChanged?.Invoke(ValidPosList);
        }
    }
    public delegate void OnValidPositionListChanged(List<Vector2> posList);
    public event OnValidPositionListChanged onValidPositionListChanged;
}
