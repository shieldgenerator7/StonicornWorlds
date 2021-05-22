using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressionManager : Manager
{
    public List<ProgressionRequirement> proreqs;

    public override void setup()
    {
        checkAllProgression();
    }

    public void checkAllProgression()
    {
        bool anyProgressed = false;
        for (int i = proreqs.Count - 1; i >= 0; i--)
        {
            if (proreqs[i].checkProgression())
            {
                proreqs.RemoveAt(i);
                anyProgressed = true;
            }
        }
        if (anyProgressed)
        {
            onProgressionChanged?.Invoke();
        }
    }
    public delegate void OnProgressionChanged();
    public event OnProgressionChanged onProgressionChanged;
}
