using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public string fileName = "planet1.wrld";

    public void setup()
    {
        LoadFile();
    }
    private void OnDestroy()
    {
        SaveFile();
    }

    void SaveFile()
    {
        Debug.Log("SAVE");
        Managers.Planet.Planet.tasks = Managers.Queue.Tasks;
        ES3.Save<string>("planet", JsonUtility.ToJson(Managers.Planet.Planet), fileName);
        ES3.Save<float>("resources", Managers.Planet.Resources, fileName);
        Debug.Log("SAVED");
    }

    void LoadFile()
    {
        Debug.Log("LOAD");
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
            //Resources
            float resources = ES3.Load<float>("resources", fileName);
            Managers.Planet.Resources = resources;
            Debug.Log("LOADED");
        }
    }
}
