using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pod : PlanetObject
{
    public PodType podType => objectType as PodType;

    [NonSerialized]
    public Vector2 worldPos;
    public Vector3Int gridPos;

    [SerializeField]
    private List<PodContent> podContents = new List<PodContent>();

    public Pod(Vector2 pos, PodType podType) : base(podType)
    {
        this.worldPos = pos;
        podType.includedContentTypes.ForEach(
            contentType =>
            {
                PodContent content = new PodContent(contentType, this);
                addContent(content);
            }
            );
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
    public void removeContent(PodContentType contentType)
    {
        podContents.RemoveAll(content => content.contentType == contentType);
        onPodContentChanged?.Invoke(this);
    }

    public bool hasContent(PodContentType contentType)
        => podContents.Any(content => content.contentType == contentType);
    public bool hasContent(PodContent content)
        => podContents.Contains(content);

    public bool hasContentSolid()
        => hasContentAny(Managers.Constants.allSolidPodContentTypes);

    public bool hasContentAny(List<PodContentType> contentTypes)
        => contentTypes.Any(ct => hasContent(ct));

    public bool hasContentAll(List<PodContentType> contentTypes)
        => contentTypes.TrueForAll(ct => hasContent(ct));

    public PodContent getContent(PodContentType contentType)
        => podContents.FirstOrDefault(content => content.contentType == contentType);

    public List<PodContentType> getContentTypes()
        => podContents.ConvertAll(content => content.contentType);

    public void forEachContent(Action<PodContent> contentFunc)
    {
        podContents.ForEach(content => contentFunc(content));
    }

    public override void inflate()
    {
        base.inflate();
        if (podContents == null)
        {
            podContents = new List<PodContent>();
        }
        podContents.ForEach(pc =>
        {
            pc.inflate();
            pc.container = this;
        });
    }

    public override string ToString()
    {
        return "" + typeName + " Pod";
    }
}
