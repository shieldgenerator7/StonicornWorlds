using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public List<ProgressionRequirement> proreqs;

    // Start is called before the first frame update
    void Start()
    {
        Managers.Planet.planet.onStateChanged += (p) => checkAllProgression();
        Managers.Planet.onResourcesChanged += (resources) => checkAllProgression();
        checkAllProgression();
    }

    void checkAllProgression()
    {
        for (int i = proreqs.Count - 1; i >= 0; i--)
        {
            if (proreqs[i].checkProgression())
            {
                proreqs.RemoveAt(i);
            }
        }
    }
}
