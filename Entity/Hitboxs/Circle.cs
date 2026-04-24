using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using MonoGame.Extended;

namespace FriteCollection2.Entity.Hitboxs;

public abstract partial class Hitbox
{
    public class Circle : Hitbox
    {
        private CircleF circle;


        public Circle(Space parent, byte layer, params string[] tags) : base(parent, layer, tags)
        {
            circle = new CircleF(parent.CenterPoint, parent.W);
        }

        public Circle(Space parent, params string[] tags) : this(parent, 0, tags) { }
        public Circle(byte layer, params string[] tags) : this(Space.Zero, layer, tags) { }

        public float Radius
        {
            get => circle.Radius;
            set => circle.Radius = value;
        }

        public Vector2 Position
        {
            get => circle.Position;
            set => circle.Position = value;
        }

        public bool Check(byte layer, ConditionToCheckCollision condition, out Hitbox collider, out float marge)
        {
            base.UpdatePosition();

            foreach (Hitbox hit in layers[layer])
            {
                if (hit != this && condition(hit))
                {
                    if (hit is Circle)
                    {
                        Circle c = (Circle)hit;
                        c.UpdatePosition();

                        marge = c.circle.Radius + this.circle.Radius
                            - Vector2.Distance(c.circle.Center, this.circle.Center);

                        if (marge > 0)
                        {
                            collider = c;
                            return true;
                        }
                    }
                    else if (hit is Line)
                    {
                        Line l = (Line)hit;

                        if (l.CheckWith(this, out marge))
                        {
                            collider = l;
                            return true;
                        }
                    }
                }
            }

            collider = null;
            marge = -1;
            return false;
        }

        public bool Check(byte layer, ConditionToCheckCollision condition, out Hitbox collider)
        {
            return this.Check(layer, condition, out collider, out _);
        }

        public override bool Check(byte layer, ConditionToCheckCollision condition)
        {
            return this.Check(layer, condition, out _, out _);
        }


        public override void UpdatePosition(float x, float y)
        {
            this.circle.Center.X = x;
            this.circle.Center.Y = y;
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.DrawCircle(circle, (int)circle.Radius, layers[layer].debugColor);
        }
    }
}
