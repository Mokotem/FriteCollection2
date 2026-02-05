using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace FriteCollection2.UI;

public class Toggle : Button
{
    private bool _on;
    public bool On => _on;

    public Action OnEnable, OnDisable;
    public Color EnableColor;

    public Toggle(UI parent, string name, Action enable, Action disable, int width = 256, int height = 32) : base(parent, name, func, width, height)
    {
        this.OnEnable = enable;
        this.OnDisable = disable;
        EnableColor = Color.Yellow;
    }

    public override void Update(bool mouseHold, bool isMouseOn, bool active)
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