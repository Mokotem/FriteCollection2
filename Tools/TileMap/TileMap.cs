using FriteCollection2.Entity;
using FriteCollection2.Entity.Hitboxs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FriteCollection2.Tools.TileMap;

public class TileSet : IDisposable
{
    public int Xlenght { get; private set; }
    public int Ylenght { get; private set; }

    public TileSet(Texture2D texture, Point tileSize, Point tileSeparation, Point tileMargin)
    {
        sheet = new Point(texture.Width, texture.Height);
        _tileSize = tileSize;
        _tileSeparation = tileSeparation;
        _tileMargin = tileMargin;
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
        (sheet.X + _tileSeparation.X) / (_tileSize.X + _tileSeparation.X);

        Ylenght =
            (sheet.Y + _tileSeparation.Y) / (_tileSize.Y + _tileSeparation.Y);
    }
    public readonly Point sheet;
    private Point _tileSize;
    public Point TileSize
    {
        get { return _tileSize; }
        set
        {
            _tileSize = value;
            Apply();
        }
    }

    private Point _tileSeparation = new(0, 0);
    public Point TileSeparation
    {
        get { return _tileSeparation; }
        set
        {
            _tileSeparation = value;
            Apply();
        }
    }
    private Point _tileMargin = new(0, 0);
    public Point TileMargin
    {
        get { return _tileMargin; }
        set
        {
            _tileMargin = value;
            Apply();
        }
    }

    public Rectangle GetRectangle(int index)
    {
        Point positon = new Point
        (
            index * (_tileSize.X + _tileSeparation.X) % (sheet.X + _tileSeparation.X),
            (index / ((sheet.X + _tileSeparation.X) / (_tileSize.X + _tileSeparation.X))) * (_tileSize.Y + _tileSeparation.Y)
        );
        return new Rectangle
        (
            positon.X,
            positon.Y,
            _tileSize.X,
            _tileSize.Y
        );
    }

    public Rectangle GetRectangle(Point p)
    {
        return new Rectangle
        (
            p.X * _tileSize.X,
            p.Y * _tileSize.Y,
            _tileSize.X,
            _tileSize.Y
        );
    }

