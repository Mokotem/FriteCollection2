using System;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;


namespace FriteCollection2.Tools.TileMap;

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
                    new JsonDerivedType(typeof(OgmoLayerBreakable), "breakable"),
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

    public ushort xCount => layers[0].gridCellsX;
    public ushort yCount => layers[0].gridCellsY;

    public ImmutableArray<OgmoLayer> layers { get; set; }
    public LevelValues values { get; init; }
}

public class OgmoLayer
{
    public string name { get; init; }
    //public string _eid { get; init; }
    //public int offsetX { get; init; }
    //public int offsetY { get; init; }
    public byte gridCellWidth { get; set; }
    public byte gridCellHeight { get; set; }
    public ushort gridCellsX { get; init; }
    public ushort gridCellsY { get; init; }
    public string tileset { get; init; }
    //public int exportMode { get; init; }
    //public int arrayMode { get; init; }
}

public class OgmoLayerBlock : OgmoLayer
{
    public int[][] data2D { get; init; }
}

public class OgmoLayerGround : OgmoLayerBlock { }
public class OgmoLayerGeneral : OgmoLayerBlock { }
public class OgmoLayerForeground : OgmoLayerBlock { }
public class OgmoLayerBreakable : OgmoLayerBlock { }
public class OgmoLayerBackground : OgmoLayerBlock { }

public class OgmoLayerGrid : OgmoLayer
{
    public char[][] grid2D { get; init; }
}