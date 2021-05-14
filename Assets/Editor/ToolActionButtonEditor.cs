using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

[CustomEditor(typeof(ToolActionButton), true)]
[CanEditMultipleObjects]
public class ToolActionButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Setup button"))
        {
            foreach (Object t in targets)
            {
                ToolActionButton tab = (ToolActionButton)t;
                if (!tab.toolAction)
                {
                    string toolName = tab.gameObject.name.Split(' ')[1];
                    tab.toolAction = GameObject.Find(toolName).GetComponent<ToolAction>();
                    //PodType podType = Resources.Load<PodType>("PodTypes/" + typeName);
                    //if (podType)
                    //{
                    //    tab.planetObjectType = podType;
                    //}
                }
                tab.image.sprite = tab.toolAction.preview;
                if (tab.compatibleObjectTypes.Count == 0)
                {
                    tab.compatibleObjectTypes.AddRange(FindObjectOfType<PodTypeBank>().allPodTypes);
                    tab.compatibleObjectTypes.AddRange(FindObjectOfType<PodTypeBank>().allPodContentTypes);
                }
                EditorUtility.SetDirty(tab);
                EditorUtility.SetDirty(tab.image);
                EditorSceneManager.MarkSceneDirty(tab.gameObject.scene);
            }
        }
    }
}