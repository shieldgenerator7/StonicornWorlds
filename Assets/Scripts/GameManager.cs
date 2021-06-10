using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public CanvasScaler canvasScaler;

    private bool screenChangedLastFrame = false;

    void Awake()
    {
        Managers.init();
        registerDelegates();
        setup();
        if (Managers.Planet.Planet.residents.Count > 1)
        {
            Managers.Camera.FocusObject = Managers.PlanetEffects
                .stonicorns[Managers.Planet.Planet.residents[0]]
                .GetComponent<StonicornDisplayer>();
        }
        else
        {
            Managers.Camera.FocusObject = null;
            Managers.PlanetEffects.updateStonicornInfo(Managers.Planet.Planet.residents[0]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (screenChangedLastFrame)
        {
            screenChangedLastFrame = false;
            Managers.Input.updateToolBoxes();
        }
    }

    #region Delegates
    private void registerDelegates()
    {
        Application.quitting += onQuitting;
        //Player
        Managers.Player.onPlayerChanged += onPlayerChanged;
        //Planet
        Managers.Planet.onPlanetStateChanged += this.onPlanetStateChanged;
        //Camera
        Managers.Camera.onRotationChanged += Managers.PlanetEffects.updateEditDisplay;
        Managers.Camera.onScreenSizeChanged += onScreenSizeChanged;
        Managers.Camera.onFocusObjectChanged += onFocusObjectChanged;
        Managers.Camera.onZoomChanged += Managers.PlanetEffects.updateSpaceField;
        //Resources
        Managers.Resources.onResourcesChanged += (resources) => Managers.Progression.checkAllProgression();
        //Queue
        Managers.Queue.onTaskCompleted += Managers.Planet.updatePlanet;
        Managers.Queue.onQueueChanged += onQueueChanged;
        Managers.Queue.onPlansChanged += onPlansChanged;
        //Edge
        Managers.Edge.onValidPositionListChanged += onValidPositionListChanged;
        //Input
        Managers.Input.onPlanetObjectTypeChanged += onInputPlanetObjectTypeChanged;
        Managers.Input.onToolActionChanged += onInputToolActionChanged;
        Managers.Input.onMouseOverMoved += Managers.PlanetEffects.updateCursor;
        Managers.Input.onSelectListChanged += Managers.PlanetEffects.updateSelect;
        //Progression
        Managers.Progression.onProgressionChanged += onProgressChanged;
    }

    void onPlayerChanged(Player p)
    {
        Managers.Planet.Planet = p.LastViewedPlanet;
        Managers.Input.buttons
            .FindAll(btn => p.lastActiveButtonNames.Contains(btn.gameObject.name))
            .ForEach(btn => btn.activate());
    }

    void onPlanetStateChanged(Planet p)
    {
        Managers.Resources.updateResourceCaps(p);
        //Managers.Queue.scheduleTasksFromPlans();
        Managers.Progression.checkAllProgression();
        Managers.PlanetEffects.updateDisplay(p);
    }

    void onScreenSizeChanged(int width, int height)
    {
        canvasScaler.matchWidthOrHeight = (width > height) ? 0 : 1;
        Managers.Constants.updateScreenConstants(width, height);
        screenChangedLastFrame = true;
    }

    void onFocusObjectChanged(Stonicorn stonicorn)
    {
        Managers.PlanetEffects.updateStonicornInfo(stonicorn);
        Managers.Input.checkAllButtons();
    }

    void onQueueChanged(List<QueueTask> tasks)
    {
        Managers.QueueEffects.updateDisplay(tasks);
    }
    void onPlansChanged(Planet plans)
    {
        Managers.Edge.calculateValidPosList(plans);
    }

    void onInputPlanetObjectTypeChanged(PlanetObjectType pot)
    {
        Managers.Edge.calculateValidPosList(Managers.Queue.plans);
        Managers.Input.checkAllButtons();
    }
    void onInputToolActionChanged(ToolAction ta)
    {
        Managers.Edge.calculateValidPosList(Managers.Queue.plans);
        Managers.Input.checkAllButtons();
    }

    void onValidPositionListChanged(List<Vector2> edges)
    {
        Managers.PlanetEffects.updateEditDisplay(edges);
        Managers.Camera.autoFrame(edges);
    }

    void onProgressChanged()
    {
        Managers.Input.updateToolBoxes();
        Managers.Input.checkAllButtons();
        Managers.Player.Player.updateButtonNames(
            Managers.Input.buttons
                .FindAll(btn => btn.gameObject.activeSelf)
                .ConvertAll(btn => btn.gameObject.name)
            );
    }
    #endregion

    #region Setup
    void setup()
    {
        Managers.Constants.setup();
        Managers.Camera.setup();
        Managers.Input.setup();
        Managers.Player.setup();
        Managers.Planet.setup();
        Managers.File.setup();
        Managers.Queue.setup();
        Managers.Resources.setup();
        Managers.Edge.calculateValidPosList(Managers.Queue.plans);
        Managers.Progression.setup();
    }
    #endregion

    #region Destroy
    void onQuitting()
    {
        Managers.Player.prepareForSave();
        if (Managers.File.saveOnExit)
        {
            Managers.File.SaveFile();
        }
    }
    #endregion
}
