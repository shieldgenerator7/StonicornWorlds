using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Requirement : MonoBehaviour
{
    public abstract bool isRequirementMet();
}
