using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public float growSpeed = 0.5f;

    //Runtime vars
    private float targetFillAmount;//the fill amount that Image.fillAmount should get to

    //Components
    public List<Image> images = new List<Image>();

    // Start is called before the first frame update
    void Start()
    {
        if (images.Count == 0)
        {
            images.Add(GetComponent<Image>());
            images.AddRange(GetComponentsInChildren<Image>());
        }
        foreach (Image image in images)
        {
            image.fillAmount = 0;
        }
        //Allow app to load in background
        Application.runInBackground = true;
    }

    private void Update()
    {
        targetFillAmount = Managers.Processor.FastForwardPercentDone;
        foreach (Image image in images)
        {
            if (image.fillAmount != targetFillAmount)
            {
                image.fillAmount = Mathf.MoveTowards(image.fillAmount, targetFillAmount, growSpeed * Time.unscaledDeltaTime);
            }
            if (image.fillAmount == 1)
            {
                Destroy(gameObject);
                break;
            }
        }
    }
}
