using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : Manager
{
    public string fileName = "";

    public bool saveOnExit = true;

    public override void setup()
    {
        LoadFile();
    }
    private void OnDestroy()
    {
        if (saveOnExit)
        {
            SaveFile();
        }
    }

    public void SaveFile()
    {
        Managers.Planet.Planet.tasks = Managers.Queue.Tasks;
        ES3.Save<string>("player", JsonUtility.ToJson(Managers.Player.Player), fileName);
    }

    public void LoadFile()
    {
        if (ES3.FileExists(fileName))
        {
            //0.020 File Format
            if (ES3.KeyExists("player", fileName))
            {
                //Planet
                Player player = JsonUtility.FromJson<Player>(
                    ES3.Load<string>("player", fileName)
                    );
                player.inflate();
                Managers.Player.Player = player;
                //Tasks
                Managers.Queue.loadTasks(Managers.Planet.Planet.tasks);
            }
            //0.019 File Format
            else
            {
                //Planet
                Planet planet = JsonUtility.FromJson<Planet>(
                    ES3.Load<string>("planet", fileName)
                    );
                planet.init();
                Managers.Player.Player.planets.Add(planet);
                Managers.Planet.Planet = planet;
                //Tasks
                Managers.Queue.loadTasks(Managers.Planet.Planet.tasks);
            }
        }
    }
}
