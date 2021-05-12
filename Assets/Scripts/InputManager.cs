using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    Tool tool;
    public List<Tool> tools;

    [SerializeField]
    private PlanetObjectType planetObjectType;
    public PlanetObjectType PlanetObjectType
    {
        get => planetObjectType;
        set
        {
            planetObjectType = value;
            if (planetObjectType is PodType pt)
            {
                PodType = pt;
            }
            else if (planetObjectType is PodContentType pct)
            {
                PodContentType = pct;
            }
            onPlanetObjectTypeChanged?.Invoke(planetObjectType);
        }
    }
    public delegate void OnPlanetObjectTypeChanged(PlanetObjectType planetObjectType);
    public event OnPlanetObjectTypeChanged onPlanetObjectTypeChanged;

    public PodType PodType { get; private set; }
    public PodContentType PodContentType { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        //tools = FindObjectsOfType<Tool>().ToList();
        //tool.activate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Input Down
            if (Input.GetMouseButtonDown(0))
            {
                //Check click on Tool
                Tool nextTool = tools.FirstOrDefault(t => t.checkClick(Input.mousePosition));
                if (nextTool)
                {
                    tool = nextTool;
                    tool.activate();
                }
                else
                {
                    tool.inputDown(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
            //Input Up
            else if (Input.GetMouseButtonUp(0))
            {
                tool.inputUp(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            //Input Move
            else
            {
                tool.inputMove(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }
}
