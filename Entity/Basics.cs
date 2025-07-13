using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections;

namespace FriteCollection2.Entity;

interface ICopy<T>
{
    /// <summary>
    /// Fait une copie.
    /// </summary>
    public T Copy();
}

/// <summary>
/// Permet de décrire une entité dans l'espace.
/// </summary>
public class Space : ICopy<Space>, IEnumerable
{
    public static void SetDefaultEnvironment(in Environment env)
    {
        defaultEnvironment = env;
    }
    private static Environment defaultEnvironment;

    private readonly Environment environment;

    public class EnumCorners : IEnumerator
    {
        byte i;
        Space space;

        public EnumCorners(Space sp)
        {
            this.space = sp;
        }

        bool IEnumerator.MoveNext()
        {
            ++i;
            return i < 4;
        }

        void IEnumerator.Reset()
        {
            i = 0;
        }

        object IEnumerator.Current =>
            new Vector2(
                i % 2 == 0 ? space.X : space.X + space.W,
                i / 2 == 0 ? space.Y : space.Y + space.H);
    }

    public IEnumerator GetEnumerator()
    {
        return new EnumCorners(this);
    }

    private void Init()
    {
        Position = Vector2.Zero;
        Scale = new Vector2(30, 30);
    }

    public Space()
    {
        this.environment = defaultEnvironment;
        Init();
    }

    public Space(in Environment env)
    {
        this.environment = env;
        Init();
    }

    public Space Copy()
    {
        return new Space()
        {
            Position = Position,
            Scale = Scale
        };
    }

    /// <summary>
    /// Position.
    /// </summary>
    public Vector2 Position;

    public void SetPosition(Vector2 pos, Bounds centerPoint)
    {
        Position = pos - BoundFunc.BoundToVector(centerPoint, Scale.X, Scale.Y);
    }

    public Vector2 CenterPoint => new Vector2(Position.X + (Scale.X / 2f), Position.Y + (Scale.Y / 2f));
    public float CenterPointX => Position.X + (Scale.X / 2f);
    public float CenterPointY => Position.Y + (Scale.Y / 2f);

    public float X
    {
        get => Position.X;
        set => Position.X = value;
    }
    public float Y
    {
        get => Position.Y;
        set => Position.Y = value;
    }

    public float W
    {
        get => Scale.X;
        set => Scale.X = value;
    }
    public float H
    {
        get => Scale.Y;
        set => Scale.Y = value;
    }

    /// <summary>
    /// Taille.
    /// </summary>
    /// <remarks>Les tailles négatives sont prises en charges.</remarks>
    public Vector2 Scale;

    public override bool Equals(object obj)
    {
        if (obj is Space)
        {
            Space sp = obj as Space;
            return Scale == sp.Scale && Position == sp.Position;
        }
        return false;
    }

    public override string ToString()
    {
        return "Transform (position:" + Position.ToString() + ", scale:" + Scale.ToString() + ")";
    }
}

public class SpaceDirection : Space
{
    public float direction = 0f;
}

public static class BoundFunc
{
    public static Vector2 BoundToVector(Bounds b, float width, float height)
    {
        return b switch
        {
            Bounds.TopLeft => new Vector2(0, 0),
            Bounds.Top => new Vector2(width / 2f, 0),
            Bounds.TopRight => new Vector2(width, 0),

            Bounds.Left => new Vector2(0, height / 2f),
            Bounds.Center => new Vector2(width / 2f, height / 2f),
            Bounds.Right => new Vector2(width, height / 2f),

            Bounds.BottomLeft => new Vector2(0, height),
            Bounds.Bottom => new Vector2(width / 2f, height),
            Bounds.BottomRight => new Vector2(width, height),

            _ => new Vector2(0, 0)
        };
    }

    public static Vector2[] CreateBounds(float width, float height)
    {
        Vector2[] vList = new Vector2[9];
        for (int i = 0; i < 9; i++)
        {
            vList[i] = BoundToVector((Bounds)i, width, height);
        }

        return vList;
    }
}

public interface ILayer
{
    public short Layer
    {
        get;
        set;
    }
}

/// <summary>
/// Permet de décrire l'apparence d'une entité.
/// </summary>
public class Renderer : ICopy<Renderer>, ILayer
{
    internal static Texture2D _defaultTexture, _notFoundTexture;
    public static Texture2D DefaultTexture => _defaultTexture;
    public static Texture2D NotFoundTexture => _notFoundTexture;

