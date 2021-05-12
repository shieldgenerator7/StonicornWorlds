using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tool : MonoBehaviour
{
    public Image image;
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

    public bool checkClick(Vector2 screenPos)
    {
        return Vector2.Distance(screenPos, position) <= 50;
    }

    public abstract void activate();

    public abstract void inputDown(Vector2 pos);

    public abstract void inputMove(Vector2 pos);

    public abstract void inputUp(Vector2 pos);
}
