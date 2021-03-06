using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseGestureInput : GestureInput
{
    public int mouseButton = 0;
    public int mouseButton2 = 1;
    public float dragThreshold = 0;
    public float holdThreshold = 1f;

    private DragType dragType = DragType.UNKNOWN;

    private Vector2 origPosScreen;
    private Vector2 OrigPosWorld
        => Camera.main.ScreenToWorldPoint(origPosScreen);

    private float origTime;

    private enum MouseEvent
    {
        UNKNOWN,
        CLICK,
        DRAG,
        HOLD,
        SCROLL
    }
    private MouseEvent mouseEvent = MouseEvent.UNKNOWN;

    public override bool InputSupported
        => Input.mousePresent;

    public override InputDeviceMethod InputType
        => InputDeviceMethod.MOUSE;

    public override bool InputOngoing
        => Input.GetMouseButton(mouseButton)
            || Input.GetMouseButtonUp(mouseButton)
            || Input.GetMouseButton(mouseButton2)
            || Input.GetMouseButtonUp(mouseButton2)
            || Input.GetAxis("Mouse ScrollWheel") != 0;

    public override bool processInput(GestureProfile profile)
    {
        if (InputOngoing)
        {
            //
            //Check for click start
            //
            if (mouseEvent == MouseEvent.UNKNOWN)
            {
                //Click beginning
                if (Input.GetMouseButtonDown(mouseButton))
                {
                    origPosScreen = Input.mousePosition;
                    origTime = Time.time;
                    dragType = DragType.DRAG_ACTION;
                    mouseEvent = MouseEvent.DRAG;
                }
                else if (Input.GetMouseButtonDown(mouseButton2))
                {
                    origPosScreen = Input.mousePosition;
                    origTime = Time.time;
                    dragType = DragType.DRAG_CAMERA;
                    mouseEvent = MouseEvent.DRAG;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    mouseEvent = MouseEvent.SCROLL;
                }
            }

            //
            //Main Processing
            //

            switch (mouseEvent)
            {
                case MouseEvent.DRAG:
                    profile.processDragGesture(
                        OrigPosWorld,
                        Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        dragType,
                        (dragType == DragType.DRAG_ACTION)
                            ? GesturePhaseUtility.ToGesturePhase(mouseButton)
                            : GesturePhaseUtility.ToGesturePhase(mouseButton2)
                        );
                    break;
                case MouseEvent.HOLD:
                    profile.processHoldGesture(
                        Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        Time.time - origTime,
                        (dragType == DragType.DRAG_ACTION)
                            ? GesturePhaseUtility.ToGesturePhase(mouseButton)
                            : GesturePhaseUtility.ToGesturePhase(mouseButton2)
                        );
                    break;
                case MouseEvent.SCROLL:
                    if (Input.GetAxis("Mouse ScrollWheel") < 0)
                    {
                        Managers.Camera.ZoomLevel *= 1.2f;
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    {
                        Managers.Camera.ZoomLevel /= 1.2f;
                    }
                    break;
            }

            //
            //Check for click end
            //
            if (Input.GetMouseButtonUp(mouseButton))
            {
                mouseEvent = MouseEvent.DRAG;
                profile.processDragGesture(
                      OrigPosWorld,
                      Camera.main.ScreenToWorldPoint(Input.mousePosition),
                      DragType.DRAG_ACTION,
                      GesturePhase.ENDED
                      );
            }
            else if (Input.GetMouseButtonUp(mouseButton2))
            {
                mouseEvent = MouseEvent.DRAG;
                profile.processDragGesture(
                      OrigPosWorld,
                      Camera.main.ScreenToWorldPoint(Input.mousePosition),
                      DragType.DRAG_CAMERA,
                      GesturePhase.ENDED
                      );
            }
            return true;
        }
        //If there's no input,
        else
        {
            //Reset gesture variables
            mouseEvent = MouseEvent.UNKNOWN;
            dragType = DragType.UNKNOWN;
            //Hover gesture
            profile.processHoverGesture(
                Camera.main.ScreenToWorldPoint(Input.mousePosition)
                );
            return false;
        }
    }
}
