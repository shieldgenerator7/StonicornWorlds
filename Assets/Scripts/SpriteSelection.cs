using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSelection : MonoBehaviour
{
    public List<Sprite> sprites;

    public Image img;

    public void selectSprite(int index)
    {
        img.sprite = sprites[index];
    }
}
