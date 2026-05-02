using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace FriteCollection2.Entity.Hitboxs;

public abstract partial class Hitbox
{
    public class Line : Hitbox
    {
        public float angle;
        private Vector2 pos;
        public float thickness;

        public Line(Space parent, byte layer, float angle, params string[] tags) : base(parent, layer, tags)
        {
            this.angle = angle;
        }

        public Line(Space parent, byte layer, params string[] tags) : this(parent, layer, 0f, tags) { }
        public Line(Space parent, float angle, params string[] tags) : this(parent, 0, angle, tags) { }
        public Line(byte layer, params string[] tags) : this(Space.Zero, layer, 0f, tags) { }
        public Line(float angle, params string[] tags) : this(Space.Zero, 0, angle, tags) { }
        public Line(Space parent, params string[] tags) : this(parent, 0, 0f, tags) { }

        public bool Check(byte layer, ConditionToCheckCollision condition, out Circle collider, out float distance)
        {
            if (!this.active)
            {
                collider = null;
                distance = -1;
                return false;
            }

            base.UpdatePosition();

            float angleNorme = angle + (float.Pi / 2f);
            Vector2 norme = new Vector2(float.Cos(angleNorme), float.Sin(angleNorme));
            float d;

            foreach (Hitbox hit in layers[layer])
            {
                if (hit.active && hit != this && condition(hit))
                {
                    if (hit is Circle)
                    {
                        Circle c = (Circle)hit;
                        c.UpdatePosition();

                        Vector2 cToThis = this.pos - c.Position;

                        d = Vector2.Dot(cToThis, norme);

                        if (d < c.Radius + thickness)
                        {
                            collider = c;
                            distance = c.Radius + thickness - d;
                            return true;
                        }
                    }
                }
            }

            collider = null;
            distance = -1;
            return false;
        }

        public bool Check(byte layer, ConditionToCheckCollision condition, out Circle collider)
        {
            return this.Check(layer, condition, out collider, out _);
        }

        public bool Check(byte layer, out Circle collider)
        {
            return this.Check(layer, Hitbox.SelectAllHitboxs, out collider, out _);
        }

        public bool Check(byte layer, string tag, out Circle collider)
        {
            return this.Check(layer, Hitbox.SelectTag(tag), out collider, out _);
        }

        public bool Check(out Circle collider)
        {
            return this.Check(this.layer, Hitbox.SelectAllHitboxs, out collider, out _);
        }

        public bool Check(out Circle collider, out float distance)
        {
            return this.Check(this.layer, Hitbox.SelectAllHitboxs, out collider, out distance);
        }

        public bool Check(string tag, out Circle collider)
        {
            return this.Check(this.layer, Hitbox.SelectTag(tag), out collider, out _);
        }

        public override bool Check(byte layer, ConditionToCheckCollision condition)
        {
            return this.Check(layer, condition, out _, out _);
        }

        public bool CheckWith(Circle c, out float distance)
        {
            if (!this.active || !c.active)
            {
                distance = -1;
                return false;
            }

            this.UpdatePosition();

            float angleNorme = angle + (float.Pi / 2f);
            Vector2 norme = new Vector2(float.Cos(angleNorme), float.Sin(angleNorme));
            Vector2 cToThis = this.pos - c.Position;

            float d = float.Abs(Vector2.Dot(cToThis, norme));

            if (d < c.Radius + thickness)
            {
                distance = c.Radius + thickness - d;
                return true;
            }

            distance = -1;
            return false;
        }

        public bool CheckWith(Circle c)
        {
            return this.CheckWith(c, out _);
        }

        public override void UpdatePosition(float x, float y)
        {
            this.pos.X = x;
            this.pos.Y = y;
        }

        public override void Draw(SpriteBatch batch)
        {
            Vector2 dir = new Vector2(float.Cos(angle), float.Sin(angle));
            Vector2 p1 = pos + (dir * 180);
            Vector2 p2 = pos - (dir * 180);
            float t = thickness;
            if (t < 1)
                t = 1;

            batch.DrawLine(p1, p2, layers[layer].debugColor, t);
        }
    }
}
