using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static List<Vector3Int> getArea(int maxring)
    {
        List<Vector3Int> area = new List<Vector3Int>();
        for (int q = -maxring; q <= maxring; q++)
        {
            for (int r = -maxring; r <= maxring; r++)
            {
                int s = -(q + r);
                area.Add(new Vector3Int(q, s, r));
            }
        }
        return area;
    }

    public static List<Vector3Int> getBorder(List<Vector3Int> vList)
        => getArea(maxRing(vList) + 1)
            .FindAll(v =>
                !vList.Contains(v)
                //&& getNeighborhood(v).neighbors.ToList()
                //    .Any(nv => vList.Contains(nv))
                );

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

    public static int maxRing(List<Vector3Int> vList)
        => vList.Max(v => ring(v));

    public static int ring(Vector3Int v)
        => Math.Max(Math.Abs(v.x), Math.Max(Math.Abs(v.y), Math.Abs(v.z)));
}
