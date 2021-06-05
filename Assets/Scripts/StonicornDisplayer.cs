using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonicornDisplayer : MonoBehaviour
{
    public Stonicorn stonicorn;
    [SerializeField]
    private SpriteRenderer bodySR;
    [SerializeField]
    private SpriteRenderer hairSR;
    [SerializeField]
    private SpriteRenderer eyeSR;
    [SerializeField]
    private SpriteRenderer ui_work;
    [SerializeField]
    private SpriteRenderer ui_sleep;

    // Start is called before the first frame update
    void Start()
    {
        bodySR.color = stonicorn.bodyColor;
        hairSR.color = stonicorn.hairColor;
        eyeSR.color = stonicorn.eyeColor;
        ui_work.color = stonicorn.eyeColor;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Position
        transform.position = stonicorn.position;
        transform.up = Managers.Planet.Planet.getUpDirection(stonicorn.position);
        //Working
        if (stonicorn.isUsingMagic)
        {
            //Effects
            ui_work.transform.up = (stonicorn.locationOfInterest - stonicorn.position);
            Vector3 scale = ui_work.transform.localScale;
            scale.y = (stonicorn.locationOfInterest - stonicorn.position).magnitude;
            ui_work.transform.localScale = scale;
            ui_work.gameObject.SetActive(true);
        }
        else
        {
            ui_work.gameObject.SetActive(false);
        }
        //Sleeping
        ui_sleep.gameObject.SetActive(
            stonicorn.action == Stonicorn.Action.REST &&
            stonicorn.position == stonicorn.homePosition
            );
    }
}
