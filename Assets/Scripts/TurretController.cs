using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretController : PlanetProcessor
{
    public float damage = 10;
    public float range = 10;
    public PodContentType turretType;
    List<PodContent> turrets = new List<PodContent>();
    List<AsteroidController> targets;

    private void Start()
    {
        Managers.Planet.onPlanetStateChanged += updateCount;
        updateCount(Managers.Planet.Planet);
    }

    void updateCount(Planet planet)
    {
        turrets = planet.Pods(Managers.Constants.spacePodType)
            .FindAll(pod => pod.hasContent(turretType))
            .ConvertAll(pod => pod.getContent(turretType));
        enabled = turrets.Count > 0;
    }

    public override void update(float timeDelta)
    {
        if (turrets.Count == 0)
        {
            return;
        }
        targets = FindObjectsOfType<AsteroidController>().ToList();
        if (targets.Count > 0)
        {
            turrets.ForEach(turret => operate(turret, timeDelta));
        }
        else
        {
            turrets.ForEach(turret => turret.target = Vector2.zero);
        }
    }

    void operate(PodContent turret, float timeDelta)
    {
        AsteroidController target = targets
            .OrderBy(target => Vector2.Distance(target.transform.position, turret.container.worldPos))
            .ToList()[0];
        if (Vector2.Distance(target.transform.position, turret.container.worldPos) <= range)
        {
            turret.target = target.transform.position;
            target.damage(damage * timeDelta);
        }
        else
        {
            turret.target = Vector2.zero;
        }
    }
}
