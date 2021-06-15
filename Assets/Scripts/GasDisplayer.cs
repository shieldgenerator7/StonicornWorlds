using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasDisplayer : MonoBehaviour
{
    public GasDiffuser gasDiffuser;

    private Dictionary<Vector3Int, SpriteRenderer> srs = new Dictionary<Vector3Int, SpriteRenderer>();

    private HexagonGrid<float> gasGrid;

    // Start is called before the first frame update
    public void Start()
    {
        Debug.Log("Displaying gas: " + gasDiffuser.gasPodContentType.name);
        gasGrid = Managers.Planet.Planet.getGasGrid(gasDiffuser.gasPodContentType);
        this.enabled = false;
        Managers.Processor.onFastForwardFinished += () => this.enabled = true;
    }

    public void Update()
    {
        foreach (Vector3Int v in gasGrid.Keys)
        {
            if (gasGrid[v] > 0 && gasGrid[v] != gasDiffuser.emitterPressure)
            {
                if (!srs.ContainsKey(v))
                {
                    GameObject goGas = Instantiate(gasDiffuser.gasPodContentType.prefab);
                    goGas.transform.position = Managers.Planet.Planet.getHexPos(v);
                    srs.Add(v, goGas.GetComponent<SpriteRenderer>());
                }
                updateDisplay(srs[v], gasGrid[v]);
            }
            else
            {
                if (srs.ContainsKey(v))
                {
                    Destroy(srs[v].gameObject);
                    srs.Remove(v);
                }
            }
        }
    }

    void updateDisplay(SpriteRenderer sr, float val)
    {
        Color color = sr.color;
        color.a = val / 100;
        sr.color = color;
    }
}
