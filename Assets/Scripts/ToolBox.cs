using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToolBox : ToolButton
{
    public List<ToolButton> buttons;

    public bool Enabled => buttons.Any(btn => btn.gameObject.activeSelf);

    public bool collapsed = false;
    private int savedIndexY;

    public bool ShowRow = true;
    public bool ShowSelf => false;// buttons.Count(btn => btn.gameObject.activeSelf) >= 3;

    /// <summary>
    /// Arranges the buttons in their positions.
    /// </summary>
    /// <param name="indexY">The index of this toolbox, starting from 0 at the bottom left</param>
    public int organize(int indexY)
    {
        savedIndexY = indexY;
        if (!ShowRow)
        {
            //Hide toolbox and its buttons
            organizeCollapsed(indexY);
            float hideX = -Managers.Constants.buttonSpacing * 2;
            setPosition(hideX, 0);
            return 0;
        }
        if (collapsed)
        {
            organizeCollapsed(indexY);
        }
        else
        {
            organizeFull(indexY);
        }
        //Button direction
        Vector3 scale = transform.localScale;
        scale.x = (collapsed) ? -1 : 1;
        transform.localScale = scale;
        return 1;
    }
    void organizeCollapsed(int indexY)
    {
        float spacing = Managers.Constants.buttonSpacing;
        float spacing2 = spacing / 2;
        setPosition(
            spacing2,
            spacing2 + (indexY * spacing)
            );
        gameObject.SetActive(true);
        //Hide buttons
        float hideX = -spacing * 2;
        buttons.FindAll(b => b.gameObject.activeSelf)
             .ForEach(btn =>
             {
                 btn.setPosition(hideX, 0);
             });
    }
    void organizeFull(int indexY)
    {
        float spacing = Managers.Constants.buttonSpacing;
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

    protected override void activateImpl()
    {
        collapsed = !collapsed;
        organize(savedIndexY);
    }

    protected override bool isActiveImpl()
    {
        return false;
    }
    public override Color Color
    {
        get
        {
            ToolButton colorButton = buttons.FirstOrDefault(btn => btn.Active);
            if (!colorButton)
            {
                colorButton = buttons.FirstOrDefault(btn => btn.gameObject.activeSelf);
            }
            return (colorButton) ? colorButton.Color : base.Color;
        }
    }
    public void updateColor()
    {
        image.color = this.Color;
    }
}
