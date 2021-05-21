using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProgressionManager))]
public class ProgressionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Progress All (Play Mode)"))
        {
            ProgressionManager pm = (ProgressionManager)target;
            pm.proreqs.ForEach(pr => pr.button.gameObject.SetActive(true));
            pm.proreqs.Clear();
            Managers.Input.updateToolBoxes();
        }
    }
}
