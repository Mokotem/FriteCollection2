using FriteCollection2.Entity;
using FriteCollection2.Entity.Hitboxs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace FriteCollection2.Tools.TileMap;

public class TileMap : IDisposable, IDraw
{
    public class Settings
    {
        internal readonly Dictionary<string, TileSet> TileSets;
        internal readonly Dictionary<char, Hitbox.Rectangle> hitboxesreplaces;
        internal readonly float[] layers;

        public void LoadTileset(string key, TileSet value)
        {
            TileSets[key] = value;
        }

        public void DisposeTileset(string key)
        {
            TileSets[key].Dispose();
            TileSets[key] = null;
        }

        public Settings(short back, short ground, short general, short fore,
            string[] _tilesets, in Dictionary<char, Hitbox.Rectangle> _hitReplaces)
        {
            layers = new float[4]
            {
                Renderer.ToLayer(back),
                Renderer.ToLayer(ground),
                Renderer.ToLayer(general),
                Renderer.ToLayer(fore),
            };

            this.TileSets = new Dictionary<string, TileSet>();
            foreach (string key in _tilesets)
            {
                this.TileSets[key] = null;
            }

            this.hitboxesreplaces = _hitReplaces;
        }
    }

    private int xCount => _file.layers[0].gridCellsX;
    private int yCount => _file.layers[0].gridCellsY;

    public delegate void DoAt(Point pos);
    public delegate void Entity(Point pos);

    public readonly int Width, Height;
    private readonly Hitbox.Rectangle[,] _hitboxData;

    private readonly float[] _targetLayers;

