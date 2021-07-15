using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHUD : MonoBehaviour
{
    private ToolButton button;
    public TMP_Text buttonName;
    public TMP_Text buttonDesc;
    public Image imgIcon;
    public Image imgOutline;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void trackStonicorn(ToolButton button)
    {
        this.button = button;
        bool turnOn = button != null;
        gameObject.SetActive(turnOn);
        if (turnOn)
        {
            //Name
            buttonName.text = button.buttonName;
            //Description
            buttonDesc.text = button.buttonDescription.Replace("\\n", "\n");
            //Icon
            imgIcon.sprite = button.spriteSmall;
            imgIcon.color = (button.spriteSmall != null) ? button.Color : Color.white;
            imgIcon.enabled = button.spriteSmall != null;
            //Outline
            imgOutline.sprite = button.sprite;
            imgOutline.color = (button.spriteSmall != null) ? button.Color : Color.white;
            imgOutline.enabled = button.sprite != null;
        }
    }
}