    public void Dispose()
    {
        _texture.Dispose();
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

public class TileMap : IDisposable, ILayer
{
    private int xCount => _file.layers[0].gridCellsX;
    private int yCount => _file.layers[0].gridCellsY;

    public delegate void DoAt(Point pos);
    public delegate void Entity(Point pos);

    private float layer = 0.5f;
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

    public readonly int Width, Height;
    private readonly Hitbox.Rectangle[,] _hitboxData;

    public TileMap(TileSet sheet,
        OgmoFile file,
        in Hitbox.Rectangle[] hitboxesreplaces,
        TileRandomizer[] randomize,
        int seed)
    {
        Random rand = new Random(seed);

        Width = file.width;
        Height = file.height;

        _sheet = sheet;
        _file = file;

        SpriteBatch batch = GameManager.Instance.SpriteBatch;

        _targets = new RenderTarget2D[3];

        Random r = new Random();

        _hitboxData = new Hitbox.Rectangle[xCount, yCount];

        foreach (OgmoLayer layer in file.layers)
        {
            if (layer is OgmoLayerGrid)
            {
                OgmoLayerGrid grid = layer as OgmoLayerGrid;
                for (int i = 0; i < grid.grid.Length; i++)
                {
                    if (!grid.grid[i].Equals('0'))
                    {
                        int x = i % xCount;
                        int y = i / xCount;
                        _hitboxData[x, y] = hitboxesreplaces[0];
                    }
                }
            }
            else if (layer is OgmoLayerEntity)
            {

            }
            else
            {
                int layer_id = layer is OgmoLayerBackground ? 0 : (layer is OgmoLayerForeground ? 2 : 1);
                _targets[layer_id] = new RenderTarget2D
                (
                    GameManager.Instance.GraphicsDevice,
                    file.width,
                    file.height
                );
                GameManager.Instance.GraphicsDevice.SetRenderTarget(_targets[layer_id]);
                GameManager.Instance.GraphicsDevice.Clear(Color.Transparent);
                batch.Begin(samplerState: SamplerState.PointClamp);

                OgmoLayerBlock data = layer as OgmoLayerBlock;
                for (int i = 0; i < data.data.Length; i++)
                {
                    if (data.data[i] >= 0)
                    {
                        int x = i % xCount;
                        int y = i / xCount;

                        Point s = new Point(data.data[i] % _sheet.Xlenght, data.data[i] / _sheet.Xlenght);

                        byte id_random = 0;
                        while (id_random < randomize.Length && !randomize[id_random].Has(s))
                            ++id_random;

                        if (id_random < randomize.Length)
                            s = randomize[id_random].GetTile(s, in rand);

                        batch.Draw
                        (
                            sheet.Texture,
                            new Rectangle
                            (
                                x * sheet.TileSize.X,
                                y * sheet.TileSize.Y,
                                sheet.TileSize.X,
                                sheet.TileSize.Y
                            ),
                            sheet.GetRectangle(s),
                            Color.White
                        );
                    }
                }
                batch.End();
            }
        }

        Color = Color.White;
    }

    private Hitbox.Rectangle[] savedHitboxes;

    public void GenerateHitboxs(bool mergeHitBoxes = true)
    {
        if (mergeHitBoxes)
            savedHitboxes = MergeHitBoxes(in _hitboxData);
        else
            savedHitboxes = PlaceHitboxes(in _hitboxData, _sheet.TileSize);
    }

    public void GenerateHitboxs(Point size, bool mergeHitBoxes = true)
    {
        if (mergeHitBoxes)
            savedHitboxes = MergeHitBoxes(in _hitboxData);
        else
            savedHitboxes = PlaceHitboxes(in _hitboxData, size);
    }

    private Hitbox.Rectangle[] PlaceHitboxes(in Hitbox.Rectangle[,] _hitboxData, Point tileSize)
    {
        List<Hitbox.Rectangle> result = new List<Hitbox.Rectangle>();
        for (int x = 0; x < xCount; ++x)
        {
            for (int y = 0; y < yCount; ++y)
            {
                if (_hitboxData[x, y] is not null)
                {
                    Hitbox.Rectangle hit = _hitboxData[x, y].Copy();
                    hit.Active = true;
                    hit.PositionOffset.X += x * tileSize.X;
                    hit.PositionOffset.Y += y * tileSize.Y;
                    hit.LockSize(new Vector2(
                        _hitboxData[x, y].Size.X,
                        _hitboxData[x, y].Size.Y)
                        );
                    result.Add(hit);
                }
            }
        }
        return result.ToArray();
    }

    public void ReactivateHitboxs()
    {
        foreach(Hitbox.Rectangle hit in savedHitboxes)
        {
            hit.Reactivate();
        }
    }

    /// <summary>
    /// algo banger que j'ai fais pour éviter la redondance de hitboxes
    /// </summary>
    private Hitbox.Rectangle[] MergeHitBoxes(in Hitbox.Rectangle[,] lst)
    {
        List<Hitbox.Rectangle> result = new List<Hitbox.Rectangle>();
        int i = -1;
        while (i + 1 < xCount * yCount)
        {
            i++;
            int x = i % xCount;
            int y = i / xCount;

            Hitbox.Rectangle hit1 = lst[x, y];

            if (hit1 is not null)
            {
                int width = 1;
                int height = 1;

                while (x + width < xCount
                    && lst[x + width, y] is not null
                    && lst[x + width, y]._tag == hit1._tag
                    && lst[x + width, y].Layer == hit1.Layer
                    && lst[x + width, y].Size.Y == hit1.Size.Y)
                {
                    lst[x + width, y] = null;
                    width++;
                }

                bool Cond(in Hitbox.Rectangle[,] h)
                {
                    if (y + height >= yCount)
                        return false;
                    Hitbox.Rectangle h2 = hit1;
                    for (int k = 0; k < width; k++)
                    {
                        Hitbox.Rectangle h1 = h[x + k, y + height];
                        if (h1 is null
                           || h1._tag != h2._tag
                           || h1.Layer != h2.Layer
                           || h1.Size.X != h2.Size.X)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                while (Cond(in lst))
                {
                    for (int k = 0; k < width; k++)
                    {
                        lst[x + k, y + height] = null;
                    }
                    height++;
                }

                Hitbox.Rectangle hit = hit1.Copy();
                hit.Active = true;
                hit.PositionOffset.X += x * _file.layers[0].gridCellWidth + this.Position.X;
                hit.PositionOffset.Y += y * _file.layers[0].gridCellHeight + this.Position.Y;
                float a = 0f;
                float b = 0f;
                if (hit._tag == "red" || hit._tag == "green")
                {
                    if (width >= height)
                    {
                        a = -6f;
                    }
                    else
                    {
                        b = -6f;
                    }
                }
                hit.LockSize(new Vector2(
                    hit1.Size.X * width + a,
                    hit1.Size.Y * height + b));
                hit.IsStatic = true;

                result.Add(hit);

                lst[x, y] = null;

                i = -1;
            }
        }

        return result.ToArray();
    }

    private readonly TileSet _sheet;
    private readonly OgmoFile _file;

    private readonly RenderTarget2D[] _targets;
    private float[] _targetLayers;

    public void SetTargetLayers(short back, short ground, short fore)
    {
        _targetLayers = new float[3]
        {
            (back + 1000f) / 2000f,
            (ground + 1000f) / 2000f,
            (fore + 1000f) / 2000f
        };
    }

    public Point Position;

    public Color Color { get; set; }

    public void Draw(byte i)
    {
        GameManager.Instance.SpriteBatch.Draw
            (
                _targets[i],
                new Rectangle
                (
                    (int)float.Round(Position.X - Camera.Position.X),
                    (int)float.Round(Position.Y - Camera.Position.Y),
                    _targets[i].Width,
                    _targets[i].Height
                ),
                null,
                Color,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                _targetLayers[i]
            );
    }

    public void Draw()
    {
        for(byte i = 0; i < 3; ++i)
        {
            Draw(i);
        }
    }



    public void DestroyHitboxs()
    {
        foreach (Hitbox.Rectangle hit in this.savedHitboxes)
        {
            hit.Destroy();
        }
    }

    public void Dispose()
    {
        foreach (RenderTarget2D t in _targets)
            t.Dispose();
        this.savedHitboxes = null;
    }
}

public class OgmoFile
{
    public static T Open<T>(string path) where T: OgmoFile
    {
        string file;
        using (StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + "/" + path))
            file = sr.ReadToEnd();
        return JsonSerializer.Deserialize<T>(file);
    }

    public string ogmoVersion { get; init; }
    public short width { get; set; }
    public short height { get; set; }
    public short offsetX { get; init; }
    public short offsetY { get; init; }
    public OgmoLayer[] layers { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "name")]
[JsonDerivedType(typeof(OgmoLayerGrid), typeDiscriminator: "hitboxs")]
[JsonDerivedType(typeof(OgmoLayerForeground), typeDiscriminator: "foreground")]
[JsonDerivedType(typeof(OgmoLayerGround), typeDiscriminator: "ground")]
[JsonDerivedType(typeof(OgmoLayerBackground), typeDiscriminator: "background")]
[JsonDerivedType(typeof(OgmoLayerEntity), typeDiscriminator: "entities")]
public abstract class OgmoLayer
{
    public string name { get; init; }
    public string _eid { get; init; }
    public int offsetX { get; init; }
    public int offsetY { get; init; }
    public int gridCellWidth { get; set; }
    public int gridCellHeight { get; set; }
    public int gridCellsX { get; init; }
    public int gridCellsY { get; init; }
    public string tileset { get; init; }
    public int exportMode { get; init; }
    public int arrayMode { get; init; }
}

public class MoonValues
{
    public string Name { get; init; }
}

public class MoonOgmoFile : OgmoFile
{
    public MoonValues values { get; init; }
}

public class OgmoLayerBlock : OgmoLayer
{
    public int[] data { get; init; }
}

public class OgmoLayerGround : OgmoLayerBlock { }
public class OgmoLayerForeground : OgmoLayerBlock { }
public class OgmoLayerBackground : OgmoLayerBlock { }

public class OgmoLayerGrid : OgmoLayer
{
    public char[] grid { get; init; }
}

public class OgmoLayerEntity : OgmoLayer
{
    public Ent[] entities { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "name")]
[JsonDerivedType(typeof(Ent.StartPos), typeDiscriminator: "spawn")]
[JsonDerivedType(typeof(Ent.Cleaner), typeDiscriminator: "cleaner")]
public abstract class Ent
{
    public string name { get; init; }
    public short id { get; init; }
    public short x { get; init; }
    public short y { get; init; }

    public class StartPos : Ent { }
    public class Cleaner : Ent { }
}

public static class MetroidMaker
{
    public const short RoomWidth = 30, RoomHeight = 20;
    public const short TRoomWidth = RoomWidth * TileSize, TRoomHeight = RoomHeight * TileSize;
    public const short TileSize = 8;

    public class Room
    {
        public readonly short ID;
        public readonly short width, height;
        public readonly string name;
        public readonly byte[] data;

        public Room(MoonOgmoFile room, short id)
        {
            this.ID = id;
            this.width = (short)float.Round(room.width / TRoomWidth);
            this.height = (short)float.Round(room.height / TRoomHeight);
            this.name = room.values.Name;
            OgmoLayerGrid layer = room.layers[0] as OgmoLayerGrid;
            OgmoLayerBackground bg = room.layers[4] as OgmoLayerBackground;
            this.data = new byte[layer.grid.Length];
            for (int i = 0; i < layer.grid.Length; ++i)
            {
                this.data[i] = 0;
                if (!layer.grid[i].Equals('0'))
                    this.data[i] = 1;
                else if (bg.data[i] >= 0)
                    this.data[i] = 2;
            }
        }
    }

    public struct P
    {
        public short X { get; set; }
        public short Y { get; set; }

        public P(short x, short y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is P)
            {
                P a = (P)obj;
                return X == a.X && Y == a.Y;
            }
            return false;
        }
    }

    public class Map
    {
        public bool IsSaved { get; set; }

        public bool IsNew { get; set; }

        public short Width { get; set; }
        public short Height { get; set; }
        public short TotalWidth => (short)(Width * RoomWidth);
        public short TotalHeight => (short)(Height * RoomHeight);

        public Map()
        {

        }

        public List<P> Data { get; set; }
        public short[][] Position { get; set; }

        public List<P[]> PosLinks { get; set; }
        public List<short[]> IdLinks { get; set; }

        public void Start(int numberRooms)
        {
            Data = new List<P>();
            for (int i = 0; i < numberRooms; ++i)
            {
                Data.Add(new P(-1, -1));
            }
            PosLinks = new List<P[]>();
            Position = new short[Width][];
            for (short i = 0; i < Width; ++i)
            {
                Position[i] = new short[Height];
                Array.Fill<short>(Position[i], -1);
            }
            IdLinks = new List<short[]>();
        }
    }
}