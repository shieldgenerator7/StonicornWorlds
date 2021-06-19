using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToggleUIButton : ToolButton
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        canClickWhenActive = true;
    }

    protected override void activateImpl()
    {
        bool active = isActive();
        Managers.Input.toolBoxes.ForEach(
            box => box.ShowRow = !active
            );
        Managers.Input.toolBoxes.Find(
            box => box.buttons.Contains(this)
            ).ShowRow = true;
        Managers.Input.updateToolBoxes();
        Managers.Input.checkAllButtons();
    }

    protected override bool isActiveImpl()
        => Managers.Input.toolBoxes.Count(box => box.ShowRow) >= 2;

}
