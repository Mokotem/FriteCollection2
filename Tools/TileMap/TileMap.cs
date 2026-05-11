using FriteCollection2.Entity;
using FriteCollection2.Entity.Hitboxs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace FriteCollection2.Tools.TileMap;

public class TileMap : IDisposable, IDraw
{
    private const byte gridLayer = 0;

    public class Settings
    {
        internal readonly Dictionary<string, TileSet> TileSets;
        internal readonly Dictionary<char, Hitbox.Rectangle> hitModels;
        internal readonly float[] layers;

        public bool HasTileset(string value) => TileSets.ContainsKey(value) && TileSets[value] is not null; 

        public void LoadTileset(string key, TileSet value)
        {
            TileSets[key] = value;
        }

        public void DisposeTileset(string key)
        {
            TileSets[key].Dispose();
            TileSets[key] = null;
            TileSets.Remove(key);
        }

        public Settings(short back, short ground, short general, short fore,
            string[] _tilesets, Dictionary<char, Hitbox.Rectangle> _hitReplaces)
        {
            layers = new float[4]
            {
                TextureRenderer.ToLayer(back),
                TextureRenderer.ToLayer(ground),
                TextureRenderer.ToLayer(general),
                TextureRenderer.ToLayer(fore)
            };

            this.TileSets = new Dictionary<string, TileSet>();
            foreach (string key in _tilesets)
            {
                this.TileSets[key] = null;
            }

            this.hitModels = _hitReplaces;
        }
    }

    private const byte BlockLayerCount = 4;

    private int CountX => _file.layers[0].gridCellsX;
    private int CountY => _file.layers[0].gridCellsY;

    public delegate void DoAt(Point pos);
    public delegate void Entity(Point pos);

    public readonly int Width, Height;

    private readonly float[] _targetLayers;
    private readonly Point[] _breakPos;
    private readonly FriteCollection2.Entity.Object[] _breakableWalls;
    public FriteCollection2.Entity.Object[] BreakableWalls => _breakableWalls;
    public Point[] BreakableWallsPosition => _breakPos;
    private Settings _settings;

    private Space _space;
    public Point Position
    {
        get => _space.Position.ToPoint();
        set
        {
            _space.Position = value.ToVector2();
        }
    }

    private byte TayerToTarget(byte layer)
    {
        return (layer) switch
        {
            3 or 4 => 3,
            5 or 6 => 2,
            7 => 1,
            _ => 0
        };
    }

