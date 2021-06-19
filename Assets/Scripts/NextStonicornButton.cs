using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStonicornButton : ToolButton
{
    protected override void Start()
    {
        base.Start();
        canClickWhenActive = true;
    }

    protected override void activateImpl()
    {
        int index = -1;
        if (Managers.Camera.FocusObject != null)
        {
            index = Managers.Planet.Planet.residents.IndexOf(Managers.Camera.FocusObject.stonicorn);
        }
        index++;
        if (index < Managers.Planet.Planet.residents.Count)
        {
            Managers.Camera.FocusObject = Managers.PlanetEffects
                .stonicorns[Managers.Planet.Planet.residents[index]]
                .GetComponent<StonicornDisplayer>();
        }
        else
        {
            Managers.Camera.FocusObject = null;
        }
    }

    protected override bool isActiveImpl()
        => Managers.Camera.FocusObject != null;
}
