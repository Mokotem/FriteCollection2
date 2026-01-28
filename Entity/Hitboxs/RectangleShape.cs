using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;


namespace FriteCollection2.Entity.Hitboxs;


public abstract partial class Hitbox
{
    public class RectangleShape : Hitbox
    {
        private float left, right, up, down;
        private float width, height;
        private float offsetX, offsetY;
        private float centerX, centerY;

        public RectangleShape(in Space parent, byte layer, params string[] tags)
            : base(in parent, layer, tags)
        {
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
            this.width = width;
            this.height = height;
        }

        protected override void UpdatePosition()
        {
            left = parent.X + offsetX;
            right = parent.X + width + offsetX;
            up = parent.Y + offsetY;
            down = parent.Y + height + offsetY;

            centerX = (left + right) / 2f;
            centerY = (up + down) / 2f;
        }

        public bool Check(byte layer, ConditionToCheckCollision condition, out RectangleShape collider)
        {
            foreach (RectangleShape col in layers[layer])
            {
                if (col.active
                    && condition(col)
                    && !(right < col.left || left > col.right)  // truc de batard du prof de bdd
                    && !(down < col.up || up > col.down))
                {
                    collider = col;
                    return true;
                }
            }

            collider = null;
            return false;
        }

        public override bool Check(byte layer, ConditionToCheckCollision condition)
        {
            return this.Check(layer, condition, out _);
        }

        public bool AdvancedCheck()
        {

        }

        public override void Draw(in SpriteBatch batch)
        {
            batch.DrawRectangle(
                new RectangleF(left, up, width, height), layers[layer].debugColor);
        }
    }
}
