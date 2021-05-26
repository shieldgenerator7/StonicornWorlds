using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public List<Planet> planets = new List<Planet>();

    public List<string> buttonNames = new List<string>();

    [SerializeField]
    private int lastViewedPlanetIndex = 0;
    public Planet LastViewedPlanet
    {
        get => planets[lastViewedPlanetIndex];
        set
        {
            lastViewedPlanetIndex = Mathf.Clamp(
                planets.IndexOf(value),
                0,
                planets.Count - 1
                );
        }
    }

    public List<string> lastActiveButtonNames = new List<string>();


    public void inflate()
    {
        planets.ForEach(planet => planet.inflate());
    }
}
