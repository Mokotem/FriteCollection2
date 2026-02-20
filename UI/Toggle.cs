using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;

namespace FriteCollection2.UI;

public class Toggle : Button
{
    private bool _on;
    public bool On => _on;

    public Action OnEnable, OnDisable;
    public Color EnableColor;

    public Toggle(UI parent, string name, Action enable, Action disable, int width = 256, int height = 32)
        : base(parent, name, () => { }, width, height)
    {
        this.OnEnable = enable;
        this.OnDisable = disable;
        EnableColor = Color.Yellow;
    }

    public Toggle(string name, Action enable, Action disable, int width = 256, int height = 32)
        : this(screen, name, enable, disable, width, height) { }

    private void SetColor(float alpha)
    {
        if (_on)
        {
            this.bgColor = EnableColor * alpha;
        }
        else
        {
            this.bgColor = Color.White * alpha;
        }
        this.bgColor.A = 255;
    }

    public override void Update(bool mouseHold, bool isMouseOn, bool active, KeyboardState kb, KeyboardState pkb)
    {
        this.SetColor(0.9f);

        outlineThickness = 2;

        if (Enabled && base.AmIVisible())
        {
            UpdateCtrl(out bool sh, out bool so, kb, pkb);

            if (active && this.Active)
            {
                if (active && (ctrl || InRange()))
                {
                    outlineThickness = 1;
                    this.SetColor(0.95f);

                    if (mouseHold || sh)
                    {
                        this.SetColor(0.8f);
                    }
                    else if (isMouseOn || so)
                    {
                        Function();
                        if (_on)
                        {
                            _on = false;
                            OnDisable();
                        }
                        else
                        {
                            _on = true;
                            OnEnable();
                        }
                    }
                }
            }
        }
    }
}