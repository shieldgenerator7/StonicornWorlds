using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public Pod get(Vector3Int pos)
        => (grid.ContainsKey(pos)) ? grid[pos] : null;

    public Pod getGround(Vector3Int pos)
        => get(HexagonUtility.getGroundPos(pos));

    public Neighborhood getNeighborhood(Vector3Int pos)
    {
        return new Neighborhood(
            HexagonUtility.getNeighborhood(pos),
            this
            );
    }

    public List<Vector3Int> getBorder()
        => HexagonUtility.getBorder(grid.Keys.ToList());

    public static implicit operator List<Pod>(HexagonGrid hg)
        => hg.grid.Values.ToList();
}
