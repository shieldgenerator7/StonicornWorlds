using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StonicornHUD : MonoBehaviour
{
    public Stonicorn stonicorn;

    public TMP_Text stonicornName;
    public Image imgBody;
    public Image imgHair;
    public Image imgEye;
    public Image sleepMeter;
    public Image resourceMeter;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        sleepMeter.fillAmount = stonicorn.rest / stonicorn.maxRest;
        resourceMeter.fillAmount = stonicorn.toolbeltResources / stonicorn.toolbeltSize;
    }

    public void trackStonicorn(Stonicorn stonicorn)
    {
        this.stonicorn = stonicorn;
        bool turnOn = stonicorn != null;
        gameObject.SetActive(turnOn);
        if (turnOn)
        {
            stonicornName.text = stonicorn.name;
            imgBody.color = stonicorn.bodyColor;
            imgHair.color = stonicorn.hairColor;
            imgEye.color = stonicorn.eyeColor;
            Update();
        }
    }
}
