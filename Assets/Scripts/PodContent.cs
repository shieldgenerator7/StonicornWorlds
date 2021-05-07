using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodContent
{
    [System.NonSerialized]
    public PodContentType contentType;
    public string contentTypeName;
    [System.NonSerialized]
    public Pod container;

    public PodContent(PodContentType contentType, Pod container)
    {
        this.contentType = contentType;
        this.contentTypeName = contentType.name;
        this.container = container;
        this.container.podContents.Add(this);
    }

    public void inflate(Pod container)
    {
        this.container = container;
        this.contentType = Resources.Load<PodContentType>("PodContentTypes/" + contentTypeName);
    }
}
