using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PodType))]
[CanEditMultipleObjects]
public class PodTypeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Setup pod type"))
        {
            foreach (Object t in targets)
            {
                PodType pt = (PodType)t;
                pt.typeName = pt.name;
                pt.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefabs/Pod " + pt.typeName + ".prefab"
                    );
                pt.preview = AssetDatabase.LoadAssetAtPath<Sprite>(
                    "Assets/Sprites/pod_" + pt.typeName.ToLower() + ".png"
                    );
                pt.uiColor = pt.prefab.GetComponent<SpriteRenderer>().color;
                EditorUtility.SetDirty(pt);
            }
        }
    }
}
