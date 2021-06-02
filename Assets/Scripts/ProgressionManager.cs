using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressionManager : Manager
{
    public List<ButtonProgressor> buttonProgressors;

    public override void setup()
    {
        //Restore all saved progressed buttons
        Managers.Player.Player.buttonNames.ForEach(
            btnName =>
            {
                ToolButton button = Managers.Input.buttons.First(btn => btn.gameObject.name == btnName);
                button.gameObject.SetActive(true);
            }
            );
        //Check all progression
        checkAllProgression();
    }

    public void checkAllProgression()
    {
        bool anyProgressed = false;
        for (int i = buttonProgressors.Count - 1; i >= 0; i--)
        {
            if (buttonProgressors[i].checkProgression())
            {
                buttonProgressors.RemoveAt(i);
                anyProgressed = true;
            }
        }
        if (anyProgressed)
        {
            onProgressionChanged?.Invoke();
        }
    }
    public delegate void OnProgressionChanged();
    public event OnProgressionChanged onProgressionChanged;
}
