using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private HexagonGrid grid;

    public Vector2 gridToWorld(Vector3Int hexpos)
    {
        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#hex-to-pixel-axial
        float size = 0.5f;
        float x = size * (3 / 2 * hexpos.x);
        float y = size * (Mathf.Sqrt(3) / 2 * hexpos.x + Mathf.Sqrt(3) * hexpos.z);
        return new Vector2(x, y);
    }

    public Vector3Int worldToGrid(Vector2 pos)
    {
        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#pixel-to-hex
        float size = 0.5f;
        float q = (2 / 3 * pos.x) / size;
        float r = (-1 * pos.x + Mathf.Sqrt(3) * pos.y) / (size * 3);
        float s = -(q + r);

        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#rounding
        int rx = Mathf.RoundToInt(q);
        var ry = Mathf.RoundToInt(s);
        var rz = Mathf.RoundToInt(r);

        var x_diff = Mathf.Abs(rx - q);
        var y_diff = Mathf.Abs(ry - s);
        var z_diff = Mathf.Abs(rz - r);

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
        return new Vector3Int(rx, ry, rz);
    }
}
