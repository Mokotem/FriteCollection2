using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FriteCollection2.Tools.TileMap;

public class TileSet : IDisposable
{
    public class Settings
    {
        public readonly Point tileSize;
        public readonly Point tileSeparation;
        public readonly Point tileMargin;

        public Settings(Point tileSize, Point tileSeparation, Point tileMargin)
        {
            this.tileSize = tileSize;
            this.tileSeparation = tileSeparation;
            this.tileMargin = tileMargin;
        }
    }

    public int Xlenght { get; private set; }
    public int Ylenght { get; private set; }

    internal readonly Settings settings;

    public TileSet(Texture2D texture, in Settings sets)
    {
        sheet = new Point(texture.Width, texture.Height);
        this.settings = sets;
        pasX = (ushort)(sets.tileSize.X + sets.tileSeparation.X);
        pasY = (ushort)(sets.tileSize.Y + sets.tileSeparation.Y);
        Apply();
        _texture = texture;
    }


    private Texture2D _texture;
    public Texture2D Texture
    {
        get { return _texture; }
    }

    private void Apply()
    {
        Xlenght =
        (sheet.X + settings.tileSeparation.X) / pasX;

        Ylenght =
            (sheet.Y + settings.tileSeparation.Y) / pasY;
    }

    public readonly Point sheet;
    private readonly ushort pasX, pasY;

    public Rectangle GetRectangle(int index)
    {
        Point positon = new Point
        (
            (index % Xlenght) * pasX,
            (index / Xlenght) * pasY
        );
        return new Rectangle
        (
            positon.X,
            positon.Y,
            settings.tileSize.X,
            settings.tileSize.Y
        );
    }

    public virtual Rectangle GetRectangle(int index, in Random rand)
    {
        return GetRectangle(index);
    }

    public virtual Rectangle GetRectangle(Point p)
    {
        return new Rectangle
        (
            p.X * settings.tileSize.X,
            p.Y * settings.tileSize.Y,
            settings.tileSize.X,
            settings.tileSize.Y
        );
    }

    public virtual Rectangle GetRectangle(Point p, in Random rand)
    {
        return GetRectangle(p);
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}

public class TileSetRandomized : TileSet
{
    internal readonly TileRandomizer[] randomizers;


    public TileSetRandomized
        (Texture2D texture, in Settings sets, in TileRandomizer[] randomizer)
        : base(texture, in sets)
    {
        this.randomizers = randomizer;
    }

    public override Rectangle GetRectangle(int index, in Random rand)
    {
        Point pos = new Point(index % Xlenght, index / Xlenght);
        foreach (TileRandomizer r in randomizers)
        {
            if (r.Has(pos))
            {
                pos = r.GetTile(pos, in rand);
                return base.GetRectangle(pos);
            }
        }
        return base.GetRectangle(pos);
    }

    public override Rectangle GetRectangle(Point p, in Random rand)
    {
        foreach (TileRandomizer r in randomizers)
        {
            if (r.Has(p))
            {
                p = r.GetTile(p, in rand);
                return base.GetRectangle(p);
            }
        }
        return base.GetRectangle(p);
    }
}

public class TileRandomizer
{
    private readonly bool twoRectangle;
    private readonly Rectangle rect1, rect2;
    private readonly Point offset;

    public TileRandomizer(Point p1, Point p2)
    {
        if (p2.X <= p1.X)
            throw new System.Exception("p2 doit etre a droite.");

        rect1 = new Rectangle(p1, new Point(1));
        rect2 = new Rectangle(p2, new Point(1));

        offset = new Point(p2.X - p1.X, p2.Y - p1.Y);
        twoRectangle = true;
    }

    public TileRandomizer(Rectangle r1, Rectangle r2)
    {
        if (r2.X <= r1.X)
            throw new System.Exception("p2 doit etre a droite.");

        rect1 = r1;
        rect2 = r2;

        offset = new Point(r2.X - r1.X, r2.Y - r1.Y);
        twoRectangle = true;
    }

    public TileRandomizer(Rectangle r1)
    {
        rect1 = r1;
        twoRectangle = false;
    }

    private static bool PointInRect(Point p, Rectangle r)
    {
        return p.X >= r.X && p.Y >= r.Y && p.X < r.X + r.Width && p.Y < r.Y + r.Height;
    }

    public bool Has(Point p)
    {
        if (twoRectangle)
            return PointInRect(p, rect1) || PointInRect(p, rect2);
        else
            return PointInRect(p, rect1);
    }

    public Point GetTile(Point p, in Random rand)
    {
        if (twoRectangle)
        {
            if (p.X < rect2.X)
            {
                return rand.Next(2) == 0 ? p : p + offset;
            }
            else
            {
                return rand.Next(2) == 0 ? p : p - offset;
            }
        }
        else
        {
            return new Point(
                rect1.X + rand.Next(rect1.Width),
                rect1.Y + rand.Next(rect1.Height)
                );
        }
    }
}
