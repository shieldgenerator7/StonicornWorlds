using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearSaveButton : ToolButton
{
    protected override void activateImpl()
    {
        string origFileName = Managers.File.fileName;
        string[] split = origFileName.Split('.');
        Managers.File.fileName = split[0] + "_" + System.DateTime.Now.Ticks + "." + split[1];
        Managers.File.SaveFile();
        Managers.File.DeleteFile(origFileName);
        Managers.File.saveOnExit = false;
        SceneManager.LoadScene(0);
    }

    protected override bool isActiveImpl() => false;
}
