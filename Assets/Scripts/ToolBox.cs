using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBox : ToolButton
{
    public int indexY = 0;
    public List<ToolButton> buttons;
    public float spacing = 100;

    public bool ShowSelf => false;

    public void organize()
    {
        organize(indexY);
    }
    /// <summary>
    /// Arranges the buttons in their positions.
    /// </summary>
    /// <param name="indexY">The index of this toolbox, starting from 0 at the bottom left</param>
    public void organize(int indexY)
    {
        float offsetX = spacing / 2;
        float spacingX = 7 * spacing / 8;
        float y = spacing / 2 + (indexY * spacing);
        float y2 = spacing / 2 + y;
        int indexX = 0;
        if (ShowSelf)
        {
            setPosition(
                spacingX * indexX + offsetX,
                (indexX % 2 == 0) ? y : y2
                );
            indexX++;
        }
        else
        {
            setPosition(-spacing, -spacing);
        }
        buttons.FindAll(b => b.gameObject.activeSelf)
            .ForEach(btn =>
            {
                btn.setPosition(
                    spacingX * indexX + offsetX,
                    (indexX % 2 == 0) ? y : y2
                    );
                indexX++;
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
