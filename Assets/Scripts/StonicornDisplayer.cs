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
    private SpriteRenderer ui_work;

    // Start is called before the first frame update
    void Start()
    {
        bodySR.color = stonicorn.bodyColor;
        hairSR.color = stonicorn.hairColor;
        ui_work.color = stonicorn.hairColor;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = stonicorn.position;
        transform.up = Managers.Planet.Planet.getUpDirection(stonicorn.position);
        if (stonicorn.isAtLocationOfInterest && !stonicorn.atHomeOrGoing)
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
    }
}
