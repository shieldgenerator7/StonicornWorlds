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
            buttonName.text = button.name;
            buttonDesc.text = button.name + " description goes here.";
            imgIcon.color = button.Color;
            imgOutline.color = button.Color;
        }
    }
}
