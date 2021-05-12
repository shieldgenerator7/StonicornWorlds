using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDisplayer : MonoBehaviour
{
    public PodType requiredGroundType;

    public SpriteRenderer srLeft;
    public SpriteRenderer srMiddle;
    public SpriteRenderer srRight;
    public SpriteRenderer srCLeft;
    public SpriteRenderer srCMiddle;
    public SpriteRenderer srCRight;

    PlanetManager planetManager;
    Neighborhood<Pod> neighborhood;

    // Start is called before the first frame update
    void Start()
    {
        planetManager = FindObjectOfType<PlanetManager>();
        planetManager.Planet.onStateChanged += updateNeighborhood;
        updateNeighborhood(Managers.Planet.Planet);
    }
    private void OnDestroy()
    {
        if (planetManager)
        {
            planetManager.Planet.onStateChanged -= updateNeighborhood;
        }
    }

    void updateNeighborhood(Planet p)
    {
        neighborhood = p.getNeighborhood(transform.position);
        srLeft.enabled = validGround(neighborhood.groundLeft);
        srMiddle.enabled = validGround(neighborhood.ground);
        srRight.enabled = validGround(neighborhood.groundRight);
        srCLeft.enabled = validGround(neighborhood.ceilingLeft);
        srCMiddle.enabled = validGround(neighborhood.ceiling);
        srCRight.enabled = validGround(neighborhood.ceilingRight);
    }

    bool validGround(Pod pod)
    {
        return pod && pod.podType == requiredGroundType;
    }
}
