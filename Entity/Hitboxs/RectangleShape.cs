using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;


namespace FriteCollection2.Entity.Hitboxs;


public abstract partial class Hitbox
{
    public class RectangleShape : Hitbox
    {
        private float left, right, up, down;
        private float width, height;
        private float centerX, centerY;

        public RectangleShape(in Space parent) : base(in parent)
        {

        }

        protected override void UpdatePosition()
        {
            left = parent.X;
            right = parent.X + parent.W;
            up = parent.Y;
            down = parent.Y + parent.H;

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

        public override void Draw(in SpriteBatch batch)
        {
            batch.DrawRectangle(
                new RectangleF(left, up, width, height), layers[layer].debugColor);
        }
    }
}
