using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGestureProfile : GestureProfile
{
    public override void processDragGesture(Vector3 origMPWorld, Vector3 newMPWorld, GestureInput.DragType dragType, GesturePhase phase)
    {
        //If the player drags on Merky,
        if (dragType == GestureInput.DragType.DRAG_ACTION)
        {
            //Activate the ForceLaunch ability
            //Managers.Player.processDragGesture(origMPWorld, newMPWorld, finished);
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
