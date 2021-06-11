using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretDisplayer : PodContentDisplayer
{
    public GameObject beam;
    public GameObject muzzle;
    PodContent turret;

    public override void setup(PodContent content)
    {
        turret = content;
        beam.transform.position = muzzle.transform.position;
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        if (turret.target != Vector2.zero)
        {
            //Effects
            beam.transform.up = (turret.target - (Vector2)muzzle.transform.position);
            Vector3 scale = beam.transform.localScale;
            scale.y = (turret.target - (Vector2)muzzle.transform.position).magnitude;
            beam.transform.localScale = scale;
            beam.SetActive(true);
        }
        else
        {
            beam.SetActive(false);
        }
    }
}