    public TileMap(IOgmoFileWithLayer file, Settings settings, int seed, SpriteBatch batch, GraphicsDevice device)
    {
        System.Random rand = new System.Random(seed);

        Width = file.width;
        Height = file.height;

        this._targetLayers = settings.layers;
        _file = file;
        this._settings = settings;

        _space = new Space(file.width, file.height);
        _targets = new RenderTarget2D[BlockLayerCount];

        OgmoLayerBreakable l = file.layers[2] as OgmoLayerBreakable;
        List<FriteCollection2.Entity.Object> walls = new List<FriteCollection2.Entity.Object>();
        List<Point> posb = new List<Point>();
        bool[,] visited = new bool[CountX, CountY];
        for (ushort y = 0; y < CountY; y++)
        {
            for (ushort x = 0; x < CountX; x++)
            {
                if (l.data2D[y][x] >= 0 && !visited[x, y])
                {
                    ushort startx = x, starty = y;
                    ushort width = 0, height = 0;
                    while (x < CountX && l.data2D[y][x] >= 0 && !visited[x, y])
                    {
                        x++;
                        width++;
                    }
                    x = startx;
                    while (y < CountY && l.data2D[y][x] >= 0 && !visited[x, y])
                    {
                        y++;
                        height++;
                    }
                    y = starty;

                    TileSet _refTileSet = settings.TileSets[l.tileset];
                    RenderTarget2D tex = new RenderTarget2D(device,
                        width * _refTileSet.settings.tileSize.X,
                        height * _refTileSet.settings.tileSize.Y);
                    device.SetRenderTarget(tex);
                    device.Clear(Color.Transparent);
                    batch.Begin(samplerState: SamplerState.PointClamp);

                    for (x = 0; x < width; x++)
                    {
                        for (y = 0; y < height; y++)
                        {
                            batch.Draw(
                                _refTileSet.Texture,
                                new Rectangle(
                                    x * _refTileSet.settings.tileSize.X,
                                    y * _refTileSet.settings.tileSize.Y,
                                    _refTileSet.settings.tileSize.X,
                                    _refTileSet.settings.tileSize.Y
                                ),
                                _refTileSet.GetRectangle(l.data2D[y + starty][x + startx], rand),
                                Color.White);
                            visited[x + startx, y + starty] = true;
                        }
                    }

                    batch.End();

                    FriteCollection2.Entity.Object wall = new FriteCollection2.Entity.Object();
                    wall.Renderer.Texture = tex;
                    wall.Scale = new Vector2(
                        width * _refTileSet.settings.tileSize.X,
                        height * _refTileSet.settings.tileSize.Y);
                    walls.Add(wall);
                    posb.Add(new Point(startx, starty));

                    x = (ushort)(startx + width);
                }
            }
        }

        for (byte i = 0; i < BlockLayerCount; i++)
        {
            _targets[i] = new RenderTarget2D
            (
                device,
                file.width,
                file.height
            );
        }

        device.SetRenderTarget(_targets[3]);
        device.Clear(Color.Transparent);
        batch.Begin(samplerState: SamplerState.PointClamp);
        DrawLayer((OgmoLayerBlock)file.layers[3], settings, batch, rand);
        DrawLayer((OgmoLayerBlock)file.layers[4], settings, batch, rand);
        batch.End();

        device.SetRenderTarget(_targets[2]);
        device.Clear(Color.Transparent);
        batch.Begin(samplerState: SamplerState.PointClamp);
        DrawLayer((OgmoLayerBlock)file.layers[5], settings, batch, rand);
        DrawLayer((OgmoLayerBlock)file.layers[6], settings, batch, rand);
        batch.End();

        device.SetRenderTarget(_targets[1]);
        device.Clear(Color.Transparent);
        batch.Begin(samplerState: SamplerState.PointClamp);
        DrawLayer((OgmoLayerBlock)file.layers[7], settings, batch, rand);
        batch.End();

        device.SetRenderTarget(_targets[0]);
        device.Clear(Color.Transparent);
        batch.Begin(samplerState: SamplerState.PointClamp);
        DrawLayer((OgmoLayerBlock)file.layers[8], settings, batch, rand);
        DrawLayer((OgmoLayerBlock)file.layers[9], settings, batch, rand);
        batch.End();

        this._breakableWalls = walls.ToArray();
        this._breakPos = posb.ToArray();
        walls = null;

        Color = Color.White;
    }

    private void DrawLayer(OgmoLayerBlock layer, Settings settings, SpriteBatch batch, Random rand)
    {
        TileSet _refTileSet = settings.TileSets[layer.tileset];

        for (ushort x = 0; x < CountX; x++)
        {
            for (ushort y = 0; y < CountY; y++)
            {
                if (layer.data2D[y][x] >= 0)
                {
                    batch.Draw
                    (
                        _refTileSet.Texture,
                        new Rectangle
                        (
                            x * _refTileSet.settings.tileSize.X,
                            y * _refTileSet.settings.tileSize.Y,
                            _refTileSet.settings.tileSize.X,
                            _refTileSet.settings.tileSize.Y
                        ),
                        _refTileSet.GetRectangle(layer.data2D[y][x], rand),
                        Color.White
                    );
                }
            }
        }
    }

    private Hitbox.Rectangle[] savedHitboxes;

    private bool GetNextHole(bool[,] done, char key, out Point pos)
    {
        for(int x = 0; x < CountX; x++)
        {
            for (int y = 0; y < CountY; y++)
            {
                if (!done[x, y] && ((OgmoLayerGrid)_file.layers[gridLayer]).grid2D[y][x].Equals(key))
                {
                    pos = new Point(x, y);
                    return true;
                }
            }
        }

        pos = Point.Zero;
        return false;
    }

