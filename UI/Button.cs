using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace FriteCollection2.UI;

public class Button : Text
{
    private static List<Button> _buts = new List<Button>();

    public static List<Button> Buttons => _buts;

    public Action Function { get; init; }
    protected Color bgColor;
    public bool Enabled = true;

    public Keys shortcut;

    public override bool Active
    { 
        get => base.Active;
        set
        {
            if (value && !base.Active)
            {
                _buts.Add(this);
            }
            else if (!value && base.Active)
            {
                _buts.Remove(this);
            }
            base.Active = value;
        }
    }

    public override int Left => rect.X;
    public override int Right => rect.Right;

    public override int Top => rect.Y;
    public override int Bottom => rect.Bottom;

    public Button(UI parent, string name, Action func, int width = 256, int height = 32) : base(parent, name, Bounds.Center, 12)
    {
        _buts.Add(this);
        rect = new Rectangle(0, 0, width, height);
        outlineThickness = 1;
        this.Function = func;
        this.Scale(width, height);
        BackColor = Color.White;
        shortcut = Keys.None;
    }

    public Button(string name, Action func, int width = 256, int height = 32)
        : this(screen, name, func, width, height) { }

    protected byte outlineThickness;

    public bool InRange()
    {
        return rect.Contains(parent.GetMousePos());
    }

    public Color BackColor;

    protected bool ctrl;

    protected void UpdateCtrl(out bool sh, out bool so, KeyboardState kb, KeyboardState pkb)
    {
        if ((shortcut != Keys.None) && (kb.IsKeyDown(Keys.LeftControl) || kb.IsKeyDown(Keys.RightControl)))
        {
            ctrl = true;
        }
        else if (!kb.IsKeyDown(shortcut) && !pkb.IsKeyDown(shortcut))
        {
            ctrl = false;
        }

        sh = ctrl && kb.IsKeyDown(shortcut);

        so = pkb.IsKeyDown(shortcut) && !kb.IsKeyDown(shortcut);
    }

    public virtual void Update(bool mouseHold, bool isMouseOn, bool active, KeyboardState kb, KeyboardState pkb)
    {
        if (this.Active)
        {
            bgColor = new Color(0.9f, 0.9f, 0.9f);

            if (Enabled)
            {
                outlineThickness = 2;

                UpdateCtrl(out bool sh, out bool so, kb, pkb);

                if (active)
                {
                    if (active && (ctrl || InRange()))
                    {
                        outlineThickness = 1;
                        bgColor = new Color(0.9f, 0.95f, 1f);
                        if (mouseHold || sh)
                        {
                            bgColor = new Color(0.8f, 0.9f, 1f);
                        }
                        else if (isMouseOn || so)
                        {
                            Function();
                        }
                    }
                }
            }
        }
    }

    public Color OutlineColor = new Color(0f, 0.5f, 1f);

    public override void Draw(in SpriteBatch batch)
    {
        if (Active)
        {
            if (Enabled)
            {
                batch.Draw(TextureRenderer.Default,
                    new Rectangle(rect.X + 3, rect.Y + 3, rect.Width - 6, rect.Height - 6),
                    null,
                    bgColor * BackColor,
                    0f, Vector2.Zero, Renderer.effect, Renderer._layer + 0.001f);
                batch.DrawRectangle(rect.ToRectangleF(), OutlineColor, outlineThickness, Renderer._layer + 0.001f);
                base.Draw(in batch);
            }
            else
            {
                batch.Draw(TextureRenderer.Default,
                    new Rectangle(rect.X + 3, rect.Y + 3, rect.Width - 6, rect.Height - 6),
                    null, bgColor * BackColor * 0.8f, 0f, Vector2.Zero, Renderer.effect, Renderer._layer + 0.001f);
                batch.DrawRectangle(rect.ToRectangleF(), new Color(0.5f, 0.5f, 0.5f), 1, Renderer._layer + 0.001f);
                base.Draw(in batch);
            }
        }
    }
}