    private float layer;
    public float GetLayer() => layer;

    public SpriteEffects effect = SpriteEffects.None;

    public static float outlineLayer;
    public bool outline = true;
    public Color outlineColor = Color.Black;

    public static float ToLayer(short value)
    {
        if (value > 1000) throw new ArgumentOutOfRangeException("value cannot be greater than 1000");
        if (value < -1000) throw new ArgumentOutOfRangeException("value cannot be less than -1000");
        return (value + 1000f) / 2000f;
    }

    public short Layer
    {
        get
        {
            return (short)((layer * 2000) - 1000);
        }

        set
        {
            if (value > 1000) throw new ArgumentOutOfRangeException("value cannot be greater than 1000");
            if (value < -1000) throw new ArgumentOutOfRangeException("value cannot be less than -1000");
            layer = (value + 1000f) / 2000f;
        }
    }

    public static Texture2D CreateCircleTexture(int width)
    {
        Texture2D tex = new Texture2D(GameManager.Instance.GraphicsDevice, width, width);
        Color[] data = new Color[width * width];
        for (int i = 0; i < width; i += 1)
        {
            for (int j = 0; j < width; j += 1)
            {
                if (float.Sqrt(float.Pow(i - (width / 2), 2) + float.Pow(j - (width / 2), 2)) <= width / 2)
                {
                    data[i + (j * width)] = Color.White;
                }
                else
                {
                    data[i + (j * width)] = Color.Transparent;
                }
            }
        }
        tex.SetData<Color>(data);
        return tex;
    }

    public static Texture2D CreateCircleTexture(int width, int holeSize)
    {
        Texture2D tex = new Texture2D(GameManager.Instance.GraphicsDevice, width, width);
        Color[] data = new Color[width * width];
        for (int i = 0; i < width; i += 1)
        {
            float a = float.Pow(i - (width / 2), 2);
            for (int j = 0; j < width; j += 1)
            {
                float d = float.Sqrt(a + float.Pow(j - (width / 2), 2));
                if (d <= width / 2 && d >= holeSize / 2)
                {
                    data[i + (j * width)] = Color.White;
                }
                else
                {
                    data[i + (j * width)] = Color.Transparent;
                }
            }
        }
        tex.SetData<Color>(data);
        return tex;
    }

    public static Texture2D CreateFrameTexture(int width, int height, ushort borderSize)
    {
        Texture2D tex = new Texture2D(GameManager.Instance.GraphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < width; i += 1)
        {
            for (int j = 0; j < height; j += 1)
            {
                if (i < borderSize || j < borderSize || width - i < borderSize + 1 || height - j < borderSize + 1)
                {
                    data[i + (j * width)] = Color.White;
                }
                else
                {
                    data[i + (j * width)] = Color.Transparent;
                }
            }
        }
        tex.SetData<Color>(data);
        return tex;
    }

    public Renderer()
    {
        _texture = _defaultTexture;
        layer = 0.5f;
    }

    public static RenderTarget2D CreateRenderTarget(int width, int height)
    {
        return new RenderTarget2D(GameManager.Instance.GraphicsDevice, width, height);
    }

    public Renderer Copy()
    {
        Renderer r = new()
        {
            _texture = _defaultTexture,
            Color = this.Color,
            hide = hide
        };
        return r;
    }

    private Texture2D _texture;

    /// <summary>
    /// Texture.
    /// </summary>
    public Texture2D Texture
    {
        get
        {
            return _texture;
        }
        set
        {
            _texture = value;
        }
    }

    public Color Color = Color.White;

    /// <summary>
    /// Masquer.
    /// </summary>
    public bool hide = false;

    public override bool Equals(object obj)
    {
        if (obj is Renderer)
        {
            Renderer re = obj as Renderer;
            return _texture.Equals(re._texture) && Color == re.Color
                && hide == re.hide;
        }
        return false;
    }

    public override string ToString()
    {
        if (Texture is null)
        {
            return "Renderer (texture: NULL, color:" + Color.ToString() + ")";
        }
        else { return "Renderer (texture: true, color:" + Color.ToString() + ")"; }
    }
}
