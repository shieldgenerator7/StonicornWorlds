using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGestureProfile : GestureProfile
{
    public override void processHoverGesture(Vector2 curMPWorld)
    {
        List<ToolButton> mobs = Managers.Input.buttons
            .FindAll(btn => btn.checkMouseOver(Input.mousePosition));
        if (mobs.Count > 0)
        {
            Managers.Gesture.switchGestureProfile(GestureManager.GestureProfileType.MENU);
        }
        else
        {
            Managers.Input.tool.inputIdle(curMPWorld);
        }
    }
    public override void processDragGesture(Vector3 origMPWorld, Vector3 newMPWorld, GestureInput.DragType dragType, GesturePhase phase)
    {
        if (dragType == GestureInput.DragType.DRAG_ACTION)
        {
            switch (phase)
            {
                //Click in world with Tool
                case GesturePhase.STARTED:
                    Managers.Input.tool.inputDown(origMPWorld);
                    break;
                case GesturePhase.ONGOING:
                    Managers.Input.tool.inputMove(newMPWorld);
                    break;
                case GesturePhase.ENDED:
                    Managers.Input.tool.inputUp(newMPWorld);
                    break;
                default:
                    throw new System.NotSupportedException("Enum value not supported!: " + phase);
            }
        }
        else if (dragType == GestureInput.DragType.DRAG_CAMERA)
        {
            //Drag the camera
            //Managers.Camera.processDragGesture(origMPWorld, newMPWorld, finished);
        }
        else
        {
            throw new System.ArgumentException("DragType must be a valid value! dragType: " + dragType);
        }
    }
}