    public TileMap(IOgmoFileWithLayer file, Settings settings, int seed, in SpriteBatch batch, GraphicsDevice device)
    {
        Random rand = new Random(seed);

        Width = file.width;
        Height = file.height;

        this._targetLayers = settings.layers;
        _file = file;


        _targets = new RenderTarget2D[4];

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
                        _hitboxData[x, y] = settings.hitboxesreplaces[grid.grid[i]];
                    }
                }
            }
            else if (layer is OgmoLayerBlock)
            {
                int layer_id;
                if (layer is OgmoLayerBackground)
                    layer_id = 0;
                else if (layer is OgmoLayerGround)
                    layer_id = 1;
                else if (layer is OgmoLayerGeneral)
                    layer_id = 2;
                else
                    layer_id = 3;

                _targets[layer_id] = new RenderTarget2D
                (
                    device,
                    file.width,
                    file.height
                );

                TileSet _refTileSet = settings.TileSets[layer.tileset];

                device.SetRenderTarget(_targets[layer_id]);
                device.Clear(Color.Transparent);
                batch.Begin(samplerState: SamplerState.PointClamp);

                OgmoLayerBlock data = layer as OgmoLayerBlock;
                for (int i = 0; i < data.data.Length; i++)
                {
                    if (data.data[i] >= 0)
                    {
                        int x = i % xCount;
                        int y = i / xCount;

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
                            _refTileSet.GetRectangle(data.data[i], in rand),
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
            savedHitboxes = PlaceHitboxes(in _hitboxData, this._file.layers[0].gridCellsX, _file.layers[0].gridCellsY);
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
        return PlaceHitboxes(in _hitboxData, tileSize.X, tileSize.Y);
    }

    private Hitbox.Rectangle[] PlaceHitboxes(in Hitbox.Rectangle[,] _hitboxData, int sx, int sy)
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
                    hit.PositionOffset.X += x * sx;
                    hit.PositionOffset.Y += y * sy;
                    hit.LockSize(new Point(
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
        foreach (Hitbox.Rectangle hit in savedHitboxes)
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
                    ++width;
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
                hit.LockSize(
                    hit1.Size.X * width,
                    hit1.Size.Y * height);
                hit.IsStatic = true;

                if (x == 0)
                    hit.IsInfinitOnX = Align.Left;
                else if (x + width >= xCount)
                    hit.IsInfinitOnX = Align.Right;
                if (y == 0)
                    hit.IsInfinitOnY = Align.Left;
                else if (y + height >= yCount)
                    hit.IsInfinitOnY = Align.Right;


                result.Add(hit);

                lst[x, y] = null;

                i = -1;
            }
        }

        return result.ToArray();
    }

    private readonly IOgmoFileWithLayer _file;

    private readonly RenderTarget2D[] _targets;

    public Point Position;

    public Color Color { get; set; }

    public void Draw(byte i, in SpriteBatch batch)
    {
        batch.Draw
        (
            _targets[i],
            new Rectangle
            (
                (int)float.Round(Position.X - Space.Camera.X),
                (int)float.Round(Position.Y - Space.Camera.Y),
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

    public void Draw(in SpriteBatch batch)
    {
        for (byte i = 0; i < 3; ++i)
        {
            Draw(i, in batch);
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

public interface IOgmoFileWithLayer
{
    public string ogmoVersion { get; init; }
    public short width { get; set; }
    public short height { get; set; }
    public short offsetX { get; init; }
    public short offsetY { get; init; }
    public ImmutableArray<OgmoLayer> layers { get; set; }
}

public class LayerTypeDiscriminator : DefaultJsonTypeInfoResolver
{
    private readonly JsonDerivedType entities;
    private readonly Type baseValueType;
    private readonly bool makeEntities;

    public LayerTypeDiscriminator(JsonDerivedType entities)
    {
        this.makeEntities = true;
        this.entities = entities;
        baseValueType = typeof(OgmoLayer);
    }

    public LayerTypeDiscriminator()
    {
        this.makeEntities = false;
        baseValueType = typeof(OgmoLayer);
    }

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type == baseValueType)
        {
            if (makeEntities)
            {
                jsonTypeInfo.PolymorphismOptions = new()
                {
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType,
                    TypeDiscriminatorPropertyName = "name",
                    DerivedTypes =
                {
                    entities
                }
                };
            }
            else
            {
                jsonTypeInfo.PolymorphismOptions = new()
                {
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType,
                    TypeDiscriminatorPropertyName = "name",
                    DerivedTypes =
                {
                    new JsonDerivedType(typeof(OgmoLayerGrid), "hitboxs"),
                    new JsonDerivedType(typeof(OgmoLayerGround), "ground"),
                    new JsonDerivedType(typeof(OgmoLayerGeneral), "general"),
                    new JsonDerivedType(typeof(OgmoLayerBackground), "background"),
                    new JsonDerivedType(typeof(OgmoLayerForeground), "foreground")
                }
                };
            }
        }

        return jsonTypeInfo;
    }
}

public class OgmoFile<LevelValues> : IOgmoFileWithLayer
{
    public static OgmoFile<LevelValues> Open(string path)
    {
        string file;
        using (StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + "/" + path))
            file = sr.ReadToEnd();

        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new LayerTypeDiscriminator()
        };
        return JsonSerializer.Deserialize<OgmoFile<LevelValues>>(file, options);
    }

    public static OgmoFile<LevelValues> OpenPath(string path)
    {
        string file;
        using (StreamReader sr = new StreamReader(path))
            file = sr.ReadToEnd();

        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new LayerTypeDiscriminator()
        };
        return JsonSerializer.Deserialize<OgmoFile<LevelValues>>(file, options);
    }

    public static OgmoFile<LevelValues> Deserialize(string file)
    {
        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new LayerTypeDiscriminator()
        };
        return JsonSerializer.Deserialize<OgmoFile<LevelValues>>(file, options);
    }

    public static ImmutableArray<OgmoLayer> Deserialize(string file, JsonDerivedType entities)
    {
        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new LayerTypeDiscriminator(entities)
        };
        return JsonSerializer.Deserialize<OgmoFile<LevelValues>>(file, options).layers;
    }

    public static ImmutableArray<OgmoLayer> Open(string path, JsonDerivedType entities)
    {
        string file;
        using (StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + "/" + path))
            file = sr.ReadToEnd();

        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new LayerTypeDiscriminator(entities)
        };
        return JsonSerializer.Deserialize<OgmoFile<LevelValues>>(file, options).layers;
    }

    public string ogmoVersion { get; init; }
    public short width { get; set; }
    public short height { get; set; }
    public short offsetX { get; init; }
    public short offsetY { get; init; }

    public ImmutableArray<OgmoLayer> layers { get; set; }
    public LevelValues values { get; init; }
}

public class OgmoLayer
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

public class OgmoLayerBlock : OgmoLayer
{
    public int[] data { get; init; }
}

public class OgmoLayerGround : OgmoLayerBlock { }
public class OgmoLayerGeneral : OgmoLayerBlock { }
public class OgmoLayerForeground : OgmoLayerBlock { }
public class OgmoLayerBackground : OgmoLayerBlock { }

public class OgmoLayerGrid : OgmoLayer
{
    public char[] grid { get; init; }
}