using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activity
{
    protected readonly Stonicorn stonicorn;

    public Activity(Stonicorn stonicorn)
    {
        this.stonicorn = stonicorn;
    }

    public abstract Stonicorn.Action action { get; }

    public abstract bool canStart { get; }

    public abstract bool canContinue { get; }

    public abstract void doActivity();

    public abstract bool isDone { get; }

    public void goToActivity()
    {
        stonicorn.locationOfInterest = chooseActivityLocation();
    }

    protected abstract Vector2 chooseActivityLocation();

    public abstract float ActivityRange { get; }

    public bool isInRange =>
        Vector2.Distance(
            stonicorn.locationOfInterest,
            stonicorn.position
            )
        <= ActivityRange;

    public static implicit operator bool(Activity act)
        => act != null;
}
