using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : Manager
{
    public string fileName = "";

    public bool saveOnExit = true;
    public bool loadOnStart = true;

    public override void setup()
    {
        if (loadOnStart)
        {
            LoadFile();
        }
    }

    private string FullFileName
        => Application.persistentDataPath + "/" + fileName;

    public bool FileExists
        => File.Exists(FullFileName);

    public void SaveFile()
    {
        Managers.Player.prepareForSave();
        File.WriteAllText(
            FullFileName, 
            JsonUtility.ToJson(Managers.Player.Player)
            );
    }

    public void LoadFile()
    {
        if (FileExists)
        {
            string saveData = File.ReadAllText(FullFileName);
            ////pre-0.027 File Format
            //if (saveData.Contains("__type"))
            //{
            //    //0.020 File Format
            //    if (ES3.KeyExists("player", fileName))
            //    {
            //        //Planet
            //        Player player = JsonUtility.FromJson<Player>(
            //            ES3.Load<string>("player", fileName)
            //            );
            //        player.inflate();
            //        Managers.Player.Player = player;
            //    }
            //    //0.019 File Format
            //    else
            //    {
            //        //Planet
            //        Planet planet = JsonUtility.FromJson<Planet>(
            //            ES3.Load<string>("planet", fileName)
            //            );
            //        planet.inflate();
            //        Managers.Player.Player.planets.Add(planet);
            //        Managers.Planet.Planet = planet;
            //    }
            //}
            //0.027 File Format
            //else
            //{
            Player player = JsonUtility.FromJson<Player>(saveData);
            player.inflate();
            Managers.Player.Player = player;
            //}
        }
    }

    public void DeleteFile()
    {
        DeleteFile(this.fileName);
    }

    public void DeleteFile(string filename)
    {
        string filePath = Application.persistentDataPath + "/" + filename;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
