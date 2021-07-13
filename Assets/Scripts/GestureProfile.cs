﻿using UnityEngine;
using System.Collections;
using System.Linq;

public abstract class GestureProfile
{
    /// <summary>
    /// Called when this profile is set to the current one
    /// </summary>
    public virtual void activate() { }

    /// <summary>
    /// Called when the GestureManager switches off this profile to a different one
    /// </summary>
    public virtual void deactivate() { }

    public virtual void processHoverGesture(Vector2 curMPWorld) {
        //Check click on Tool
        ToolButton clickedButton = Managers.Input.buttons
            .FindAll(b => b.gameObject.activeSelf)
            .FirstOrDefault(b => b.checkClick(Input.mousePosition));
        if (clickedButton)
        {
            //Click on Button
            clickedButton.activate();
            Managers.Input.buttonActivation = true;
            Managers.Input.toolBoxes.ForEach(tb => tb.updateColor());
        }
        else
        {
            //Click in world with Tool
            Managers.Input.tool.inputDown(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Managers.Input.buttonActivation = false;
        }
    }

    public void processTapGesture(Vector3 curMPWorld)
    {
        throw new System.NotSupportedException("This function isn't supported: everything is now a drag gesture");
    }
    public void processHoldGesture(Vector3 curMPWorld, float holdTime, GesturePhase phase)
    {
        throw new System.NotSupportedException("This function isn't supported: everything is now a drag gesture");
    }
    public virtual void processDragGesture(Vector3 origMPWorld, Vector3 newMPWorld, GestureInput.DragType dragType, GesturePhase phase)
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
