using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pod
{
    public Vector2 pos;
    [System.NonSerialized]
    public PodType podType;
    public string podTypeName;

    private List<PodContent> podContents = new List<PodContent>();

    public Pod(Vector2 pos, PodType podType)
    {
        this.pos = pos;
        this.podType = podType;
        this.podTypeName = podType.name;
    }

    public void addContent(PodContent content)
    {
        if (!podContents.Contains(content))
        {
            podContents.Add(content);
            onPodContentChanged?.Invoke(this);
        }
    }
    public delegate void OnPodContentChanged(Pod p);
    public event OnPodContentChanged onPodContentChanged;

    public void removeContent(PodContent content)
    {
        podContents.Remove(content);
        onPodContentChanged?.Invoke(this);
    }

    public bool hasContent(PodContentType contentType)
        => podContents.Any(content => content.contentType == contentType);

    public bool hasContentAny(List<PodContentType> contentTypes)
        => contentTypes.Any(ct => hasContent(ct));

    public bool hasContentAll(List<PodContentType> contentTypes)
        => contentTypes.TrueForAll(ct => hasContent(ct));

    public PodContent getContent(PodContentType contentType)
        => podContents.FirstOrDefault(content => content.contentType == contentType);

    public void forEachContent(Action<PodContent> contentFunc)
    {
        podContents.ForEach(content => contentFunc(content));
    }

    public Pod Clone()
    {
        Pod clone = new Pod(this.pos, this.podType);
        clone.podContents = podContents.ConvertAll(content => content.Clone(clone));
        return clone;
    }

    public override string ToString()
    {
        return "" + podType.name + " Pod";
    }

    public static implicit operator bool(Pod p)
        => p != null;
}
