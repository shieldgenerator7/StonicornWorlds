using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagnifyAction : ToolAction
{
    public override Color color => Color.white;

    public override bool isActionValidAt(Vector2 pos)
        => Managers.Planet.Planet.hasPod(pos);

    public override void takeAction(List<Vector2> posList)
    {
        List<Vector2> origPosList = posList.ToList();
        //If any of the positions are on the planet
        if (posList.Any(v => isActionValidAt(v)))
        {
            Vector2 center = posList.Aggregate((sum, v) => sum + v) / posList.Count;
            Managers.Camera.Locked = true;
            Managers.Camera.autoFrame(center, posList);
        }
        else
        {
            Managers.Camera.Locked = false;
            Managers.Camera.autoFrame(posList);
        }
        //Track Stonicorn
        Managers.Camera.focusObject = null;
        if (origPosList.Count == 1)
        {
            Vector2 pos = Managers.Planet.Planet.getHexPos(origPosList[0]);
            Stonicorn stonicorn = Managers.Planet.Planet.residents.FirstOrDefault(
                stncrn => Managers.Planet.Planet.getHexPos(stncrn.position) == pos
                );
            if (stonicorn == null)
            {
                stonicorn = Managers.Planet.Planet.residents.FirstOrDefault(
                    stncrn => Managers.Planet.Planet.getHexPos(stncrn.homePosition) == pos
                    );
            }
            if (stonicorn != null)
            {
                Managers.Camera.focusObject = Managers.PlanetEffects.stonicorns[stonicorn]
                    .GetComponent<StonicornDisplayer>();
                Managers.Camera.Locked = true;
                Managers.Camera.autoFrame(stonicorn.position, origPosList);
            }
        }
    }
}
