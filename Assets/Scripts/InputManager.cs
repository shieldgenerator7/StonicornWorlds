using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : Manager
{
    public Tool tool;

    public List<Tool> tools;
    public List<ToolBox> toolBoxes;
    public List<ToolButton> buttons;

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

    private Vector2 mouseOverHex;
    public Vector2 MouseOver
    {
        get => mouseOverHex;
        set
        {
            if (mouseOverHex != value)
            {
                mouseOverHex = value;
                onMouseOverMoved?.Invoke(mouseOverHex);
            }
        }
    }
    public delegate void OnMouseOverMoved(Vector2 pos);
    public event OnMouseOverMoved onMouseOverMoved;

    private List<Vector2> selectList;
    public List<Vector2> SelectList
    {
        get => selectList;
        set
        {
            if (value != selectList)
            {
                selectList = value;
                onSelectListChanged?.Invoke(selectList);
            }
        }
    }
    public delegate void OnSelectListChanged(List<Vector2> selectList);
    public event OnSelectListChanged onSelectListChanged;

    private bool buttonActivation = false;

    public override void setup()
    {
        if (planetObjectType is PodType pt)
        {
            PodType = pt;
        }
        else if (planetObjectType is PodContentType pct)
        {
            PodContentType = pct;
        }
    }

    public void updateToolBoxes()
    {
        toolBoxes.ForEach(btn => btn.gameObject.SetActive(false));
        int index = 0;
        toolBoxes.FindAll(tb => tb.Enabled)
            .ForEach(tb =>
            {
                tb.organize(index);
                tb.updateColor();
                index++;
            });
    }

    public void checkAllButtons()
    {
        buttons.ForEach(btn => btn.checkActive());
    }

    public List<ToolButton> ActiveButtons
        => buttons.FindAll(btn => btn.Active);

    public KeyCode cheatKey;
    public List<ToolButton> cheatButtons;

    // Update is called once per frame
    void Update()
    {
        //Mouse Over
        List<ToolButton> mobs = buttons
            .FindAll(btn => btn.checkMouseOver(Input.mousePosition));
        if (mobs.Count > 0)
        {
            MouseOver = Vector2.one * -1;
        }
        else
        {
            MouseOver = Managers.Planet.Planet.getHexPos(
                Camera.main.ScreenToWorldPoint(Input.mousePosition)
            );
        }

        bool mouseButtonUp = Input.GetMouseButtonUp(0);
        //Mouse Click
        if (Input.GetMouseButton(0) || mouseButtonUp)
        {
            //Input Down
            if (Input.GetMouseButtonDown(0))
            {
                //Check click on Tool
                ToolButton clickedButton = buttons
                    .FindAll(b => b.gameObject.activeSelf)
                    .FirstOrDefault(b => b.checkClick(Input.mousePosition));
                if (clickedButton)
                {
                    //Click on Button
                    clickedButton.activate();
                    buttonActivation = true;
                    toolBoxes.ForEach(tb => tb.updateColor());
                }
                else
                {
                    //Click in world with Tool
                    tool.inputDown(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    buttonActivation = false;
                }
            }
            //Input Up
            else if (!buttonActivation)
            {
                if (mouseButtonUp)
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
        //Input Idle
        else
        {
            tool.inputIdle(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        //Cheat Key
        if (Input.GetKeyDown(cheatKey))
        {
            cheatButtons.ForEach(btn => btn.gameObject.SetActive(true));
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            updateToolBoxes();
        }
    }
}
