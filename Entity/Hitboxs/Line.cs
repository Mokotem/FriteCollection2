using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FriteCollection2.Entity.Hitboxs;

public abstract partial class Hitbox
{
    public class Line : Hitbox
    {
        private float angle;
        private Vector2 pos;

        public Line(Space parent, byte layer, float angle, params string[] tags) : base(parent, layer, tags)
        {
            this.angle = angle;
        }

        public Line(Space parent, byte layer, params string[] tags) : this(parent, layer, 0f, tags) { }
        public Line(Space parent, float angle, params string[] tags) : this(parent, 0, angle, tags) { }
        public Line(byte layer, params string[] tags) : this(Space.Zero, layer, 0f, tags) { }
        public Line(float angle, params string[] tags) : this(Space.Zero, 0, angle, tags) { }
        public Line(Space parent, params string[] tags) : this(parent, 0, 0f, tags) { }

        public override bool Check(byte layer, ConditionToCheckCollision condition)
        {
            return true;
        }

        public override void UpdatePosition(float x, float y)
        {
            this.pos.X = x;
            this.pos.Y = y;
        }

        public override void Draw(SpriteBatch batch)
        {

        }
    }
}
