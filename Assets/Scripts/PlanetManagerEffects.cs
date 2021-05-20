using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetManagerEffects : MonoBehaviour
{
    public GameObject editPodPrefab;
    public GameObject cursorObject;

    public TMP_Text txtResources;

    List<GameObject> goPods = new List<GameObject>();
    List<GameObject> goPodContents = new List<GameObject>();
    List<GameObject> editPods = new List<GameObject>();

    public void updateCursor(Vector2 pos)
    {
        cursorObject.SetActive(
            pos != Vector2.one * -1 &&
            Managers.Edge.ValidPosList.Contains(pos)
            );
        cursorObject.transform.position = pos;
    }

    public void updateDisplay(Planet p)
    {
        //Update pods
        goPods.ForEach(go => Destroy(go));
        goPods.Clear();
        p.PodsAll.ForEach(pod =>
            {
                GameObject go = Instantiate(
                    pod.podType.prefab,
                    pod.pos,
                    Quaternion.identity,
                    transform
                    );
                go.transform.up = Managers.Planet.Planet.getUpDirection(go.transform.position);
                goPods.Add(go);
            });
        //Update pod contents
        goPodContents.ForEach(go => Destroy(go));
        goPodContents.Clear();
        Managers.Planet.Planet.forEachPodContent(
            content =>
            {
                GameObject go = Instantiate(
                    content.contentType.prefab,
                    content.container.pos,
                    Quaternion.identity,
                    transform
                    );
                go.transform.up = Managers.Planet.Planet.getUpDirection(go.transform.position);
                goPodContents.Add(go);
            });
    }
    public void updateEditDisplay(List<Vector2> posList)
    {
        //Update edit pod UI
        editPods.ForEach(go => Destroy(go));
        editPods.Clear();
        Color color = Managers.Input.ToolAction.color;
        Vector2 up = Managers.Camera.transform.up;
        Sprite preview = Managers.Input.ToolAction.preview;
        posList.ForEach(edge =>
        {
            GameObject editPod = Instantiate(
                editPodPrefab,
                edge,
                Quaternion.identity,
                transform
                );
            editPod.GetComponent<SpriteRenderer>().color = color;
            SpriteRenderer sr = editPod.transform.GetChild(0).GetComponent<SpriteRenderer>();
            sr.sprite = preview;
            sr.color = color;
            sr.transform.up = up;
            editPods.Add(editPod);
        });
    }
    public void updateEditDisplay(Vector2 up)
    {
        editPods.ForEach(go =>
            go.transform.GetChild(0).up = up
        );
    }

    public void updateUI(float resources)
    {
        txtResources.text = "Resources: " + (int)resources + " / " + Managers.Planet.ResourceCap;
    }
}
