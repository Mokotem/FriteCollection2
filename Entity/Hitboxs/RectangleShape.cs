using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;


namespace FriteCollection2.Entity.Hitboxs;


public abstract partial class Hitbox
{
    public class RectangleShape : Hitbox, ICopy<RectangleShape>
    {
        private float left, right, up, down;

        private float _width, _height;
        public float Width => _width;
        public float Height => _height;

        public Vector2 offset;
        private float centerX, centerY;
        public Align isInfinitOnX;
        public Align isInfinitOnY;

        public RectangleShape(in Space parent, byte layer, params string[] tags)
            : base(in parent, layer, tags)
        {
            offset = Vector2.Zero;
            SetScale(parent.W, parent.H);
        }

        public RectangleShape() : this(Space.Zero, 0) { }
        public RectangleShape(byte layer, params string[] tags) : this(Space.Zero, layer, tags) { }
        public RectangleShape(in Space parent) : this(in parent, 0) { }
        public RectangleShape(in Space parent, params string[] tags) : this(in parent, 0, tags) { }

        public void UpdateScale()
        {
            this.SetScale(parent.W, parent.H);
        }

        public void SetScale(float width, float height)
        {
            this._width = width;
            this._height = height;
        }

        protected override void UpdatePosition()
        {
            left = parent.X + offset.X;
            right = parent.X + _width + offset.X;
            up = parent.Y + offset.Y;
            down = parent.Y + _height + offset.Y;

            centerX = (left + right) / 2f;
            centerY = (up + down) / 2f;
        }

        public bool Check(byte layer, ConditionToCheckCollision condition, out RectangleShape collider)
        {
            this.UpdatePosition();

            foreach (RectangleShape col in layers[layer])
            {
                if (col.active && (col != this) && condition(col))
                {
                    col.UpdatePosition();
                    if (!(right < col.left || left > col.right)  // truc de batard du prof de bdd
                    && !(down < col.up || up > col.down))
                    {
                        collider = col;
                        return true;
                    }
                }
            }

            collider = null;
            return false;
        }

        public override bool Check(byte layer, ConditionToCheckCollision condition)
        {
            return this.Check(layer, condition, out _);
        }

        public override void Draw(in SpriteBatch batch)
        {
            batch.DrawRectangle(
                new RectangleF(left, up, _width, _height), layers[layer].debugColor);
        }

        public RectangleShape Copy()
        {
            throw new NotImplementedException();
        }
    }
}
