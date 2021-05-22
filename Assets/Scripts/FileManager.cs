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
        ES3.Save<string>("planet", JsonUtility.ToJson(Managers.Planet.Planet), fileName);
        //ES3.Save<List<QueueTask>>("tasks", Managers.Queue.Tasks, fileName);
        Debug.Log("SAVED");
    }

    void LoadFile()
    {
        Debug.Log("LOAD");
        if (ES3.FileExists(fileName))
        {
            Planet planet = JsonUtility.FromJson<Planet>(
                ES3.Load<string>("planet", fileName)
                );
            planet.init();
            Managers.Planet.Planet = planet;
            //Managers.Queue.loadTasks(ES3.Load<List<QueueTask>>("tasks", fileName));
            Debug.Log("LOADED");
        }
    }
}
