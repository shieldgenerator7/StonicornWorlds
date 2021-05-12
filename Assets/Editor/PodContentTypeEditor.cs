using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PodContentType))]
[CanEditMultipleObjects]
public class PodContentTypeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Setup pod content type"))
        {
            foreach (Object t in targets)
            {
                PodContentType pct = (PodContentType)t;
                pct.typeName = pct.name;
                pct.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefabs/Pod " + pct.typeName + ".prefab"
                    );
                pct.preview = AssetDatabase.LoadAssetAtPath<Sprite>(
                    "Assets/Sprites/pod_" + pct.typeName.ToLower() + ".png"
                    );
                pct.uiColor = pct.prefab.GetComponent<SpriteRenderer>().color;
                EditorUtility.SetDirty(pct);
            }
        }
    }
}
