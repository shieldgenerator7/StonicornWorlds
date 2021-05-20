using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public string fileName = "planet1.wrld";

    // Start is called before the first frame update
    void Start()
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
        ES3.Save<Planet>("planet", Managers.Planet.Planet, fileName);
        //ES3.Save<List<QueueTask>>("tasks", Managers.Queue.Tasks, fileName);
        Debug.Log("SAVED");
    }

    void LoadFile()
    {
        Debug.Log("LOAD");
        if (ES3.FileExists(fileName))
        {
            Managers.Planet.Planet = ES3.Load<Planet>("planet", fileName);
            //Managers.Queue.loadTasks(ES3.Load<List<QueueTask>>("tasks", fileName));
            Debug.Log("LOADED");
        }
    }
}
