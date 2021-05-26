using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonicornController : MonoBehaviour
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
    void Update()
    {
        if (stonicorn.position != stonicorn.locationOfInterest)
        {
            moveToLocationOfInterest();
        }
        transform.position = stonicorn.position;
        transform.up = Managers.Planet.Planet.getUpDirection(stonicorn.position);
        ui_work.gameObject.SetActive(stonicorn.isAtLocationOfInterest);
        if (stonicorn.isAtLocationOfInterest)
        {
            ui_work.transform.up = (stonicorn.locationOfInterest - stonicorn.position);
            Vector3 scale = ui_work.transform.localScale;
            scale.y = (stonicorn.locationOfInterest - stonicorn.position).magnitude;
            ui_work.transform.localScale = scale;
        }
    }

    void moveToLocationOfInterest()
    {
        Vector2 aboveLoI = Managers.Planet.Planet.getCeilingPos(stonicorn.locationOfInterest);
        if (Vector2.Distance(stonicorn.position, aboveLoI) > 1.0f)
        {
            stonicorn.position +=
                (aboveLoI - stonicorn.position).normalized
                * stonicorn.moveSpeed * Time.deltaTime;
        }
        else
        {
            stonicorn.position = Vector2.Lerp(
                stonicorn.position,
                aboveLoI,
                Time.deltaTime * stonicorn.moveSpeed
                );
        }
    }
}
