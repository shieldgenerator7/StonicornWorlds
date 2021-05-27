using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonicornGenerator : MonoBehaviour
{
    [Header("Appearance")]
    public List<Color> bodyColors;
    public List<Color> hairColors;

    [Header("Stats")]
    public float minWorkRate = 20;
    public float maxWorkRate = 20;
    public float minMoveSpeed = 2;
    public float maxMoveSpeed = 2;
    public float minRestRate = 40;
    public float maxRestRate = 40;
    public float minRestCap = 1000;
    public float maxRestCap = 1000;

    [Header("Profiles")]
    public List<Stonicorn> profiles;

    public Stonicorn generate()
    {
        Stonicorn stonicorn = new Stonicorn();
        //Appearance
        stonicorn.bodyColor = bodyColors[Random.Range(0, bodyColors.Count)];
        stonicorn.hairColor = hairColors[Random.Range(0, hairColors.Count)];
        //Stats
        stonicorn.workRate = Random.Range(minWorkRate, maxWorkRate);
        stonicorn.moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        stonicorn.restSpeed = Random.Range(minRestRate, maxRestRate);
        stonicorn.maxRest = Random.Range(minRestCap, maxRestCap);
        //
        return stonicorn;
    }

    public void statsFromProfile(Stonicorn stonicorn, int profileIndex)
    {
        Stonicorn model = profiles[profileIndex];
        stonicorn.workRate = model.workRate;
        stonicorn.moveSpeed = model.moveSpeed;
        stonicorn.restSpeed = model.restSpeed;
        stonicorn.maxRest = model.maxRest;
    }
}
