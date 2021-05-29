using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HexagonUtility
{
    public static Vector3Int getGroundPos(Vector3Int pos)
    {
        if (pos == Vector3Int.zero)
        {
            return new Vector3Int(0, -1, 1);
        }
        Vector3Int groundPos = reduceAbs(pos);
        //if original pos has a zero coordinate
        if (pos.x == 0 || pos.y == 0 || pos.z == 0)
        {
            //all good, no changes needed
        }
        //if ground pos has a zero coordinate
        else if (groundPos.x == 0)
        {
            groundPos.x = pos.x;
        }
        else if (groundPos.y == 0)
        {
            groundPos.y = pos.y;
        }
        else if (groundPos.z == 0)
        {
            groundPos.z = pos.z;
        }
        //else check coordinate sign alignment
        else if (Math.Sign(pos.x) == Mathf.Sign(pos.y))
        {
            groundPos.y = pos.y;
        }
        else
        {
            groundPos.z = pos.z;
        }
        return groundPos;
    }

    public static Vector3Int getCeilingPos(Vector3Int pos)
        => reflect(getGroundPos(pos), pos);

    public static HexagonNeighborhood getNeighborhood(Vector3Int pos)
    {
        HexagonNeighborhood neighborhood = new HexagonNeighborhood();
        Vector3Int groundPos = getGroundPos(pos);
        neighborhood.ground = groundPos;
        neighborhood.groundLeft = rotate(groundPos, pos, -1);
        neighborhood.groundRight = rotate(groundPos, pos, 1);
        Vector3Int ceilPos = reflect(groundPos, pos);
        neighborhood.ceiling = ceilPos;
        neighborhood.ceilingLeft = rotate(ceilPos, pos, 1);
        neighborhood.ceilingRight = rotate(ceilPos, pos, -1);
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

    public static List<Vector3Int> getLine(Vector3Int start, Vector3Int end)
    {
        //special case: same hex
        if (start == end)
        {
            return new List<Vector3Int>() { start };
        }
        //special case: neighboring hex
        if (cube_distance(start, end) == 1)
        {
            return new List<Vector3Int>() { start, end };
        }
        //normal case:
        //2021-05-23: copied from https://www.redblobgames.com/grids/hexagons/#line-drawing
        int N = cube_distance(start, end);
        List<Vector3Int> results = new List<Vector3Int>();
        for (int i = 0; i <= N; i++)
        {
            results.Add(cube_lerp(start, end, 1.0f / N * i));
        }
        return results;
    }

    public static List<Vector3Int> getRing(int ringIndex)
    {
        List<Vector3Int> ring = new List<Vector3Int>();
        for (int q = -ringIndex; q <= ringIndex; q++)
        {
            for (int r = -ringIndex; r <= ringIndex; r++)
            {
                int s = -(q + r);
                if (Mathf.Max(Mathf.Abs(q), Mathf.Abs(s), Mathf.Abs(r)) == ringIndex)
                {
                    ring.Add(new Vector3Int(q, s, r));
                }
            }
        }
        return ring;
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
                && getNeighborhood(v).neighbors.ToList()
                    .Any(nv => vList.Contains(nv))
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
            angle--;
            pos = new Vector3Int(-pos.z, -pos.x, -pos.y);
        }
        //rotate counter-clockwise
        while (angle < 0)
        {
            angle++;
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
        => (vList.Count == 0) ? -1 : vList.Max(v => ring(v));

    public static int ring(Vector3Int v)
        => Math.Max(Math.Abs(v.x), Math.Max(Math.Abs(v.y), Math.Abs(v.z)));

    #region Math Operations
    //2021-05-23: copied from https://www.redblobgames.com/grids/hexagons/#distances
    public static int cube_distance(Vector3Int a, Vector3Int b)
        => (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;

    public static Vector3Int round(float x, float y, float z)
    {
        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#rounding
        float rx = Mathf.Round(x);
        float ry = Mathf.Round(y);
        float rz = Mathf.Round(z);

        float x_diff = Mathf.Abs(rx - x);
        float y_diff = Mathf.Abs(ry - y);
        float z_diff = Mathf.Abs(rz - z);

        if (x_diff > y_diff && x_diff > z_diff)
        {
            rx = -ry - rz;
        }
        else if (y_diff > z_diff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }
        return new Vector3Int((int)rx, (int)ry, (int)rz);
    }

    //2021-05-23: copied from https://www.redblobgames.com/grids/hexagons/#line-drawing
    private static float lerp(int a, int b, float t)
        => a + ((b - a) * t);

    private static Vector3Int cube_lerp(Vector3Int a, Vector3Int b, float t)
        => round(lerp(a.x, b.x, t),
                lerp(a.y, b.y, t),
                lerp(a.z, b.z, t));

    #endregion
}
