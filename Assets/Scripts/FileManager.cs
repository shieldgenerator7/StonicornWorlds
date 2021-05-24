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
        ES3.Save<string>("planet", JsonUtility.ToJson(Managers.Planet.Planet), fileName);
    }

    public void LoadFile()
    {
        if (ES3.FileExists(fileName))
        {
            //Planet
            Planet planet = JsonUtility.FromJson<Planet>(
                ES3.Load<string>("planet", fileName)
                );
            planet.init();
            Managers.Planet.Planet = planet;
            //Tasks
            Managers.Queue.loadTasks(Managers.Planet.Planet.tasks);
        }
    }
}
