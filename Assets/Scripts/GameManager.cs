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
        Managers.Planet.onFuturePlanetStateChanged += (fp) => Managers.Edge.calculateValidPosList();
        Managers.Planet.onResourcesChanged += (resources) => Managers.Progression.checkAllProgression();
        //Queue
        Managers.Queue.onTaskCompleted += Managers.Planet.updatePlanet;
        Managers.Queue.onQueueChanged += Managers.Planet.calculateFutureState;
        //Edge
        Managers.Edge.onValidPositionListChanged += Managers.Camera.autoFrame;
        //Input
        Managers.Input.onPlanetObjectTypeChanged += onInputPlanetObjectTypeChanged;
        Managers.Input.onToolActionChanged += onInputToolActionChanged;
        //Progression
        Managers.Progression.onProgressionChanged += onProgressChanged;
    }

    void onPlanetStateChanged(Planet p)
    {
        Managers.Planet.calculateFutureState(Managers.Queue.Tasks);
        Managers.Queue.updateQueueWorkerList(p);
        Managers.Progression.checkAllProgression();
    }

    void onInputPlanetObjectTypeChanged(PlanetObjectType pot)
    {
        Managers.Edge.calculateValidPosList();
        Managers.Input.checkAllButtons();
    }
    void onInputToolActionChanged(ToolAction ta)
    {
        Managers.Edge.calculateValidPosList();
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
        Managers.File.setup();
        Managers.Planet.setup();
        Managers.Input.setup();
        Managers.Progression.setup();
    }
    #endregion
}
