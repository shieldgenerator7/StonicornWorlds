using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Requirement), true)]
public class RequirementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Remove"))
        {
            foreach (object o in targets)
            {
                Requirement req = (Requirement)o;
                ButtonProgressor bp = req.GetComponent<ButtonProgressor>();
                bp.proreqs.Remove(req);
                DestroyImmediate(req);
                EditorUtility.SetDirty(bp);
            }
        }
    }
}
