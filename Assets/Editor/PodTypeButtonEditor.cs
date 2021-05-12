using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

[CustomEditor(typeof(PlanetObjectTypeButton), true)]
[CanEditMultipleObjects]
public class PlanetObjectTypeButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Setup button"))
        {
            foreach (Object t in targets)
            {
                PlanetObjectTypeButton ptb = (PlanetObjectTypeButton)t;
                ptb.image.sprite = ptb.planetObjectType.preview;
                EditorUtility.SetDirty(ptb);
                EditorUtility.SetDirty(ptb.image);
                EditorSceneManager.MarkSceneDirty(ptb.gameObject.scene);
            }
        }
    }
}
