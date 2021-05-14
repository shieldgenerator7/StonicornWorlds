using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class BuildInfoDisplayer : MonoBehaviour
{
    public TMP_Text txtBuildVersion;

    public void updateBuildInfoTexts()
    {
        txtBuildVersion.text = "STONICORN WORLDS " + PlayerSettings.bundleVersion
            + "\n[no save]";
    }
}
#endif
