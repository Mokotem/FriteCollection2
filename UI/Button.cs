using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace FriteCollection2.UI;

public class Button : Text
{
    public Action Function;
    protected Color bgColor;

    public Button(UI parent, string name, Action func, int width = 256, int height = 32) : base(parent, name, Bounds.Center, 15)
    {
        rect = new Rectangle(0, 0, width, height);
        outlineThickness = 1;
        this.Function = func;
        this.ApplyScale(width, height);
    }

    public Button(string name, Action func, int width = 256, int height = 32)
        : this(screen, name, func, width, height) { }

    protected byte outlineThickness;

    protected bool InRange()
    {
        return rect.Contains(parent.GetMousePos())
               && rect.Left >= parent.ParentRect.Left
               && rect.Right <= parent.ParentRect.Right
               && rect.Top >= parent.ParentRect.Top
               && rect.Bottom <= parent.ParentRect.Bottom;
    }

    public virtual void Update(bool mouseHold, bool isMouseOn, bool active)
    {
        bgColor = new Color(0.9f, 0.9f, 0.9f);
        outlineThickness = 2;
        if (active && this.Active)
        {
            if (active && InRange())
            {
                outlineThickness = 1;
                bgColor = new Color(0.9f, 0.95f, 1f);
                if (mouseHold)
                {
                    bgColor = new Color(0.8f, 0.9f, 1f);
                }
                else
                {
                    if (isMouseOn)
                    {
                        Function();
                    }
                }
            }
        }
    }

    public override void Draw(in SpriteBatch batch)
    {
        if (Active)
        {
            batch.Draw(Image.defaultTex, new Rectangle(rect.X + 3, rect.Y + 3, rect.Width - 6, rect.Height - 6), bgColor);
            batch.DrawRectangle(rect.ToRectangleF(), new Color(0f, 0.5f, 1f), outlineThickness, layer);
            base.Draw(in batch);
        }
    }
}
