using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public List<ProgressionRequirement> proreqs;

    // Start is called before the first frame update
    void Start()
    {
        Managers.Planet.Planet.onStateChanged += (p) => checkAllProgression();
        Managers.Planet.onResourcesChanged += (resources) => checkAllProgression();
        checkAllProgression();
    }

    void checkAllProgression()
    {
        for (int i = proreqs.Count - 1; i >= 0; i--)
        {
            if (proreqs[i].checkProgression())
            {
                Managers.Input.updateToolBoxes();
                Managers.Input.checkAllButtons();
                proreqs.RemoveAt(i);
            }
        }
    }
}
