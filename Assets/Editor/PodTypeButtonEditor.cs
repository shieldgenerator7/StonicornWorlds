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
                if (!ptb.planetObjectType)
                {
                    string typeName = ptb.gameObject.name.Split(' ')[1];
                    PodType podType = Resources.Load<PodType>("PodTypes/" + typeName);
                    if (podType)
                    {
                        ptb.planetObjectType = podType;
                    }
                    else
                    {
                        PodContentType podContentType = Resources
                            .Load<PodContentType>("PodContentTypes/" + typeName);
                        if (podContentType)
                        {
                            ptb.planetObjectType = podContentType;
                        }
                    }
                }
                ptb.image.sprite = ptb.planetObjectType.preview;
                ptb.activeImage.color = ptb.planetObjectType.uiColor;
                ptb.mouseOverImage.color = ptb.planetObjectType.uiColor;
                if (ptb.compatibleToolActions.Count == 0)
                {
                    if (ptb.planetObjectType is PodType pt)
                    {
                        ptb.compatibleToolActions.Add(FindObjectOfType<ConstructAction>());
                        ptb.compatibleToolActions.Add(FindObjectOfType<ConvertAction>());
                    }
                    else if (ptb.planetObjectType is PodContentType pct)
                    {
                        ptb.compatibleToolActions.Add(FindObjectOfType<PlantAction>());
                    }
                }
                EditorUtility.SetDirty(ptb);
                EditorUtility.SetDirty(ptb.image);
                EditorUtility.SetDirty(ptb.activeImage);
                EditorUtility.SetDirty(ptb.mouseOverImage);
                EditorSceneManager.MarkSceneDirty(ptb.gameObject.scene);
            }
        }
    }
}
