using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearSaveButton : ToolButton
{
    public override void activate()
    {
        string origFileName = Managers.File.fileName;
        string[] split = origFileName.Split('.');
        Managers.File.fileName = split[0] + "_" + System.DateTime.Now.Ticks + "." + split[1];
        Managers.File.SaveFile();
        ES3.DeleteFile(origFileName);
        Managers.File.saveOnExit = false;
        SceneManager.LoadScene(0);
    }

    protected override bool isActive() => false;
}
