using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToolBox : ToolButton
{
    public List<ToolButton> buttons;
    public float spacing = 100;

    public bool Enabled => buttons.Any(btn => btn.gameObject.activeSelf);

    public bool ShowSelf => false;

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
        bool showSelf = ShowSelf;
        if (showSelf)
        {
            setPosition(
                spacingX * indexX + offsetX,
                (indexX % 2 == 0) ? y : y2
                );
            indexX++;
        }
        gameObject.SetActive(showSelf);
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
