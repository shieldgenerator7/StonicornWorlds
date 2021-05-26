using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlanetManagerEffects : MonoBehaviour
{
    public GameObject editPodPrefab;
    public GameObject stonicornPrefab;
    public GameObject cursorObject;

    List<GameObject> editPods = new List<GameObject>();
    List<GameObject> selectObjects = new List<GameObject>();

    Dictionary<PlanetObject, GameObject> displayObjects = new Dictionary<PlanetObject, GameObject>();
    Dictionary<Stonicorn, GameObject> stonicorns = new Dictionary<Stonicorn, GameObject>();

    public void updateCursor(Vector2 pos)
    {
        cursorObject.SetActive(
            pos != Vector2.one * -1 &&
            Managers.Edge.ValidPosList.Contains(pos)
            );
        cursorObject.transform.position = pos;
    }

    public void updateSelect(List<Vector2> posList)
    {
        selectObjects.ForEach(go => go.SetActive(false));
        if (posList.Count == 0)
        {
            return;
        }
        while (selectObjects.Count < posList.Count)
        {
            GameObject select = Instantiate(cursorObject);
            select.SetActive(false);
            select.transform.localScale *= 0.9f;
            selectObjects.Add(select);
        }
        for (int i = 0; i < posList.Count; i++)
        {
            selectObjects[i].SetActive(true);
            selectObjects[i].transform.position = posList[i];
        }
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
                    p.forEachContent(pc =>
                    {
                        removeDisplayObject(pc);
                    });
                    removeDisplayObject(p);
                }
            }
            //Removed PodContents
            else if (po is PodContent pc)
            {
                if (!pc.container.hasContent(pc))
                {
                    removeDisplayObject(pc);
                }
            }
        }
        //Check for added stonicorns
        planet.residents.FindAll(stncrn => !stonicorns.ContainsKey(stncrn))
            .ForEach(stncrn =>
            {
                GameObject go = Instantiate(
                    stonicornPrefab,
                    Vector2.zero,
                    Quaternion.identity,
                    transform
                    );
                go.transform.up = Managers.Planet.Planet.getUpDirection(go.transform.position);
                go.GetComponent<StonicornController>().stonicorn = stncrn;
                stonicorns.Add(stncrn, go);
            });
        //Check for removed stonicorns
        foreach (Stonicorn stncrn in stonicorns.Keys.ToList())
        {
            if (!planet.residents.Contains(stncrn))
            {
                Destroy(stonicorns[stncrn]);
                stonicorns.Remove(stncrn);
            }
        }
    }
    private void removeDisplayObject(PlanetObject po)
    {
        try
        {
            Destroy(displayObjects[po]);
            displayObjects.Remove(po);
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError("Key not found: " + po);
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
