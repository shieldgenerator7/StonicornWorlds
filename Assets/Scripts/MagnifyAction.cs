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
        if (origPosList.Count == 1)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos = Managers.Planet.Planet.getHexPos(mousePos);
            List<Stonicorn> stonicorns = Managers.Planet.Planet.residents.FindAll(
                stncrn => Vector2.Distance(stncrn.position, mousePos) < 1.0f
                );
            if (stonicorns.Count == 0)
            {
                stonicorns = Managers.Planet.Planet.residents.FindAll(
                    stncrn => Managers.Planet.Planet.getHexPos(stncrn.homePosition) == pos
                    );
            }
            stonicorns.RemoveAll(stncrn => stncrn == null);
            if (Managers.Camera.FocusObject)
            {
                stonicorns.Remove(Managers.Camera.FocusObject.stonicorn);
            }
            if (stonicorns.Count > 0)
            {
                Stonicorn stonicorn = stonicorns.OrderBy(
                        stncrn => Vector2.Distance(stncrn.position, mousePos)
                    ).ToList()[0];
                Managers.Camera.FocusObject = Managers.PlanetEffects
                    .stonicorns[stonicorn].GetComponent<StonicornDisplayer>();
            }
            else
            {
                Managers.Camera.FocusObject = null;
            }
        }
    }
}
