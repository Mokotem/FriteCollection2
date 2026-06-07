

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using System.Collections.Generic;

namespace FriteCollection2._3D;

public class TriangleDrawer
{
    private readonly int width, height;

    private BasicEffect basicEffect;
    private VertexBuffer vertexBuffer;

    private readonly List<Vector2> triangles;
    private VertexPositionColor[] data;
    private RasterizerState rasterizerState;

    public TriangleDrawer(int width, int height, Effect effect, GraphicsDevice device)
    {
        this.width = width;
        this.height = height;

        triangles = new List<Vector2>();


        basicEffect = new BasicEffect(device);
        basicEffect.CurrentTechnique = effect.CurrentTechnique;
        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(width / 2, -height / 2, 16), new Vector3(width / 2, -height / 2, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreateOrthographic(width, height, 0, 32);
        basicEffect.World = world;
        basicEffect.View = view;
        basicEffect.Projection = projection;
        basicEffect.VertexColorEnabled = false;

        rasterizerState = new RasterizerState();
        rasterizerState.CullMode = CullMode.None;
        device.RasterizerState = rasterizerState;

        vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColor), 128, BufferUsage.WriteOnly);


        data = new VertexPositionColor[0];

    }

    public int AddTriangle(Vector2[] points)
    {
        if (points.Length != 3)
        {
            throw new System.Exception();
        }

        return this.AddTriangle(points[0], points[1], points[2]);
    }

    public int AddTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        int id = triangles.Count;
        triangles.AddRange(p1, p2, p3);
        return id;
    }

    public void SetTriangle(int id, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        triangles[id] = p1;
        triangles[id + 1] = p2;
        triangles[id + 2] = p3;
    }

    public void SetTriangle(int id, byte num, Vector2 value)
    {
        if (num > 2)
        {
            throw new System.Exception();
        }
        triangles[id + num] = value;
    }

    public void SetTriangle(int id, Vector2[] points)
    {
        if (points.Length != 3)
        {
            throw new System.Exception();
        }

        this.SetTriangle(id, points[0], points[1], points[2]);
    }

    public void Clear()
    {
        triangles.Clear();
    }

    public int Count => data.Length / 3;

    public void Apply(Color c)
    {
        if (triangles.Count > 0)
        {
            if (data.Length != triangles.Count)
            {
                data = new VertexPositionColor[triangles.Count];
            }

            for (int i = 0; i < triangles.Count; i++)
            {
                data[i] = new VertexPositionColor(new Vector3(triangles[i].X, triangles[i].Y, 0.5f), c);
            }

            vertexBuffer.SetData<VertexPositionColor>(data);
        }
    }

    public void Draw(SpriteBatch batch, GraphicsDevice device)
    {
        if (data.Length > 0 && triangles.Count > 0)
        {
            device.SetVertexBuffer(vertexBuffer);
            RasterizerState state = new RasterizerState();
            state.CullMode = CullMode.None;
            device.RasterizerState = state;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, Count);
            }
        }
    }
}
