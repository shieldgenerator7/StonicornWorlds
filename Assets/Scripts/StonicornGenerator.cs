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
        //Personal Data
        string firstName = names[Random.Range(0, names.Count)];
        string lastName = names[Random.Range(0, names.Count)];
        stonicorn.name = firstName + " " + lastName;
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
        stonicorn.maxRest = model.maxRest;
    }
}
