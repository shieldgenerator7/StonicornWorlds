using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexagonNeighborhood
{
    public Vector3Int ceilingLeft;
    public Vector3Int ceiling;
    public Vector3Int ceilingRight;
    public Vector3Int groundLeft;
    public Vector3Int ground;
    public Vector3Int groundRight;
    public Vector3Int[] neighbors;
    public Vector3Int[] upsides;
    public Vector3Int[] downsides;
}
