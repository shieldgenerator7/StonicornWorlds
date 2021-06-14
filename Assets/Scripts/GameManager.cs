using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public CanvasScaler canvasScaler;

    private long startTime;
    private long setupEndTime;

    void Awake()
    {
        startTime = System.DateTime.Now.Ticks;
        Managers.init();
        registerDelegates();
        setup();
    }

    #region Delegates
    private void registerDelegates()
    {
        Application.quitting += onQuitting;
        //Player
        Managers.Player.onPlayerChanged +=
            (p) => Managers.Planet.Planet = p.LastViewedPlanet;
        //Planet
        Managers.Planet.onPlanetStateChanged +=
            (p) => Managers.Resources.updateResourceCaps(p);
        Managers.Planet.onPlanetStateChangedUnplanned +=
            (p) => Managers.Queue.scheduleTasksFromPlans();
        //Queue
        Managers.Queue.onTaskCompleted += Managers.Planet.updatePlanet;
        //Processor
        Managers.Processor.onFastForwardFinished += onFastForwardFinished;
    }

    void onFastForwardFinished()
    {
        try
        {
            registerUIDelegates();
            Managers.Input.updateToolBoxes();
            setupUI();
            callUIDelegates();
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
        finally
        {
            setupEndTime = System.DateTime.Now.Ticks;
            System.TimeSpan span = new System.TimeSpan(setupEndTime - startTime);
            Debug.Log("Setup time (s): " + span.TotalSeconds);
            Debug.Log("Setup time (m): " + span.TotalMinutes);
        }
    }
    #endregion

    #region UI Delegates
    private void registerUIDelegates()
    {
        //Player
        Managers.Player.onPlayerChanged += onPlayerChangedUI;
        //Planet
        Managers.Planet.onPlanetStateChanged += this.onPlanetStateChangedUI;
        //Camera
        Managers.Camera.onRotationChanged += Managers.PlanetEffects.updateEditDisplay;
        Managers.Camera.onScreenSizeChanged += onScreenSizeChangedUI;
        Managers.Camera.onFocusObjectChanged += onFocusObjectChangedUI;
        Managers.Camera.onZoomChanged += Managers.PlanetEffects.updateSpaceField;
        //Resources
        Managers.Resources.onResourcesChanged +=
            (resources) => Managers.Progression.checkAllProgression();
        //Queue
        Managers.Queue.onQueueChanged += onQueueChangedUI;
        Managers.Queue.onPlansChanged += onPlansChangedUI;
        //Edge
        Managers.Edge.onValidPositionListChanged += onValidPositionListChangedUI;
        //Input
        Managers.Input.onPlanetObjectTypeChanged += onInputPlanetObjectTypeChangedUI;
        Managers.Input.onToolActionChanged += onInputToolActionChangedUI;
        Managers.Input.onMouseOverMoved += Managers.PlanetEffects.updateCursor;
        Managers.Input.onSelectListChanged += Managers.PlanetEffects.updateSelect;
        //Progression
        Managers.Progression.onProgressionChanged += onProgressChangedUI;
    }

    void callUIDelegates()
    {
        //Player
        onPlayerChangedUI(Managers.Player.Player);
        //Planet
        onPlanetStateChangedUI(Managers.Planet.Planet);
        //Camera
        Managers.PlanetEffects.updateEditDisplay(Managers.Camera.transform.up);
        onScreenSizeChangedUI(Camera.main.scaledPixelWidth, Camera.main.scaledPixelHeight);
        if (Managers.Camera.FocusObject)
        {
            onFocusObjectChangedUI(Managers.Camera.FocusObject.stonicorn);
        }
        Managers.PlanetEffects.updateSpaceField(Managers.Camera.ZoomLevel);
        //Resources
        Managers.Progression.checkAllProgression();
        //Queue
        onQueueChangedUI(Managers.Queue.queue);
        onPlansChangedUI(Managers.Queue.plans);
        //Edge
        onValidPositionListChangedUI(Managers.Edge.ValidPosList);
        //Input
        onInputPlanetObjectTypeChangedUI(Managers.Input.PlanetObjectType);
        onInputToolActionChangedUI(Managers.Input.ToolAction);
        Managers.PlanetEffects.updateCursor(Vector2.zero);
        Managers.PlanetEffects.updateSelect(new List<Vector2>() { Vector2.zero });
        //Progression
        onProgressChangedUI();
    }

    void onPlayerChangedUI(Player p)
    {
        Managers.Input.buttons
            .FindAll(btn => p.lastActiveButtonNames.Contains(btn.gameObject.name))
            .ForEach(btn => btn.activate());
    }

    void onPlanetStateChangedUI(Planet p)
    {
        Managers.Progression.checkAllProgression();
        Managers.PlanetEffects.updateDisplay(p);
    }

    void onScreenSizeChangedUI(int width, int height)
    {
        canvasScaler.matchWidthOrHeight = (width > height) ? 0 : 1;
        Managers.Constants.updateScreenConstants(width, height);
    }

    void onFocusObjectChangedUI(Stonicorn stonicorn)
    {
        Managers.PlanetEffects.updateStonicornInfo(stonicorn);
        Managers.Input.checkAllButtons();
    }

    void onQueueChangedUI(List<QueueTask> tasks)
    {
        Managers.QueueEffects.updateDisplay(tasks);
    }
    void onPlansChangedUI(Planet plans)
    {
        Managers.Edge.calculateValidPosList(plans);
    }

    void onInputPlanetObjectTypeChangedUI(PlanetObjectType pot)
    {
        Managers.Edge.calculateValidPosList(Managers.Queue.plans);
        Managers.Input.checkAllButtons();
    }
    void onInputToolActionChangedUI(ToolAction ta)
    {
        Managers.Edge.calculateValidPosList(Managers.Queue.plans);
        Managers.Input.checkAllButtons();
    }

    void onValidPositionListChangedUI(List<Vector2> edges)
    {
        Managers.PlanetEffects.updateEditDisplay(edges);
        Managers.Camera.autoFrame(edges);
    }

    void onProgressChangedUI()
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
        Managers.Player.setup();
        Managers.Planet.setup();
        Managers.File.setup();
        Managers.Queue.setup();
        Managers.Resources.setup();
        Managers.Processor.setup();
    }

    void setupUI()
    {
        Managers.Constants.setup();
        Managers.Camera.setup();
        Managers.Input.setup();
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
