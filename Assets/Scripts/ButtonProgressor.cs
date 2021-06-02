using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonProgressor : MonoBehaviour
{
    public ToolButton button;
    public List<Requirement> proreqs;

    public bool checkProgression()
    {
        //if already progressed,
        if (button.gameObject.activeSelf)
        {
            //return true
            return true;
        }
        bool allProgressed = proreqs.All(proreq => proreq.isRequirementMet());
        if (allProgressed)
        {
            button.gameObject.SetActive(true);
            return true;
        }
        return false;
    }
}
