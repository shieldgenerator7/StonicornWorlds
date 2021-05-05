using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodContent
{
    public PodContentType contentType;
    public Pod container;

    public PodContent(PodContentType contentType, Pod container)
    {
        this.contentType = contentType;
        this.container = container;
        this.container.podContents.Add(this);
    }
}
