using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDisplayer : MonoBehaviour
{
    public PodContentType contentType;

    private PodContent content;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        if (!content)
        {
            content = Managers.Planet.Planet
                .getPod(transform.position)
                .getContent(contentType);
        }
        if (content)
        {
            Color color = sr.color;
            color.a = content.Var / 100;
            sr.color = color;
        }
    }
}
