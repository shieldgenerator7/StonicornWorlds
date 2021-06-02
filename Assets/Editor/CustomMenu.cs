using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Debug = UnityEngine.Debug;
using System.Linq;
using UnityEditor.SceneManagement;
using System;
using System.Reflection;

public class CustomMenu
{

    #region Editor Menu


    //Find Missing Scripts
    //2018-04-13: copied from http://wiki.unity3d.com/index.php?title=FindMissingScripts
    static int go_count = 0, components_count = 0, missing_count = 0;
    [MenuItem("SG7/Editor/Refactor/Find Missing Scripts")]
    private static void FindMissingScripts()
    {
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.isLoaded)
            {
                foreach (GameObject go in s.GetRootGameObjects())
                {
                    FindInGO(go);
                }
            }
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }
    private static void FindInGO(GameObject g)
    {
        go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }
                Debug.Log(s + " has an empty script attached in position: " + i, g);
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            FindInGO(childT.gameObject);
        }
    }

    [MenuItem("SG7/Editor/Print Planet JSON")]
    private static void printPlanetJSON()
    {
        if (Application.isPlaying)
        {
            string json = JsonUtility.ToJson(Managers.Planet.Planet);
            Debug.Log("json: " + json);
        }
        else
        {
            Debug.LogError("Enter play mode to Print Planet JSON");
        }
    }
    [MenuItem("SG7/Editor/Print Planet Copy JSON")]
    private static void printPlanetCopyJSON()
    {
        if (Application.isPlaying)
        {
            string json = JsonUtility.ToJson(Managers.Planet.Planet.deepCopy());
            Debug.Log("json copy: " + json);
        }
        else
        {
            Debug.LogError("Enter play mode to Print Planet Copy JSON");
        }
    }

    [MenuItem("SG7/Editor/Slow Down #,")]
    private static void slowDown()
    {
        if (Application.isPlaying)
        {
            GameObject.FindObjectOfType<ResourceGenerator>().generateRate /= 2;
            Managers.Queue.defaultWorkRate /= 2;
            Managers.Planet.Planet.residents
                .ForEach(qw => qw.workRate /= 2);
        }
        else
        {
            Debug.LogError("Enter play mode to Slow Down");
        }
    }

    [MenuItem("SG7/Editor/Speed Up #.")]
    private static void speedUp()
    {
        if (Application.isPlaying)
        {
            GameObject.FindObjectOfType<ResourceGenerator>().generateRate *= 2;
            Managers.Queue.defaultWorkRate *= 2;
            Managers.Planet.Planet.residents
                .ForEach(qw => qw.workRate *= 2);
        }
        else
        {
            Debug.LogError("Enter play mode to Speed Up");
        }
    }
    #endregion

    #region Prebuild Submenu
    [MenuItem("SG7/Build/Prebuild/Run All Prebuild Tasks &w")]
    public static int runAllPrebuildTasks()
    {
        ClearLogConsole();
        Debug.Log("=== Prebuild tasks starting ===");
        int problemCount = 0;
        //
        problemCount += setupInputManager();
        problemCount += setupPodTypeBank();
        problemCount += checkProgressionManager();
        //
        if (problemCount == 0)
        {
            Debug.Log("=== Prebuild tasks finished ===");
        }
        else
        {
            Debug.Log("=== Prebuild tasks: " + problemCount + " problems.");
        }
        return problemCount;
    }
    [MenuItem("SG7/Build/Prebuild/Setup InputManager")]
    public static int setupInputManager()
    {
        int problemCount = 0;
        InputManager inputManager = GameObject.FindObjectOfType<InputManager>();
        inputManager.buttons = GameObject.FindObjectsOfType<ToolButton>(true).ToList()
            .FindAll(btn => !(btn is ToolBox));
        GameObject.FindObjectsOfType<ToolBox>(true).ToList()
            .FindAll(tb => !inputManager.toolBoxes.Contains(tb))
            .ForEach(tb =>
            {
                problemCount++;
                Debug.LogError("Tool " + tb + " is not listed in InputManager!", tb);
            });
        inputManager.buttons
            .FindAll(btn => !inputManager.toolBoxes.Any(tb => tb.buttons.Contains(btn)))
            .ForEach(btn =>
            {
                problemCount++;
                Debug.LogError("Button " + btn + " is not listed in any registered ToolBox!", btn);
            });
        inputManager.tools = GameObject.FindObjectsOfType<Tool>(true).ToList();
        EditorUtility.SetDirty(inputManager);
        Debug.Log("InputManager setup.", inputManager);
        return problemCount;
    }
    [MenuItem("SG7/Build/Prebuild/Setup PodTypeBank")]
    public static int setupPodTypeBank()
    {
        ConstantBank podTypeBank = GameObject.FindObjectOfType<ConstantBank>();
        podTypeBank.allPodTypes = Resources.FindObjectsOfTypeAll<PodType>().ToList();
        podTypeBank.allPodContentTypes = Resources.FindObjectsOfTypeAll<PodContentType>().ToList();
        EditorUtility.SetDirty(podTypeBank);
        Debug.Log("PodTypeBank setup.", podTypeBank);
        return 0;
    }
    [MenuItem("SG7/Build/Prebuild/Check ProgressionManager")]
    public static int checkProgressionManager()
    {
        int problemCount = 0;
        InputManager inputManager = GameObject.FindObjectOfType<InputManager>();
        ProgressionManager progressionManager = GameObject.FindObjectOfType<ProgressionManager>();
        progressionManager.buttonProgressors = GameObject.FindObjectsOfType<ButtonProgressor>(true).ToList();
        //Try to fill in empty button progressors
        progressionManager.buttonProgressors
            .FindAll(btnpro => btnpro.button == null)
            .ForEach(btnpro =>
            {
                //like "Pod Core"
                string subName = btnpro.gameObject.name.Split(' ')[1];
                btnpro.button = GameObject.FindObjectsOfType<ToolButton>(true)
                    //like "btn Core"
                    .FirstOrDefault(btn => subName == btn.gameObject.name.Split(' ')[1]);
                if (btnpro.button != null)
                {
                    Debug.LogWarning("Put button " + btnpro.button.name + " in button progressor " + btnpro.name, btnpro);
                }
            });
        //Check to make sure buttons are registered
        inputManager.buttons
            .FindAll(
            btn => !progressionManager.buttonProgressors.Any(
                    btnpro => btnpro.button == btn
                    )
            )
            .ForEach(btn =>
            {
                problemCount++;
                Debug.LogError("Button " + btn + " is not registered in ProgressManager!", btn);
            });
        //Check to make sure no button is registered twice (or more)
        progressionManager.buttonProgressors
            .FindAll(
            btnpro => progressionManager.buttonProgressors.Any(
                br => br != btnpro && br.button == btnpro.button
                )
            )
            .ForEach(btnpro =>
            {
                problemCount++;
                Debug.LogError("Button " + btnpro.button
                + " is registered more than once in ProgressManager!", progressionManager);
            });
        //Set active buttons to inactive
        inputManager.buttons.FindAll(btn => btn.gameObject.activeSelf)
            .ForEach(btn =>
            {
                btn.gameObject.SetActive(false);
                Debug.LogWarning("Set button inactive: " + btn, btn);
            });
        Debug.Log("ProgressionManager checked.", progressionManager);
        return problemCount;
    }

    //2021-05-25: copied from http://answers.unity.com/answers/1318322/view.html
    public static void ClearLogConsole()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
    }
    #endregion

    #region Build / Run Menu

    [MenuItem("SG7/Build/Build Windows %w")]
    public static void buildWindows()
    {
        build(BuildTarget.StandaloneWindows, "exe");
    }
    [MenuItem("SG7/Build/Build Linux")]
    public static void buildLinux()
    {
        Debug.LogError(
            "Building Linux has not been readded yet after Unity removed it in 2019.2"
            );
    }
    [MenuItem("SG7/Build/Build Mac OS X")]
    public static void buildMacOSX()
    {
        build(BuildTarget.StandaloneOSX, "");
    }
    public static void build(BuildTarget buildTarget, string extension)
    {
        string defaultPath = getDefaultBuildPath();
        if (!System.IO.Directory.Exists(defaultPath))
        {
            System.IO.Directory.CreateDirectory(defaultPath);
        }
        //2017-10-19 copied from https://docs.unity3d.com/Manual/BuildPlayerPipeline.html
        // Get filename.
        string buildName = EditorUtility.SaveFilePanel("Choose Location of Built Game", defaultPath, PlayerSettings.productName, extension);

        // User hit the cancel button.
        if (buildName == "")
            return;

        string path = buildName.Substring(0, buildName.LastIndexOf("/"));
        Debug.Log("BUILDNAME: " + buildName);
        Debug.Log("PATH: " + path);

        string[] levels = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            if (EditorBuildSettings.scenes[i].enabled)
            {
                levels[i] = EditorBuildSettings.scenes[i].path;
            }
            else
            {
                break;
            }
        }

        // Build player.
        BuildPipeline.BuildPlayer(levels, buildName, buildTarget, BuildOptions.None);

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = buildName;
        proc.Start();
    }

    [MenuItem("SG7/Run/Run Windows %#w")]
    public static void runWindows()
    {//2018-08-10: copied from build()
        string extension = "exe";
        string buildName = getBuildNamePath(extension);
        Debug.Log("Launching: " + buildName);
        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = buildName;
        proc.Start();
    }

    [MenuItem("SG7/Run/Open Build Folder #w")]
    public static void openBuildFolder()
    {
        string extension = "exe";
        string buildName = getBuildNamePath(extension);
        //Open the folder where the game is located
        EditorUtility.RevealInFinder(buildName);
    }

    [MenuItem("SG7/Run/Open App Data Folder &f")]
    public static void openAppDataFolder()
    {
        string filePath = Application.persistentDataPath + "/merky.txt";
        if (System.IO.File.Exists(filePath))
        {
            EditorUtility.RevealInFinder(filePath);
        }
        else
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }

    public static string getDefaultBuildPath()
    {
        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Unity/StonicornWorlds/Builds/" + PlayerSettings.productName + "_" + PlayerSettings.bundleVersion.Replace(".", "_");
    }
    public static string getBuildNamePath(string extension, bool checkFolderExists = true)
    {
        string defaultPath = getDefaultBuildPath();
        if (checkFolderExists && !System.IO.Directory.Exists(defaultPath))
        {
            throw new UnityException("You need to build the " + extension + " for " + PlayerSettings.productName + " (Version " + PlayerSettings.bundleVersion + ") first!");
        }
        string buildName = defaultPath + "/" + PlayerSettings.productName + "." + extension;
        return buildName;
    }
    #endregion

    #region Session / Upgrade Menu

    [MenuItem("SG7/Session/Begin Session")]
    public static void beginSession()
    {
        Debug.Log("=== Beginning session ===");
        string oldVersion = PlayerSettings.bundleVersion;
        string[] split = oldVersion.Split('.');
        string newNum = "" + (int.Parse(split[1]) + 1);
        while (newNum.Length < 3)
        {
            newNum = "0" + newNum;
        }
        string newVersion = split[0] + "." + newNum;
        PlayerSettings.bundleVersion = newVersion;
        //Save and Log
        EditorSceneManager.SaveOpenScenes();
        Debug.LogWarning("Updated build version number from " + oldVersion + " to " + newVersion);
    }

    [MenuItem("SG7/Session/Finish Session")]
    public static void finishSession()
    {
        Debug.Log("=== Finishing session ===");
        int problemCount = runAllPrebuildTasks();
        if (problemCount == 0)
        {
            EditorSceneManager.SaveOpenScenes();
            buildWindows();
        }
    }

    [MenuItem("SG7/Upgrade/Force save all assets")]
    public static void forceSaveAllAssets()
    {
        AssetDatabase.ForceReserializeAssets();
    }
    #endregion
}
