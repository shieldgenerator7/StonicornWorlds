using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexagonGrid
{
    /*
     * 2021-05-06 Developed using recommendations from https://www.redblobgames.com/grids/hexagons/#basics
     * Uses Cube Coordinates, hence the Vector3Int
    */

    Dictionary<Vector3Int, Pod> grid = new Dictionary<Vector3Int, Pod>();

    public void add(Pod pod, Vector3Int pos)
    {
        grid[pos] = pod;
    }

    public void removeAt(Vector3Int pos)
    {
        grid[pos] = null;
    }

    public Pod getPod(Vector3Int pos)
        => grid[pos];

    public Vector3Int getGroundPos(Vector3Int pos)
    {
        Vector3Int ground = reduceAbs(pos);
        if (pos.x == 0)
        {
            ground.x = pos.x;
        }
        else if (pos.y == 0)
        {
            ground.y = pos.y;
        }
        else if (pos.z == 0)
        {
            ground.z = pos.z;
        }
        else
        {
            if (Math.Sign(pos.x) == Mathf.Sign(pos.y))
            {
                ground.y = pos.y;
            }
            else
            {
                ground.z = pos.z;
            }
        }
        return ground;
    }

    public PodNeighborhood getNeighborhood(Vector3Int pos)
    {
        PodNeighborhood neighborhood = new PodNeighborhood();
        Vector3Int groundPos = getGroundPos(pos);
        neighborhood.ground = getPod(groundPos);
        neighborhood.groundLeft = getPod(rotate(groundPos, pos, 1));
        neighborhood.groundRight = getPod(rotate(groundPos, pos, -1));
        Vector3Int ceilPos = reflect(groundPos, pos);
        neighborhood.ceiling = getPod(ceilPos);
        neighborhood.ceilingLeft = getPod(rotate(ceilPos, pos, -1));
        neighborhood.ceilingRight = getPod(rotate(ceilPos, pos, 1));
        neighborhood.neighbors = new Pod[]
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

    private Vector3Int reduceAbs(Vector3Int v)
    {
        v.x = reduceAbs(v.x);
        v.y = reduceAbs(v.y);
        v.z = reduceAbs(v.z);
        return v;
    }

    private int reduceAbs(int n)
        => (n == 0) ? 0 : n - Math.Sign(n);

    private Vector3Int rotate(Vector3Int pos, Vector3Int center, int angle)
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
    private Vector3Int reflect(Vector3Int pos, Vector3Int center)
    {
        pos -= center;
        pos *= -1;
        pos += center;
        return pos;
    }

}
