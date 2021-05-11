using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PodContent
{
    [System.NonSerialized]
    public PodContentType contentType;
    public string contentTypeName;
    [System.NonSerialized]
    public Pod container;
    private Dictionary<string, float> variables = new Dictionary<string, float>();

    public PodContent(PodContentType contentType, Pod container)
    {
        this.contentType = contentType;
        this.contentTypeName = contentType.name;
        this.container = container;
        this.container.podContents.Add(this);
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
        set => setVar("var", value);
    }

    public static implicit operator bool(PodContent pc)
        => pc != null;
}
