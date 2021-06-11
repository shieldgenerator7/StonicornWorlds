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

        List<System.Type> options = new List<System.Type>()
        {
            typeof(PodTypeRequirement),
            typeof(PodContentTypeRequirement),
            typeof(ResourceRequirement),
            typeof(StonicornRequirement),
            typeof(QueueRequirement),
            typeof(SaveFileRequirement),
            typeof(AsteroidRequirement),
        };
        options.RemoveAll(opt => anyHas(targets, opt));
        if (options.Count > 0)
        {
            int selected = EditorGUILayout.Popup(
                "Requirement Type",
                -1,
                options.ConvertAll(opt => opt.Name).ToArray()
                );
            if (selected >= 0)
            {
                foreach (object o in targets)
                {
                    ButtonProgressor bp = (ButtonProgressor)o;
                    System.Type type = options[selected];
                    addRequirement(bp, type);
                    EditorUtility.SetDirty(bp);
                }
            }
        }
    }

    bool anyHas<T>(Object[] targets) where T : Requirement
    {
        return targets.ToList()
            .ConvertAll(target => (ButtonProgressor)target)
            .Any(btnpro => btnpro.GetComponent<T>());
    }

    bool anyHas(Object[] targets, System.Type t)
    {
        return targets.ToList()
            .ConvertAll(target => (ButtonProgressor)target)
            .Any(btnpro => btnpro.GetComponent(t));
    }

    void addRequirement<T>(ButtonProgressor bp) where T : Requirement
    {
        T req = bp.gameObject.AddComponent<T>();
        if (req)
        {
            bp.proreqs.Add(req);
        }
    }

    void addRequirement(ButtonProgressor bp, System.Type t)
    {
        Requirement req = (Requirement)bp.gameObject.AddComponent(t);
        if (req)
        {
            bp.proreqs.Add(req);
            EditorUtility.SetDirty(req);
        }
    }
}
