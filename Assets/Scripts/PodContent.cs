using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PodContent : PlanetObject
{
    public PodContentType contentType => objectType as PodContentType;

    [System.NonSerialized]
    public Pod container;
    private Dictionary<string, float> variables = new Dictionary<string, float>();

    public PodContent(PodContentType contentType, Pod container) : base(contentType)
    {
        this.container = container;
        this.container.addContent(this);
        this.Var = this.contentType.initialVarValue;
    }

    public PodContent Clone(Pod clonePod)
    {
        PodContent clone = new PodContent(this.contentType, clonePod);
        clone.variables = new Dictionary<string, float>(this.variables);
        return clone;
    }

    public float getVar(string varName)
    {
        if (!variables.ContainsKey(varName))
        {
            variables[varName] = 0;
        }
        return variables[varName];
    }
    public void setVar(string varName, float val)
    {
        variables[varName] = val;
    }

    /// <summary>
    /// Used when there's only one variable
    /// </summary>
    public float Var
    {
        get => getVar("var");
        set
        {
            setVar("var", value);
            onVarChanged?.Invoke(value);
        }
    }
    public delegate void OnVarChanged(float val);
    public event OnVarChanged onVarChanged;

    public static implicit operator bool(PodContent pc)
        => pc != null;
}
