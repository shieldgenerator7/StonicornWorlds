using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlanetManagerEffects : MonoBehaviour
{
    public GameObject editPodPrefab;
    public GameObject cursorObject;

    List<GameObject> editPods = new List<GameObject>();

    Dictionary<PlanetObject, GameObject> displayObjects = new Dictionary<PlanetObject, GameObject>();

    public void updateCursor(Vector2 pos)
    {
        cursorObject.SetActive(
            pos != Vector2.one * -1 &&
            Managers.Edge.ValidPosList.Contains(pos)
            );
        cursorObject.transform.position = pos;
    }

    public void updateDisplay(Planet planet)
    {
        //Check for added pods
        List<Pod> podsAll = planet.PodsAll;
        podsAll.ForEach(pod =>
            {
                if (!displayObjects.ContainsKey(pod))
                {
                    GameObject go = Instantiate(
                        pod.podType.prefab,
                        pod.worldPos,
                        Quaternion.identity,
                        transform
                        );
                    go.transform.up = Managers.Planet.Planet.getUpDirection(go.transform.position);
                    displayObjects.Add(pod, go);
                }
                //Check for added pod contents
                pod.forEachContent(content =>
                {
                    if (!displayObjects.ContainsKey(content))
                    {
                        GameObject go = Instantiate(
                            content.contentType.prefab,
                            content.container.worldPos,
                            Quaternion.identity,
                            transform
                            );
                        go.transform.up = Managers.Planet.Planet.getUpDirection(go.transform.position);
                        PodContentDisplayer pcd = go.GetComponent<PodContentDisplayer>();
                        if (pcd)
                        {
                            pcd.setup(content);
                        }
                        displayObjects.Add(content, go);
                    }
                });
            });
        //Check for removed planet objects
        foreach (PlanetObject po in displayObjects.Keys.ToList())
        {
            //Removed Pods
            if (po is Pod p)
            {
                if (!podsAll.Contains(p))
                {
                    Destroy(displayObjects[p]);
                    displayObjects.Remove(p);
                }
            }
            //Removed PodContents
            else if (po is PodContent pc)
            {
                if (!pc.container.hasContent(pc))
                {
                    Destroy(displayObjects[pc]);
                    displayObjects.Remove(pc);
                }
            }
        }
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
}
