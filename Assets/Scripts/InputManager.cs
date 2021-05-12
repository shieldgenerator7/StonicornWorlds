using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Tool tool;
    private List<Tool> tools;

    [SerializeField]
    private List<ToolButton> buttons;

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

    [SerializeField]
    private ToolAction toolAction;
    public ToolAction ToolAction
    {
        get => toolAction;
        set
        {
            toolAction = value;
            onToolActionChanged?.Invoke(toolAction);
        }
    }
    public delegate void OnToolActionChanged(ToolAction toolAction);
    public event OnToolActionChanged onToolActionChanged;

    private bool buttonActivation = false;

    // Start is called before the first frame update
    void Start()
    {
        tools = FindObjectsOfType<Tool>().ToList();
        //tool.activate();
        onPlanetObjectTypeChanged += (pot) => checkAllButtons();
        onToolActionChanged += (ta) => checkAllButtons();
    }

    void checkAllButtons()
    {
        buttons.ForEach(btn => btn.checkActive());
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
                ToolButton clickedButton = buttons.FirstOrDefault(b => b.checkClick(Input.mousePosition));
                if (clickedButton)
                {
                    clickedButton.activate();
                    buttonActivation = true;
                }
                else
                {
                    tool.inputDown(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    buttonActivation = false;
                }
            }
            //Input Up
            else if (!buttonActivation)
            {
                if (Input.GetMouseButtonUp(0))
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
}
