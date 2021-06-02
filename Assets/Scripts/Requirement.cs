using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Requirement : MonoBehaviour
{
    public bool requireSaveFile = false;

    public abstract bool isRequirementMet();
}
