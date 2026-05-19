using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace FriteCollection2.Entity.Hitboxs;

public abstract partial class Hitbox
{
    public class Line : Hitbox
    {
        private readonly RotatableObject parent;
        private float angle;
        private Vector2 pos;
        public float thickness;

        public Line(RotatableObject parent, byte layer, float angle, params string[] tags) : base(parent, layer, tags)
        {
            this.angle = angle;
            this.parent = parent;
        }

        public Line(RotatableObject parent, byte layer, params string[] tags) : this(parent, layer, 0f, tags) { }
        public Line(RotatableObject parent, float angle, params string[] tags) : this(parent, 0, angle, tags) { }
        public Line(RotatableObject parent, params string[] tags) : this(parent, 0, 0f, tags) { }

        public override Vector2 CenterPoint => pos;

        public bool Check(byte layer, ConditionToCheckCollision condition, out Circle collider, out float distance,
            out float dx, out float dy)
        {
            if (!this.active)
            {
                collider = null;
                distance = -1;
                dx = 0;
                dy = 0;
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

                        if (!IsInRange(c.CenterPoint))
                            continue;

                        Vector2 cToThis = this.pos - c.Position;

                        d = float.Abs(Vector2.Dot(cToThis, norme));

                        if (d < c.Radius + thickness)
                        {
                            collider = c;
                            distance = c.Radius + thickness - d;

                            if (float.Cos(angle) > 0)
                            {
                                dx = distance;
                            }
                            else
                            {
                                dx = -distance;
                            }

                            if (float.Sin(angle) > 0)
                            {
                                dy = distance;
                            }
                            else
                            {
                                dy = -distance;
                            }

                            return true;
                        }
                    }
                }
            }

            collider = null;
            distance = -1;
            dx = 0;
            dy = 0;
            return false;
        }

        public bool Check(byte layer, ConditionToCheckCollision condition, out Circle collider)
        {
            return this.Check(layer, condition, out collider, out _, out _, out _);
        }

        public bool Check(byte layer, out Circle collider)
        {
            return this.Check(layer, Hitbox.SelectAllHitboxs, out collider, out _, out _, out _);
        }

        public bool Check(byte layer, string tag, out Circle collider)
        {
            return this.Check(layer, Hitbox.SelectTag(tag), out collider, out _, out _, out _);
        }

        public bool Check(out Circle collider)
        {
            return this.Check(this.layer, Hitbox.SelectAllHitboxs, out collider, out _, out _, out _);
        }

        public bool Check(out Circle collider, out float distance)
        {
            return this.Check(this.layer, Hitbox.SelectAllHitboxs, out collider, out distance, out _, out _);
        }

        public bool Check(string tag, out Circle collider)
        {
            return this.Check(this.layer, Hitbox.SelectTag(tag), out collider, out _, out _, out _);
        }

        public bool Check(string tag, out float dx)
        {
            return this.Check(this.layer, Hitbox.SelectTag(tag), out _, out _, out dx, out float _);
        }

        public bool Check(string tag, out Hitbox.Circle col, out float dx)
        {
            return this.Check(this.layer, Hitbox.SelectTag(tag), out col, out _, out dx, out float _);
        }

        public override bool Check(byte layer, ConditionToCheckCollision condition)
        {
            return this.Check(layer, condition, out _, out _, out _, out _);
        }

        public bool CheckWith(Circle c, out float distance, out float dx, out float dy)
        {
            dx = 0;
            dy = 0;

            if (!this.active || !c.active)
            {
                distance = -1;
                return false;
            }

            this.UpdatePosition();
            c.UpdatePosition();

            if (!IsInRange(c.CenterPoint))
            {
                distance = -1;
                return false;
            }

            float angleNorme = angle + (float.Pi / 2f);
            Vector2 norme = new Vector2(float.Cos(angleNorme), float.Sin(angleNorme));
            Vector2 cToThis = this.pos - c.Position;

            float d = float.Abs(Vector2.Dot(cToThis, norme));

            if (d < c.Radius + thickness)
            {
                distance = c.Radius + thickness - d;

                if (float.Cos(angle) > 0)
                {
                    dx = distance;
                }
                else
                {
                    dx = -distance;
                }

                if (float.Sin(angle) > 0)
                {
                    dy = distance;
                }
                else
                {
                    dy = -distance;
                }

                return true;
            }

            distance = -1;
            return false;
        }

        public bool CheckWith(Circle c)
        {
            return this.CheckWith(c, out _, out _, out _);
        }

        public bool CheckWith(Circle c, out float distance)
        {
            return this.CheckWith(c, out distance, out _, out _);
        }

        public bool CheckWith(Circle c, out float dx, out float dy)
        {
            return this.CheckWith(c, out _, out dx, out dy);
        }

        private bool IsInRange(Vector2 point)
        {
            float dx = float.Cos(angle);
            float dy = float.Sin(angle);

            if (float.Abs(dy) > float.Abs(dy))
            {
                if (dy > 0)
                {
                    return point.Y < pos.Y;
                }
                else
                {
                    return point.Y > pos.Y;
                }
            }
            else
            {
                if (dx > 0)
                {
                    return point.X > pos.X;
                }
                else
                {
                    return point.X < pos.X;
                }
            }
        }

        public override void UpdatePosition(float x, float y)
        {
            this.pos.X = x;
            this.pos.Y = y;
            this.angle = parent.rotation;
        }

        public override void Draw(SpriteBatch batch)
        {
            Vector2 dir = new Vector2(float.Cos(angle), float.Sin(angle));
            Vector2 p1 = pos + (dir * 180) - Space.Camera.ToVector2();
            Vector2 p2 = pos - (dir * 180) - Space.Camera.ToVector2();
            float t = thickness;
            if (t < 1)
                t = 1;

            batch.DrawLine(p1, p2, layers[layer].debugColor, t);
        }
    }
}
