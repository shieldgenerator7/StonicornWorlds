using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GesturePhase
{
    STARTED,
    ONGOING,
    ENDED
}

public static class GesturePhaseUtility
{
    public static GesturePhase ToGesturePhase(this TouchPhase tp)
    {
        switch (tp)
        {
            case TouchPhase.Began: return GesturePhase.STARTED;
            case TouchPhase.Ended: return GesturePhase.ENDED;
            case TouchPhase.Canceled: return GesturePhase.ENDED;
            default: return GesturePhase.ONGOING;
        }
    }

    public static GesturePhase ToGesturePhase(Touch[] touches)
        => ToGesturePhase(
            touches.ToList()
            .ConvertAll(touch => touch.phase.ToGesturePhase())
            .ToArray()
            );

    public static GesturePhase ToGesturePhase(int mouseButton)
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            return GesturePhase.STARTED;
        }
        else if (Input.GetMouseButtonUp(mouseButton))
        {
            return GesturePhase.ONGOING;
        }
        else
        {
            return GesturePhase.ONGOING;
        }
    }

    public static GesturePhase ToGesturePhase(params int[] mouseButtons)
        => ToGesturePhase(
            mouseButtons.ToList()
            .ConvertAll(mb => ToGesturePhase(mb))
            .ToArray()
            );

    public static GesturePhase ToGesturePhase(params GesturePhase[] gps)
    {
        if (gps.Any(gp => gp == GesturePhase.STARTED))
        {
            return GesturePhase.STARTED;
        }
        else if (gps.Any(gp => gp == GesturePhase.ENDED))
        {
            return GesturePhase.ENDED;
        }
        else
        {
            return GesturePhase.ONGOING;
        }
    }
}
