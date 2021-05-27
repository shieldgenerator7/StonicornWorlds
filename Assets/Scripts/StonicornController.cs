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
        if (stonicorn.atHome)
        {
            stonicorn.Rest += stonicorn.restSpeed * Time.deltaTime;
            if (stonicorn.Rest == stonicorn.maxRest)
            {
                stonicorn.resting = false;
            }
        }
        if (!stonicorn.resting && stonicorn.task == null)
        {
            stonicorn.task = Managers.Queue.getClosestTask(stonicorn.position);
            if (stonicorn.task)
            {
                stonicorn.locationOfInterest = stonicorn.task.pos;
            }
        }
        if (stonicorn.position != stonicorn.locationOfInterest)
        {
            moveToLocationOfInterest();
        }
        transform.position = stonicorn.position;
        transform.up = Managers.Planet.Planet.getUpDirection(stonicorn.position);
        bool atWorkSite = !stonicorn.atHomeOrGoing && stonicorn.isAtLocationOfInterest;
        if (atWorkSite && stonicorn.task != null)
        {
            //Work
            bool completed = Managers.Queue.workOnTask(stonicorn.task, stonicorn.workRate);
            if (completed || !stonicorn.task.Started)
            {
                stonicorn.goHome();
                ui_work.gameObject.SetActive(false);
            }
            else
            {
                stonicorn.Rest -= stonicorn.workRate * Time.deltaTime;
                //Effects
                ui_work.transform.up = (stonicorn.locationOfInterest - stonicorn.position);
                Vector3 scale = ui_work.transform.localScale;
                scale.y = (stonicorn.locationOfInterest - stonicorn.position).magnitude;
                ui_work.transform.localScale = scale;
                ui_work.gameObject.SetActive(true);
            }
            if (stonicorn.Rest == 0)
            {
                stonicorn.resting = true;
                stonicorn.goHome();
            }
        }
        else
        {
            ui_work.gameObject.SetActive(false);
        }
    }

    void moveToLocationOfInterest()
    {
        Vector2 aboveLoI = stonicorn.locationOfInterest;
        if (!stonicorn.atHomeOrGoing)
        {
            aboveLoI = Managers.Planet.Planet.getCeilingPos(stonicorn.locationOfInterest);
        }
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
        if (Vector2.Distance(stonicorn.position, aboveLoI) <= 0.01f)
        {
            stonicorn.position = aboveLoI;
        }
    }
}
