using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FriteCollection2.Entity;

public abstract class Entity
{
    public Space Space = new Space();

    public Renderer Renderer = new Renderer();

    public virtual void Draw() { }
}

/// <summary>
/// Object.
/// </summary>
public partial class Object : Entity, ICopy<Object>, IDraw
{
    public Object Copy()
    {
        return new()
        {
            Space = Space.Copy(),
            Renderer = Renderer.Copy()
        };
    }

    public override void Draw()
    {
        if (Renderer.hide == false)
        {
            float flipFactor = 0f;

            float rot = Space.rotation + flipFactor;

            GameManager.Instance.SpriteBatch.Draw
            (
                Renderer.Texture,
                new Rectangle(
                    (int)float.Round(Space.Position.X - Camera.Position.X),
                    (int)float.Round(Space.Position.Y - Camera.Position.Y),
                    (int)float.Round(Space.Scale.X),
                    (int)float.Round(Space.Scale.Y)
                ),
                null,
                Renderer.Color * Renderer.Alpha,
                rot,
                Vector2.Zero,
                Renderer.effect,
                Renderer.GetLayer()
            );
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Object)
        {
            return Space.Equals((obj as Object).Space)
                && Renderer.Equals((obj as Object).Renderer);
        }
        return false;
    }

    public override string ToString()
    {
        return "Object (" + Space.ToString() + ", " + Renderer.ToString() + ")";
    }
}

/// <summary>
/// Text.
/// </summary>
public class Text : Entity, ICopy<Text>, IDraw
{
    private int _spacing;

    public bool outLine = true;

    public Text Copy()
    {
        Text t = new(font)
        {
            Space = Space.Copy(),
            Renderer = Renderer.Copy(),
            Spacing = _spacing
        };
        return t;
    }

    public override void Draw()
    {
        if (Renderer.hide == false && _text != null)
        {
            if (outLine)
            {
                foreach (Vector2 pos in new Vector2[8]
                {
                new(-1, 1),
                new(-1, -1),
                new(1, -1),
                new(1, 1),

               new(0, 1),
                new(0, -1),
                new(1, 0),
                new(-1, 0),
                })
                {
                    GameManager.Instance.SpriteBatch.DrawString
                (
                    font,
                    _text,
                    Space.Position,
                    Color.Black,
                    Space.rotation,
                    Vector2.Zero,
                    1,
                    SpriteEffects.None,
                    Renderer.Layer - 0.0001f
                );
                }
            }

            GameManager.Instance.SpriteBatch.DrawString
            (
                font,
                _text,
                Space.Position,
                this.Renderer.Color,
                Space.rotation * (MathF.PI / 180f),
                Vector2.Zero,
                1,
                SpriteEffects.None,
                Renderer.Layer
            );
        }
    }

    private string _text = null;

    /// <summary>
    /// Text to show.
    /// </summary>
    public string Write
    {
        get { return _text; }
        set
        {
            _text = value;
            Space.Scale = new Vector2
            (
                font.MeasureString(value).X + _spacing * (value.Length - 1),
                font.MeasureString(value).Y
            );
        }
    }

    private SpriteFont font;

    /// <summary>
    /// Gets the font file of the Text.
    /// </summary>
    public SpriteFont Font
    {
        get
        {
            return font;
        }
    }

    private void Constructor()
    {
        _spacing = 0;
    }

    /// <summary>
    /// Creates a Text Entity.
    /// </summary>
    /// <param name="font">font file</param>
    public Text(SpriteFont font)
    {
        this.font = font;
        Constructor();
    }

    /// <summary>
    /// Creates a Text Entity.
    /// </summary>
    /// <param name="font">font file</param>
    /// <param name="text">text to show</param>
    public Text(SpriteFont font, string text)
    {
        this.font = font;
        Constructor();
        Write = text;
    }

    /// <summary>
    /// Spacing between letters.
    /// </summary>
    public float Spacing
    {
        get { return font.Spacing; }
        set
        {
            font.Spacing = value;
        }
    }

    public override string ToString()
    {
        return "Text " + Write + " : (" + Space.ToString() + ", " + Renderer.ToString() + ")";
    }
}