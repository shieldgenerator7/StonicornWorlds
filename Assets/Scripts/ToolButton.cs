using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ToolButton : MonoBehaviour
{
    public Image image;
    public Image activeImage;
    public Image mouseOverImage;
    Vector2 position;

    // Start is called before the first frame update
    void Start()
    {
        setPosition();
    }

    public void setPosition()
    {
        RectTransform rect = GetComponent<RectTransform>();
        position = rect.position;
    }

    public bool checkMouseOver(Vector2 screenPos)
    {
        bool mo = checkClick(screenPos);
        mouseOverImage.enabled = mo;
        return mo;
    }

    public bool checkClick(Vector2 screenPos)
    {
        return Vector2.Distance(screenPos, position) <= 50;
    }

    public void checkActive()
    {
        activeImage.enabled = isActive();
    }

    public abstract void activate();

    protected abstract bool isActive();
}
