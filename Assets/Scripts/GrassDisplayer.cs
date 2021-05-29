using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDisplayer : PodContentDisplayer
{
    public PodType requiredGroundType;

    public SpriteRenderer srLeft;
    public SpriteRenderer srMiddle;
    public SpriteRenderer srRight;
    public SpriteRenderer srCLeft;
    public SpriteRenderer srCMiddle;
    public SpriteRenderer srCRight;

    public override void setup(PodContent content)
    {
        Managers.Planet.onPlanetStateChanged += updateNeighborhood;
        updateNeighborhood(Managers.Planet.Planet);
    }
    private void OnDestroy()
    {
        Managers.Planet.onPlanetStateChanged -= updateNeighborhood;
    }

    void updateNeighborhood(Planet p)
    {
        Neighborhood<Pod> neighborhood = p.getNeighborhood(transform.position);
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
