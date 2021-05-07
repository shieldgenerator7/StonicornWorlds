using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexagonUtility
{
    public static Vector3Int getGroundPos(Vector3Int pos)
    {
        Vector3Int groundPos = reduceAbs(pos);
        if (pos.x == 0)
        {
            groundPos.x = pos.x;
        }
        else if (pos.y == 0)
        {
            groundPos.y = pos.y;
        }
        else if (pos.z == 0)
        {
            groundPos.z = pos.z;
        }
        else
        {
            if (Math.Sign(pos.x) == Mathf.Sign(pos.y))
            {
                groundPos.y = pos.y;
            }
            else
            {
                groundPos.z = pos.z;
            }
        }
        return groundPos;
    }

    public static HexagonNeighborhood getNeighborhood(Vector3Int pos)
    {
        HexagonNeighborhood neighborhood = new HexagonNeighborhood();
        Vector3Int groundPos = getGroundPos(pos);
        neighborhood.ground = groundPos;
        neighborhood.groundLeft = rotate(groundPos, pos, 1);
        neighborhood.groundRight = rotate(groundPos, pos, -1);
        Vector3Int ceilPos = reflect(groundPos, pos);
        neighborhood.ceiling = ceilPos;
        neighborhood.ceilingLeft = rotate(ceilPos, pos, -1);
        neighborhood.ceilingRight = rotate(ceilPos, pos, 1);
        neighborhood.neighbors = new Vector3Int[]
        {
            neighborhood.ground,
            neighborhood.groundLeft,
            neighborhood.groundRight,
            neighborhood.ceiling,
            neighborhood.ceilingLeft,
            neighborhood.ceilingRight,
        };
        return neighborhood;
    }

    private static Vector3Int reduceAbs(Vector3Int v)
    {
        v.x = reduceAbs(v.x);
        v.y = reduceAbs(v.y);
        v.z = reduceAbs(v.z);
        return v;
    }

    private static int reduceAbs(int n)
        => (n == 0) ? 0 : n - Math.Sign(n);

    public static Vector3Int rotate(Vector3Int pos, Vector3Int center, int angle)
    {
        pos -= center;
        //rotate clockwise
        while (angle > 0)
        {
            pos = new Vector3Int(-pos.z, -pos.x, -pos.y);
        }
        //rotate counter-clockwise
        while (angle < 0)
        {
            pos = new Vector3Int(-pos.y, -pos.z, -pos.x);
        }
        pos += center;
        return pos;
    }
    public static Vector3Int reflect(Vector3Int pos, Vector3Int center)
    {
        pos -= center;
        pos *= -1;
        pos += center;
        return pos;
    }
}
