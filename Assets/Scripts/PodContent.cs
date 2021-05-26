using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PodContent : PlanetObject
{
    public PodContentType contentType => objectType as PodContentType;

    [System.NonSerialized]
    public Pod container;

    public PodContent(PodContentType contentType, Pod container) : base(contentType)
    {
        this.container = container;
        this.container.addContent(this);
        this.Var = this.contentType.initialVarValue;
    }


    /// <summary>
    /// Used when there's only one variable
    /// </summary>
    [SerializeField]
    private float variable;
    public float Var
    {
        get => variable;
        set
        {
            variable = value;
            onVarChanged?.Invoke(variable);
        }
    }
    public delegate void OnVarChanged(float val);
    public event OnVarChanged onVarChanged;

    public override string ToString()
    {
        return "" + typeName + " PodContent";
    }
}
