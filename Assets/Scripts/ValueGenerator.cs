using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ValueGenerator
{
    public float min = 0;
    public float max = 100;
    public int samples = 5;
    public int decimalPlaces = 2;
    public enum Choice
    {
        AVG,
        MIN,
        MAX,
    }
    public Choice choice = Choice.AVG;

    public float generate()
    {
        List<float> values = new List<float>();
        float ten = Mathf.Pow(10, decimalPlaces);
        for (int i = 0; i < samples; i++)
        {
            float val = Random.Range(min, max);
            val = Mathf.Round(val * ten) / ten;
            values.Add(val);
        }
        switch (choice)
        {
            case Choice.AVG:
                return values.Sum() / values.Count;
            case Choice.MIN:
                return values.Min();
            case Choice.MAX:
                return values.Max();
            default:
                Debug.LogError("Unknown value: " + choice);
                return 0;
        }
    }
}
