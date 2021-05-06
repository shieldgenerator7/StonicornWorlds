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
    PodNeighborhood neighborhood;

    // Start is called before the first frame update
    void Start()
    {
        planetManager = FindObjectOfType<PlanetManager>();
        planetManager.onPodsListChanged += updateNeighborhood;
        updateNeighborhood(null);
    }
    private void OnDestroy()
    {
        if (planetManager)
        {
            planetManager.onPodsListChanged -= updateNeighborhood;
        }
    }

    void updateNeighborhood(List<Pod> pods)
    {
        neighborhood = planetManager.getNeighborhood(transform.position);
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
