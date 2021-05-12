using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SingleEditTool : Tool
{
    public override void inputDown(Vector2 pos)
    {
        Vector2 clickedUI = Managers.Edge.edges.FirstOrDefault(
            edge => Managers.Planet.checkAddPodUI(pos, edge)
            );
        if (clickedUI != Vector2.zero)
        {
            Managers.Input.toolAction.takeAction(new List<Vector2>() { clickedUI });            
        }
    }

    public override void inputMove(Vector2 pos)
    {
    }

    public override void inputUp(Vector2 pos)
    {
    }
}
