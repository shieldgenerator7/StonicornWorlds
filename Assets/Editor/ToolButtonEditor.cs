using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using System.Linq;

[CustomEditor(typeof(ToolButton), true)]
[CanEditMultipleObjects]
public class ToolButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Setup button"))
        {
            foreach (Object t in targets)
            {
                ToolButton tb = (ToolButton)t;
                if (tb is ToolBox tbox)
                {
                    tbox.compatibilities.ganzEgal = true;
                }
                else if (tb is ToolActionButton tab)
                {
                    if (!tab.toolAction)
                    {
                        string toolName = tb.gameObject.name.Split(' ')[1];
                        tab.toolAction = GameObject.Find(toolName).GetComponent<ToolAction>();
                        //PodType podType = Resources.Load<PodType>("PodTypes/" + typeName);
                        //if (podType)
                        //{
                        //    tab.planetObjectType = podType;
                        //}
                    }
                    tab.image.sprite = tab.toolAction.preview;
                }
                else if (tb is PlanetObjectTypeButton ptb)
                {
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
                }
                tb.activeImage.color = tb.image.color;
                tb.mouseOverImage.color = tb.image.color;
                if (!tb.compatibilities.ganzEgal)
                {
                    //compatible tools
                    if (tb.compatibilities.tools.Count == 0)
                    {
                        if (tb is ToolToolButton ttb)
                        {
                            ttb.compatibilities.tools.Add(ttb.tool);
                        }
                        else
                        {
                            tb.compatibilities.tools.AddRange(FindObjectsOfType<Tool>(true).ToList());
                        }
                    }
                    //compatible tool actions
                    if (tb.compatibilities.toolActions.Count == 0)
                    {
                        if (tb is PlanetObjectTypeButton ptb)
                        {
                            if (ptb.planetObjectType is PodType pt)
                            {
                                ptb.compatibilities.toolActions.Add(FindObjectOfType<ConstructAction>());
                                ptb.compatibilities.toolActions.Add(FindObjectOfType<ConvertAction>());
                            }
                            else if (ptb.planetObjectType is PodContentType pct)
                            {
                                ptb.compatibilities.toolActions.Add(FindObjectOfType<PlantAction>());
                            }
                        }
                        else if (tb is ToolActionButton tab)
                        {
                            tab.compatibilities.toolActions.Add(tab.toolAction);
                        }
                        else
                        {
                            tb.compatibilities.toolActions = FindObjectsOfType<ToolAction>(true).ToList();
                        }
                    }
                    //compatible planet object types
                    if (tb.compatibilities.planetObjectTypes.Count == 0)
                    {
                        if (tb is PlanetObjectTypeButton ptb)
                        {
                            ptb.compatibilities.planetObjectTypes.Add(ptb.planetObjectType);
                        }
                        else
                        {
                            tb.compatibilities.planetObjectTypes.AddRange(FindObjectOfType<ConstantBank>().allPodTypes);
                            tb.compatibilities.planetObjectTypes.AddRange(FindObjectOfType<ConstantBank>().allPodContentTypes);
                        }
                    }
                }
                //set dirty
                EditorUtility.SetDirty(tb);
                EditorUtility.SetDirty(tb.image);
                EditorUtility.SetDirty(tb.activeImage);
                EditorUtility.SetDirty(tb.mouseOverImage);
                EditorSceneManager.MarkSceneDirty(tb.gameObject.scene);
            }
        }
    }
}
