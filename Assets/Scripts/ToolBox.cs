using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBox : ToolButton
{
    public int index = 0;
    public List<ToolButton> buttons;
    public float spacing = 100;

    public void organize()
    {
        organize(index);
    }
    /// <summary>
    /// Arranges the buttons in their positions.
    /// </summary>
    /// <param name="index">The index of this toolbox, starting from 0 at the bottom left</param>
    public void organize(int index)
    {
        float x = spacing / 2;
        float y = spacing / 2 + (index * spacing);
        setPosition(x, y);
        x += spacing;
        buttons.FindAll(b => b.gameObject.activeSelf)
            .ForEach(btn =>
            {
                btn.setPosition(x, y);
                x += spacing;
            });
    }

    public override void activate()
    {
    }

    protected override bool isActive()
    {
        return false;
    }
}
