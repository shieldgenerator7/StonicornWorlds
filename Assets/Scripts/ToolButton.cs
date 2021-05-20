using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ToolButton : MonoBehaviour
{
    public Image image;
    public Image activeImage;
    public Image mouseOverImage;
    public float checkRadius = 50;
    Vector2 position;
    private bool activeButton = false;

    // Start is called before the first frame update
    void Start()
    {
        updatePosition();
    }

    public void setPosition(float x, float y)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.position = new Vector2(x, y);
        updatePosition();
    }

    public void updatePosition()
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
        return !activeButton && Vector2.Distance(screenPos, position) <= checkRadius;
    }

    public void checkActive()
    {
        activeButton = isActive();
        activeImage.enabled = activeButton;
    }

    public abstract void activate();

    protected abstract bool isActive();
}
