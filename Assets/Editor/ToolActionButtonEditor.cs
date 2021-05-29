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
                tab.activeImage.color = tab.image.color;
                tab.mouseOverImage.color = tab.image.color;
                if (tab.compatibleObjectTypes.Count == 0)
                {
                    tab.compatibleObjectTypes.AddRange(FindObjectOfType<ConstantBank>().allPodTypes);
                    tab.compatibleObjectTypes.AddRange(FindObjectOfType<ConstantBank>().allPodContentTypes);
                }
                EditorUtility.SetDirty(tab);
                EditorUtility.SetDirty(tab.image);
                EditorUtility.SetDirty(tab.activeImage);
                EditorUtility.SetDirty(tab.mouseOverImage);
                EditorSceneManager.MarkSceneDirty(tab.gameObject.scene);
            }
        }
    }
}
