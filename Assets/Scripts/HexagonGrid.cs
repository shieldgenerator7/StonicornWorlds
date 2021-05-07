using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class HexagonGrid<T>
{
    /*
     * 2021-05-06 Developed using recommendations from https://www.redblobgames.com/grids/hexagons/#basics
     * Uses Cube Coordinates, hence the Vector3Int
    */

    Dictionary<Vector3Int, T> grid = new Dictionary<Vector3Int, T>();

    public void add(T t, Vector3Int pos)
    {
        grid[pos] = t;
    }

    public void removeAt(Vector3Int pos)
    {
        grid[pos] = default(T);
    }
    public T get(Vector3Int pos)
        => (grid.ContainsKey(pos)) ? grid[pos] : default(T);

    public T getGround(Vector3Int pos)
        => get(HexagonUtility.getGroundPos(pos));

    public Neighborhood<T> getNeighborhood(Vector3Int pos)
    {
        return new Neighborhood<T>(
            HexagonUtility.getNeighborhood(pos),
            this
            );
    }

    public List<Vector3Int> getBorder()
        => HexagonUtility.getBorder(grid.Keys.ToList());

    public static implicit operator List<T>(HexagonGrid<T> hg)
        => hg.grid.Values.ToList();
}
