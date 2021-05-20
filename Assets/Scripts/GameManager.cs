using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Managers.init();
        registerDelegates();
        setup();
    }

    // Update is called once per frame
    void Update()
    {

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
        //Queue
        Managers.Queue.onTaskCompleted += Managers.Planet.updatePlanet;
        Managers.Queue.onQueueChanged += Managers.Planet.calculateFutureState;
        Managers.Queue.onQueueChanged += Managers.QueueEffects.updateDisplay;
        //Edge
        Managers.Edge.onValidPositionListChanged += Managers.Camera.autoFrame;
        Managers.Edge.onValidPositionListChanged += Managers.PlanetEffects.updateEditDisplay;
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

    void onProgressChanged()
    {
        Managers.Input.updateToolBoxes();
        Managers.Input.checkAllButtons();
    }
    #endregion

    #region Setup
    void setup()
    {
        Managers.Input.setup();
        //Managers.File.setup();
        Managers.Planet.setup();
        Managers.Edge.calculateValidPosList(Managers.Planet.FuturePlanet);
        Managers.Progression.setup();
        Managers.PlanetEffects.updateEditDisplay(Managers.Edge.ValidPosList);
    }
    #endregion
}
