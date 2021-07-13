﻿using UnityEngine;
using System.Collections;

public class GestureProfile
{
    /// <summary>
    /// Called when this profile is set to the current one
    /// </summary>
    public virtual void activate() { }

    /// <summary>
    /// Called when the GestureManager switches off this profile to a different one
    /// </summary>
    public virtual void deactivate() { }

    public virtual void processHoverGesture(Vector2 curMPWorld) { }

    public virtual void processTapGesture(Vector3 curMPWorld)
    {
        //Managers.Player.processTapGesture(curMPWorld);
    }
    public virtual void processHoldGesture(Vector3 curMPWorld, float holdTime, bool finished)
    {
        //Managers.Player.processHoldGesture(curMPWorld, holdTime, finished);
    }
    public virtual void processDragGesture(Vector3 origMPWorld, Vector3 newMPWorld, GestureInput.DragType dragType, bool finished)
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
    public void processZoomLevelChange(float zoomLevel)
    {
    }
}