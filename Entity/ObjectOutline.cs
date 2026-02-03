using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriteCollection2.Entity;

public class ObjectOutline : Object
{
    internal static readonly Point[] outLinePositions = new Point[8]
    {
        new(-1, 1),
        new(0, 1),
        new(1, 1),

        new(-1, 0),
        new(1, 0),

        new(-1, -1),
        new(0, -1),
        new(1, -1)
    };

    public static Color outlineColor = Color.Black;
    private static float outlineLayer = 0f;
    public static short OutlineLayer
    {
        get => Renderer.FromLayer(outlineLayer);
        set => outlineLayer = Renderer.ToLayer(value);
    }

    public ObjectOutline() : base() { }

    public ObjectOutline(Texture2D texture) : base(texture) { }

    public ObjectOutline(int width, int height) : base(width, height) { }

    public override void Draw(in SpriteBatch batch)
    {
        Rectangle rect = ToScreen();
        this.Renderer._layer = outlineLayer;
        foreach (Point p in outLinePositions)
        {
            Renderer.Draw(in batch, new Rectangle(rect.Location + p, rect.Size))
        }
    }
}
