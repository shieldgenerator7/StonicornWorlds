using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetManagerEffects : MonoBehaviour
{
    public GameObject editPodPrefab;
    public GameObject stonicornPrefab;
    public GameObject cursorObject;

    public StonicornHUD stonicornHUD;
    public SpriteRenderer spaceField;
    public float starFieldScale = 1.1f;

    List<SpriteRenderer> editPods = new List<SpriteRenderer>();
    List<GameObject> selectObjects = new List<GameObject>();

    Dictionary<PlanetObject, GameObject> displayObjects = new Dictionary<PlanetObject, GameObject>();
    public Dictionary<Stonicorn, GameObject> stonicorns = new Dictionary<Stonicorn, GameObject>();

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
        //special case: no selection
        if (posList.Count == 0)
        {
            selectObjects.ForEach(go => go.SetActive(false));
            return;
        }
        //fill up select objects list
        while (selectObjects.Count < posList.Count)
        {
            GameObject select = Instantiate(cursorObject);
            select.SetActive(false);
            select.transform.localScale *= 0.9f;
            selectObjects.Add(select);
        }
        //display select objects
        for (int i = 0; i < posList.Count; i++)
        {
            selectObjects[i].transform.position = posList[i];
            selectObjects[i].SetActive(true);
        }
        //hide unnecessary select objects
        for (int i = posList.Count; i < selectObjects.Count; i++)
        {
            selectObjects[i].SetActive(false);
        }
    }

    public void updateDisplay(Planet planet)
    {
        PodType spacePodType = Managers.Constants.spacePodType;
        //Check for added pods
        List<Pod> podsAll = planet.PodsAll;
        podsAll
            .ForEach(pod =>
            {
                if (pod.podType != spacePodType &&
                    !displayObjects.ContainsKey(pod)
                )
                {
                    GameObject go = Instantiate(
                        pod.podType.prefab,
                        pod.worldPos,
                        Quaternion.identity,
                        transform
                        );
                    go.transform.up = Managers.Planet.Planet.getUpDirection(go.transform.position);
                    //special case: pointing straight down
                    if (go.transform.eulerAngles.x != 0 || go.transform.eulerAngles.y != 0)
                    {
                        //prevent (-180,0,0)
                        go.transform.eulerAngles = Vector3.forward * 180;
                    }
                    displayObjects.Add(pod, go);
                }
                //Check for added pod contents
                pod.forEachContent(content =>
                {
                    if (!content.contentType.isGas && !displayObjects.ContainsKey(content))
                    {
                        GameObject go = Instantiate(
                            content.contentType.prefab,
                            content.container.worldPos,
                            Quaternion.identity,
                            transform
                            );
                        go.transform.up = Managers.Planet.Planet.getUpDirection(go.transform.position);
                        //special case: pointing straight down
                        if (go.transform.eulerAngles.x != 0 || go.transform.eulerAngles.y != 0)
                        {
                            //prevent (-180,0,0)
                            go.transform.eulerAngles = Vector3.forward * 180;
                        }
                        //Check for PodContentDisplayer
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
                        if (!pc.contentType.isGas)
                        {
                            removeDisplayObject(pc);
                        }
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
        //Rotate lava pods to face closest core
        new List<string>() { "Lava", "Lava2" }.ForEach(podName =>
        {
            planet.Pods(Managers.Constants.getPodType(podName)).ForEach(
                lava => displayObjects[lava].transform.up =
                planet.getUpDirectionClosestCore(lava.worldPos)
                );
        });
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
                go.GetComponent<StonicornDisplayer>().stonicorn = stncrn;
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
        //Fill up editPods list
        while (editPods.Count < posList.Count)
        {
            GameObject editPod = Instantiate(
                editPodPrefab,
                Vector2.zero,
                Quaternion.identity,
                transform
                );
            editPods.Add(editPod.GetComponent<SpriteRenderer>());
        }
        //Display positions
        Color color = Managers.Input.ToolAction.color;
        Vector2 up = Managers.Camera.transform.up;
        Sprite preview = Managers.Input.ToolAction.preview;
        for (int i = 0; i < posList.Count; i++)
        {
            SpriteRenderer psr = editPods[i];
            psr.transform.position = posList[i];
            psr.color = color;
            SpriteRenderer sr = psr.transform.GetChild(0).GetComponent<SpriteRenderer>();
            sr.sprite = preview;
            sr.color = color;
            sr.transform.up = up;
            psr.gameObject.SetActive(true);
        }
        //Turn off unnecessary editPods
        for (int i = posList.Count; i < editPods.Count; i++)
        {
            SpriteRenderer psr = editPods[i];
            psr.gameObject.SetActive(false);
        }
    }
    public void updateEditDisplay(Vector2 up)
    {
        editPods.ForEach(go =>
            go.transform.GetChild(0).up = up
        );
        spaceField.transform.up = Vector2.up;
    }

    public void updateSpaceField(float scale)
    {
        Camera cam = Camera.main;
        Vector2 sizeWorld =
            cam.ScreenToWorldPoint(Vector2.zero) -
            cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, cam.pixelHeight));
        spaceField.size = Vector2.one * sizeWorld.magnitude;
        //spaceField.transform.localScale = Vector3.one * (scale * starFieldScale);
    }

    public void updateStonicornInfo(Stonicorn stonicorn)
    {
        stonicornHUD.trackStonicorn(stonicorn);
    }
}
