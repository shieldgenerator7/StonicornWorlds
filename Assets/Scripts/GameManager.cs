using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private bool screenChangedLastFrame = false;

    void Awake()
    {
        Managers.init();
        registerDelegates();
        setup();
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
        //Planet
        Managers.Planet.onPlanetStateChanged += this.onPlanetStateChanged;
        Managers.Planet.onFuturePlanetStateChanged += Managers.Edge.calculateValidPosList;
        Managers.Planet.onResourcesChanged += (resources) => Managers.Progression.checkAllProgression();
        Managers.Planet.onResourcesChanged += Managers.PlanetEffects.updateUI;
        //Camera
        Managers.Camera.onRotationChanged += Managers.PlanetEffects.updateEditDisplay;
        Managers.Camera.onScreenSizeChanged += onScreenSizeChanged;
        //Queue
        Managers.Queue.onTaskCompleted += Managers.Planet.updatePlanet;
        Managers.Queue.onQueueChanged += Managers.Planet.calculateFutureState;
        Managers.Queue.onQueueChanged += Managers.QueueEffects.updateDisplay;
        //Edge
        Managers.Edge.onValidPositionListChanged += onValidPositionListChanged;
        //Input
        Managers.Input.onPlanetObjectTypeChanged += onInputPlanetObjectTypeChanged;
        Managers.Input.onToolActionChanged += onInputToolActionChanged;
        Managers.Input.onMouseOverMoved += Managers.PlanetEffects.updateCursor;
        //Progression
        Managers.Progression.onProgressionChanged += onProgressChanged;
    }

    void onPlanetStateChanged(Planet p)
    {
        Managers.Planet.calculateFutureState(Managers.Queue.Tasks);
        Managers.Queue.updateQueueWorkerList(p);
        Managers.Progression.checkAllProgression();
        Managers.PlanetEffects.updateDisplay(p);
    }

    void onScreenSizeChanged(int width, int height)
    {
        Managers.Constants.updateScreenConstants(width, height);
        screenChangedLastFrame = true;
    }

    void onInputPlanetObjectTypeChanged(PlanetObjectType pot)
    {
        Managers.Edge.calculateValidPosList(Managers.Planet.FuturePlanet);
        Managers.Input.checkAllButtons();
    }
    void onInputToolActionChanged(ToolAction ta)
    {
        Managers.Edge.calculateValidPosList(Managers.Planet.FuturePlanet);
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
    }
    #endregion

    #region Setup
    void setup()
    {
        Managers.Constants.setup();
        Managers.Camera.setup();
        Managers.Input.setup();
        //Managers.File.setup();
        Managers.Planet.setup();
        Managers.Edge.calculateValidPosList(Managers.Planet.FuturePlanet);
        Managers.Progression.setup();
    }
    #endregion
}
