using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureManager : Manager
{
    //2021-07-12: copied from Stonicorn.GestureManager

    //Gesture Profiles
    public enum GestureProfileType { MAIN };
    private GestureProfile currentGP;//the current gesture profile
    private Dictionary<GestureProfileType, GestureProfile> gestureProfiles = new Dictionary<GestureProfileType, GestureProfile>();//dict of valid gesture profiles

    /// <summary>
    /// The input that is currently providing input or that has most recently provided input
    /// </summary>
    private GestureInput activeInput;
    public GestureInput ActiveInput
    {
        get => activeInput;
        set
        {
            GestureInput prevInput = activeInput;
            activeInput = value ?? prevInput;
            if (activeInput != prevInput)
            {
                onInputDeviceSwitched?.Invoke(activeInput.InputType);
                Debug.Log("ActiveInput is now: " + activeInput.InputType);
            }
        }
    }
    public delegate void OnInputDeviceSwitched(InputDeviceMethod inputDevice);
    public event OnInputDeviceSwitched onInputDeviceSwitched;
    private List<GestureInput> gestureInputs;

    // Use this for initialization
    public override void setup()
    {
        gestureProfiles.Add(GestureProfileType.MAIN, new GestureProfile());
        switchGestureProfile(GestureProfileType.MAIN);

        Input.simulateMouseWithTouches = false;

        //Inputs
        gestureInputs = new List<GestureInput>();
        gestureInputs.Add(new TouchGestureInput());
        gestureInputs.Add(new MouseGestureInput());
        //Default active input
        ActiveInput = gestureInputs.Find(input => input.InputSupported);
    }

    // Update is called once per frame
    public override void update(float timeDelta)
    {
        //
        //Input Processing
        //
        bool processed = activeInput.processInput(currentGP);
        if (!processed)
        {
            GestureInput prevInput = activeInput;
            ActiveInput = gestureInputs.Find(input => input.InputOngoing);
            if (activeInput != prevInput)
            {
                activeInput.processInput(currentGP);
            }
        }

        //
        // Exiting
        //
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
        }
    }

    /// <summary>
    /// Switches the gesture profile to the profile with the given name
    /// </summary>
    /// <param name="gpName">The name of the GestureProfile</param>
    public void switchGestureProfile(GestureProfileType gpt)
    {
        GestureProfile newGP = gestureProfiles[gpt];
        //If the gesture profile is not already active,
        if (newGP != currentGP)
        {
            //Deactivate current
            if (currentGP != null)
            {
                currentGP.deactivate();
            }
            //Switch from current to new
            currentGP = newGP;
            //Activate new
            currentGP.activate();
        }
    }
}
