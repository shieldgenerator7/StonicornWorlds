using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

[CustomEditor(typeof(PlanetObjectTypeTool), true)]
[CanEditMultipleObjects]
public class PlanetObjectTypeToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Setup button"))
        {
            foreach (Object t in targets)
            {
                PlanetObjectTypeTool ptb = (PlanetObjectTypeTool)t;
                ptb.image.sprite = ptb.planetObjectType.preview;
                EditorUtility.SetDirty(ptb);
                EditorUtility.SetDirty(ptb.image);
                EditorSceneManager.MarkSceneDirty(ptb.gameObject.scene);
            }
        }
    }
}
