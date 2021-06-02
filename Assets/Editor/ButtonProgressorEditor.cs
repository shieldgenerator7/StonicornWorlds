using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ButtonProgressor))]
public class ButtonProgressorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Pod Type Requirement"))
        {
            foreach (object o in targets)
            {
                ButtonProgressor bp = (ButtonProgressor)o;
                addRequirement<PodTypeRequirement>(bp);
            }
        }
        if (!anyHas<ResourceRequirement>(targets) &&
            GUILayout.Button("Resource Requirement"))
        {
            foreach (object o in targets)
            {
                ButtonProgressor bp = (ButtonProgressor)o;
                addRequirement<ResourceRequirement>(bp);
            }
        }
        if (!anyHas<SaveFileRequirement>(targets) && 
            GUILayout.Button("SaveFile Requirement"))
        {
            foreach (object o in targets)
            {
                ButtonProgressor bp = (ButtonProgressor)o;
                addRequirement<SaveFileRequirement>(bp);
            }
        }
    }

    bool anyHas<T>(Object[] targets) where T : Requirement
    {
        return targets.ToList()
            .ConvertAll(target => (ButtonProgressor)target)
            .Any(btnpro => btnpro.GetComponent<T>());
    }

    void addRequirement<T>(ButtonProgressor bp) where T : Requirement
    {
        T req = bp.gameObject.AddComponent<T>();
        if (req)
        {
            bp.proreqs.Add(req);
        }
    }
}
