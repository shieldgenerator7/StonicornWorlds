using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager : MonoBehaviour
{
    public abstract void setup();

    public virtual void update(float timeDelta) { }
}
