using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonicornGenerator : MonoBehaviour
{
    [Header("Appearance")]
    public List<Color> bodyColors;
    public List<Color> hairColors;
    public List<Color> eyeColors;

    [Header("Stats")]
    public ValueGenerator workRateGen;
    public ValueGenerator workRangeGen;
    public ValueGenerator moveSpeedGen;
    public ValueGenerator restRateGen;
    public ValueGenerator restCapGen;
    public ValueGenerator passiveExhaustGen;
    public ValueGenerator toolbeltGen;
    public ValueGenerator transferRateGen;

    [Header("Personal Data")]
    public List<string> names;

    [Header("Profiles")]
    public List<Stonicorn> profiles;

    public Stonicorn generate()
    {
        Stonicorn stonicorn = new Stonicorn();
        //Appearance
        stonicorn.bodyColor = bodyColors[Random.Range(0, bodyColors.Count)];
        stonicorn.hairColor = hairColors[Random.Range(0, hairColors.Count)];
        stonicorn.eyeColor = eyeColors[Random.Range(0, eyeColors.Count)];
        //Stats
        stonicorn.workRate = workRateGen.generate();
        stonicorn.workRange = workRangeGen.generate();
        stonicorn.moveSpeed = moveSpeedGen.generate();
        stonicorn.restSpeed = restRateGen.generate();
        stonicorn.maxRest = restCapGen.generate();
        stonicorn.passiveExhaustRate = passiveExhaustGen.generate();
        stonicorn.toolbeltSize = toolbeltGen.generate();
        stonicorn.transferRate = transferRateGen.generate();
        //Personal Data
        string firstName = names[Random.Range(0, names.Count)];
        string lastName = names[Random.Range(0, names.Count)];
        stonicorn.name = firstName + " " + lastName;
        int enumCountTask = System.Enum.GetValues(typeof(Stonicorn.TaskPriority)).Length;
        while (!taskPriorityCompatible(stonicorn.taskPriority, stonicorn.taskPriority2))
        {
            stonicorn.taskPriority = (Stonicorn.TaskPriority)Random.Range(0, enumCountTask);
            stonicorn.taskPriority2 = (Stonicorn.TaskPriority)Random.Range(0, enumCountTask);
        }
        int enumCountJob = System.Enum.GetValues(typeof(QueueTask.Type)).Length;
        stonicorn.favoriteJobType = (QueueTask.Type)Random.Range(0, enumCountJob);
        //
        return stonicorn;
    }

    public void statsFromProfile(Stonicorn stonicorn, int profileIndex)
    {
        Stonicorn model = profiles[profileIndex];
        stonicorn.workRate = model.workRate;
        stonicorn.workRange = model.workRange;
        stonicorn.moveSpeed = model.moveSpeed;
        stonicorn.restSpeed = model.restSpeed;
        stonicorn.passiveExhaustRate = model.passiveExhaustRate;
        stonicorn.toolbeltSize = model.toolbeltSize;
        stonicorn.transferRate = model.transferRate;
        stonicorn.maxRest = model.maxRest;
        stonicorn.taskPriority = model.taskPriority;
        stonicorn.taskPriority2 = model.taskPriority2;
    }

    bool taskPriorityCompatible(Stonicorn.TaskPriority tp1, Stonicorn.TaskPriority tp2)
    {
        //if they are the same
        if (tp1 == tp2)
        {
            return false;
        }
        //if they are opposites
        int itp1 = (int)tp1;
        int itp2 = (int)tp2;
        if (itp1 % 2 == 0 && itp2 == itp1 + 1)
        {
            return false;
        }
        if (itp2 % 2 == 0 && itp1 == itp2 + 1)
        {
            return false;
        }
        //Else, they are compatible
        return true;
    }
}
