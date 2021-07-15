using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuGestureProfile : GestureProfile
{
    /// <summary>
    /// Called when this profile is set to the current one
    /// </summary>
    public override void activate() { }

    /// <summary>
    /// Called when the GestureManager switches off this profile to a different one
    /// </summary>
    public override void deactivate() { }

    public override void processHoverGesture(Vector2 curMPWorld)
    {
        List<ToolButton> mobs = Managers.Input.buttons
               .FindAll(btn => btn.checkMouseOver(Input.mousePosition));
        if (mobs.Count == 0)
        {
            Managers.Gesture.switchGestureProfile(GestureManager.GestureProfileType.PLANET);
        }
    }
    public override void processDragGesture(Vector3 origMPWorld, Vector3 newMPWorld, GestureInput.DragType dragType, GesturePhase phase)
    {
        if (phase == GesturePhase.STARTED)
        {
            ToolButton clickedButton = Managers.Input.buttons
                .FindAll(b => b.gameObject.activeSelf)
                .FirstOrDefault(b => b.checkClick(Input.mousePosition));
            if (clickedButton)
            {
                //Click on Button
                clickedButton.activate();
                Managers.Input.toolBoxes.ForEach(tb => tb.updateColor());
                Managers.PlanetEffects.updateButtonInfo(clickedButton);
            }
        }
    }
}
