using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Planet
{
    public Vector2 position;
    public float size = 0.5f;
    [NonSerialized]
    float SQRT_3 = Mathf.Sqrt(3.0f);

    private HexagonGrid<Pod> grid = new HexagonGrid<Pod>();

    #region Write State
    public delegate void OnStateChanged(Planet p);
    public event OnStateChanged onStateChanged;

    public void addPod(Pod pod, Vector2 pos)
    {
        Vector3Int hexpos = worldToGrid(pos);
        pod.pos = gridToWorld(hexpos);
        grid.add(pod, worldToGrid(pos));
        onStateChanged?.Invoke(this);
    }

    public void removePod(Vector2 pos)
    {
        grid.removeAt(worldToGrid(pos));
        onStateChanged?.Invoke(this);
    }
    #endregion

    #region Read State
    public Pod getPod(Vector2 pos)
        => grid.get(worldToGrid(pos));

    public Pod getGroundPod(Vector2 pos)
        => grid.getGround(worldToGrid(pos));

    public Vector2 getGroundPos(Vector2 pos)
        => gridToWorld(HexagonUtility.getGroundPos(worldToGrid(pos)));

    public Neighborhood<Pod> getNeighborhood(Vector2 pos)
        => grid.getNeighborhood(worldToGrid(pos));

    public List<Pod> Pods
        => grid;

    public List<Vector2> Border
        => grid.getBorder().ConvertAll(v => gridToWorld(v));
    #endregion

    #region Grid Conversion
    private Vector2 gridToWorld(Vector3Int hexpos)
    {
        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#hex-to-pixel-axial
        float x = size * (3.0f * hexpos.x / 2.0f);
        float y = size * SQRT_3 * (hexpos.x / 2.0f + hexpos.z);
        return new Vector2(x, y) + position;
    }
    private Vector3Int worldToGrid(Vector2 pos)
    {
        pos -= position;
        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#pixel-to-hex
        float q = (2.0f * pos.x) / (size * 3.0f);
        float r = (-1 * pos.x + SQRT_3 * pos.y) / (size * 3.0f);
        float s = -(q + r);

        //2021-05-06: copied from https://www.redblobgames.com/grids/hexagons/#rounding
        float rx = Mathf.Round(q);
        float ry = Mathf.Round(s);
        float rz = Mathf.Round(r);

        float x_diff = Mathf.Abs(rx - q);
        float y_diff = Mathf.Abs(ry - s);
        float z_diff = Mathf.Abs(rz - r);

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
    #endregion

    public Planet deepCopy()
    {
        Planet planet = JsonUtility.FromJson<Planet>(JsonUtility.ToJson(this));
        planet.Pods.ForEach(pod => pod.inflate());
        return planet;
        //return (Planet)FromBinary(ToBinary(this));
    }

    #region deep copy
    //2021-05-06: copied from https://stackoverflow.com/a/140279/2336212
    public static Byte[] ToBinary(Planet planet)
    {
        MemoryStream ms = null;
        Byte[] byteArray = null;
        try
        {
            BinaryFormatter serializer = new BinaryFormatter();
            ms = new MemoryStream();
            serializer.Serialize(ms, planet);
            byteArray = ms.ToArray();
        }
        catch (Exception unexpected)
        {
            Debug.LogError(unexpected.Message);
            throw;
        }
        finally
        {
            if (ms != null)
                ms.Close();
        }
        return byteArray;
    }

    public static object FromBinary(Byte[] buffer)
    {
        MemoryStream ms = null;
        object deserializedObject = null;

        try
        {
            BinaryFormatter serializer = new BinaryFormatter();
            ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            ms.Position = 0;
            deserializedObject = serializer.Deserialize(ms);
        }
        finally
        {
            if (ms != null)
                ms.Close();
        }
        return deserializedObject;
    }
    #endregion
}
