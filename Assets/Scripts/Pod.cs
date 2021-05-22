using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pod : PlanetObject
{
    public PodType podType => objectType as PodType;

    [NonSerialized]
    public Vector2 worldPos;
    public Vector3Int gridPos;

    private List<PodContent> podContents = new List<PodContent>();

    public Pod(Vector2 pos, PodType podType) : base(podType)
    {
        this.worldPos = pos;
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
    public bool hasContent(PodContent content)
        => podContents.Contains(content);

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
        Pod clone = new Pod(this.worldPos, this.podType);
        clone.podContents = podContents.ConvertAll(content => content.Clone(clone));
        return clone;
    }

    public override string ToString()
    {
        return "" + podType.name + " Pod";
    }
}