    private bool IsFree(Rectangle r, char envi)
    {
        if (r.X < 0 || r.Y < 0 || r.Right > CountX || r.Bottom > CountY)
        {
            return false;
        }

        OgmoLayerGrid grid = (OgmoLayerGrid)_file.layers[gridLayer];

        for (int x = r.Left; x < r.Right; x++)
        {
            for (int y = r.Top; y < r.Bottom; y++)
            {
                if (!grid.grid2D[y][x].Equals(envi))
                    return false;
            }
        }
        return true;
    }

    private bool TryExpand(Rectangle r, char envi, Sides side, out Rectangle result)
    {
        Rectangle res = r;
        Rectangle c;

        switch (side)
        {
            case Sides.Up:
            c = new Rectangle(r.X, r.Y - 1, r.Width, 1);
            res.Y--;
            res.Height++;
            break;
            case Sides.Left:
            c = new Rectangle(r.X - 1, r.Y, 1, r.Height);
            res.X--;
            res.Width++;
            break;
            case Sides.Right:
            c = new Rectangle(r.Right, r.Y, 1, r.Height);
            res.Width++;
            break;
            default:
            c = new Rectangle(r.X, r.Bottom, r.Width, 1);
            res.Height++;
            break;
        };

        if (IsFree(c, envi))
        {
            result = res;
            return true;
        }
        else
        {
            result = r;
            return false;
        }
    }

    private Hitbox.Rectangle CreateHitboxAt(Hitbox.Rectangle model, char envi, Point pos, bool[,] done)
    {
        Rectangle r = new Rectangle(pos.X, pos.Y, 1, 1);

        while (
            TryExpand(r, envi, Sides.Up, out r)
            | TryExpand(r, envi, Sides.Down, out r)
            | TryExpand(r, envi, Sides.Left, out r)
            | TryExpand(r, envi, Sides.Right, out r))
        {

        }

        for (int x = r.Left; x < r.Right; x++)
        {
            for (int y = r.Top; y < r.Bottom; y++)
            {
                done[x, y] = true;
            }
        }

        Hitbox.Rectangle result = new Hitbox.Rectangle(this._space, model.Tags);

        if (r.X < 1)
            result.infinitLeft = true;
        if (r.Right >= CountX)
            result.infinitRight = true;
        if (r.Y < 1)
            result.infinitUp = true;
        if (r.Bottom >= CountY)
            result.infinitDown = true;

        result.isStatic = true;
        result.SetScale(ToMap(r.Size) + model.Size);
        result.UpdatePosition(ToMap(r.Location) + model.offset + this.Position);

        return result;
    }

    public void CreateHitboxsMerge()
    {
        List<Hitbox.Rectangle> result = new List<Hitbox.Rectangle>();
        bool[,] done;

        foreach (char key in _settings.hitModels.Keys)
        {
            done = new bool[CountX, CountY];
            while (GetNextHole(done, key, out Point pos))
            {
                result.Add(CreateHitboxAt(_settings.hitModels[key], key, pos, done));
            }
        }

        savedHitboxes = result.ToArray();
    }

    public void ReactivateHitboxs()
    {
        foreach (Hitbox.Rectangle hit in savedHitboxes)
        {
            hit.Reactivate();
        }
    }

    private readonly IOgmoFileWithLayer _file;

    private readonly RenderTarget2D[] _targets;

    public Color Color { get; set; }

    private Point ToMap(Point p)
    {
        return new Point(p.X * _file.layers[0].gridCellWidth, p.Y * _file.layers[0].gridCellHeight);
    }

    private Rectangle ToMap(Rectangle r)
    {
        return new Rectangle(ToMap(r.Location), ToMap(r.Size));
    }

    public void Draw(byte i, SpriteBatch batch)
    {
        batch.Draw
        (
            _targets[i],
            new Rectangle
            (
                (int)(_space.X - Space.Camera.X),
                (int)(_space.Y - Space.Camera.Y),
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

    public void Draw(SpriteBatch batch)
    {
        for (byte i = 0; i < BlockLayerCount; i++)
        {
            Draw(i, batch);
        }
    }

    public void DestroyHitboxs()
    {
        foreach (Hitbox.Rectangle hit in this.savedHitboxes)
        {
            hit.Dispose();
        }
    }

    public void Dispose()
    {
        foreach (RenderTarget2D t in _targets)
            t.Dispose();
        this.savedHitboxes = null;
    }
}