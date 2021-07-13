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
                int rowsUsed = tb.organize(index);
                tb.updateColor();
                index += rowsUsed;
            });
    }

    public void checkAllButtons()
    {
        buttons.ForEach(btn => btn.checkActive());
    }

    public List<ToolButton> ActiveButtons
        => buttons.FindAll(btn => btn.Active);

    public bool cheatsActive = false;
    public KeyCode cheatKey;
    public List<ToolButton> cheatButtons;

    // Update is called once per frame
    public override void update(float timeDelta)
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

        //Cheat Key
        if (Input.anyKeyDown)
        {
            if (cheatsActive)
            {
                //Activate cheat buttons
                if (Input.GetKeyDown(cheatKey))
                {
                    cheatButtons.ForEach(btn => btn.gameObject.SetActive(true));
                }
                //Relayout Toolboxes
                else if (Input.GetKeyDown(KeyCode.B))
                {
                    updateToolBoxes();
                }
                //Summon Asteroids
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    FindObjectOfType<AsteroidEvent>().timeLeft = 0;
                }
                //Progress All Buttons
                else if (Input.GetKeyDown(KeyCode.P))
                {
                    FindObjectsOfType<ToolButton>(true).ToList()
                        .FindAll(btn => !cheatButtons.Contains(btn))
                        .ForEach(btn => btn.gameObject.SetActive(true));
                }
                //Exit Game
                else if (Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.X))
                {
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.ExitPlaymode();
#endif
                }
            }
            else if (Input.GetKeyDown(KeyCode.BackQuote) && Input.GetKey(KeyCode.LeftControl))
            {
                cheatsActive = true;
                BuildInfoDisplayer bid = FindObjectOfType<BuildInfoDisplayer>();
                bid.buildMessages.Add("CHEATS ACTIVE");
                bid.updateBuildInfoTexts();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
#endif
            }
        }
    }
}
