using UnityEngine;
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

    public virtual void processHoverGesture(Vector2 curMPWorld) { }

    public void processTapGesture(Vector3 curMPWorld)
    {
        throw new System.NotSupportedException("This function isn't supported: everything is now a drag gesture");
    }
    public void processHoldGesture(Vector3 curMPWorld, float holdTime, GesturePhase phase)
    {
        throw new System.NotSupportedException("This function isn't supported: everything is now a drag gesture");
    }
    public virtual void processDragGesture(Vector3 origMPWorld, Vector3 newMPWorld, GestureInput.DragType dragType, GesturePhase phase) { }
    public void processZoomLevelChange(float zoomLevel) { }
}
