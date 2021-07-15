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
    private bool activeButton = false;
    protected bool canClickWhenActive = false;
    public CompatibilitySet compatibilities;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        updatePosition();
    }

    public virtual Color Color => image.color;
    public Sprite sprite;
    public Sprite spriteSmall;
    public string buttonName;
    public string buttonDescription;

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
        return (!activeButton || canClickWhenActive)
            && Vector2.Distance(screenPos, position) <= Managers.Constants.buttonCheckRadius;
    }

    public void checkActive()
    {
        activeButton = isActiveImpl();
        activeImage.gameObject.SetActive(activeButton);
    }

    public bool Active => activeButton;

    public void activate()
    {
        activateImpl();
        if (!compatibilities.ganzEgal)
        {
            if (!compatibilities.compatibleWithCurrentInput())
            {
                compatibilities.setInputCompatible();
            }
        }
    }

    protected abstract void activateImpl();

    protected bool isActive()
    {
        return isActiveImpl() &&
            (!compatibilities.ganzEgal || compatibilities.compatibleWithCurrentInput());
    }

    protected abstract bool isActiveImpl();
}
