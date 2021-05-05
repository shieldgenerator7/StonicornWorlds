using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(PodTypeButton))]
[CanEditMultipleObjects]
public class PodTypeButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Setup button"))
        {
            foreach (Object t in targets)
            {
                PodTypeButton ptb = (PodTypeButton)t;
                ptb.image.sprite = ptb.podType.preview;
                EditorUtility.SetDirty(ptb);
                EditorUtility.SetDirty(ptb.image);
                EditorSceneManager.MarkSceneDirty(ptb.gameObject.scene);
            }
        }
    }
}
