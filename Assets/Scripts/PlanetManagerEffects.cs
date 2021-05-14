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

    // Start is called before the first frame update
    void Awake()
    {
        Managers.Planet.onPlanetStateChanged += updateDisplay;
        Managers.Planet.onResourcesChanged += updateUI;
        Managers.Edge.onValidPositionListChanged += updateEditDisplay;
        Managers.Input.onMouseOverMoved += updateCursor;
    }

    private void Start()
    {
        updateEditDisplay(Managers.Edge.validPosList);
    }

    public void updateCursor(Vector2 pos)
    {
        cursorObject.SetActive(
            pos != Vector2.one * -1 &&
            Managers.Edge.validPosList.Contains(pos)
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
        posList.ForEach(edge =>
        {
            GameObject editPod = Instantiate(
                editPodPrefab,
                edge,
                Quaternion.identity,
                transform
                );
            SpriteRenderer sr = editPod.GetComponent<SpriteRenderer>();
            sr.sprite = Managers.Input.ToolAction.preview;
            sr.color = Managers.Input.ToolAction.color;
            editPods.Add(editPod);
        });
    }

    void updateUI(float resources)
    {
        txtResources.text = "Resources: " + (int)resources + " / " + Managers.Planet.ResourceCap;
    }
}
