using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidEvent : PlanetProcessor
{
    public float startDelay = 300;//sec before it can start doing events
    public List<float> cooldownDelays;
    public GameObject asteroidPrefab;

    [SerializeField]
    private float timeLeft = 100;
    private float delay = 100;
    private float lastEventTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        delay = startDelay;
        lastEventTime = Time.time;
        timeLeft = startDelay;
    }

    // Update is called once per frame
    public override void update(float timeDelta)
    {
        if (timeLeft <= 0)
        //if (Time.time > lastEventTime + delay)
        {
            scheduleNextEvent();
            processEvent();
        }
        timeLeft = lastEventTime + delay - Time.time;
    }

    void scheduleNextEvent()
    {
        lastEventTime = Time.time;
        delay = cooldownDelays[Random.Range(0, cooldownDelays.Count)];
    }

    void processEvent()
    {
        GameObject asteroid = Instantiate(asteroidPrefab);
        asteroid.transform.parent = transform;
        asteroid.transform.eulerAngles = new Vector3(0, 0, Random.Range(0.0f, 360.0f));
        List<Pod> pods = Managers.Planet.Planet.PodsAll;
        Pod pod = pods[Random.Range(0, pods.Count)];
        float size = Managers.Planet.Planet.size
            * HexagonUtility.maxRing(pods.ConvertAll(p => p.gridPos));
        float distance = size * 3;
        asteroid.transform.position = pod.worldPos + (-(Vector2)asteroid.transform.up * distance);
        asteroid.GetComponent<AsteroidController>().exitRadius = distance * 1.5f;
    }

}
