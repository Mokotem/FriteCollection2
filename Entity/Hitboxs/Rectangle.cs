using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;


namespace FriteCollection2.Entity.Hitboxs;


public abstract partial class Hitbox
{
    public partial class Rectangle : Hitbox
    {
        private float left, right, up, down;

        private float _width, _height;
        public float Width => _width;
        public float Height => _height;

        public Vector2 offset;
        private float centerX, centerY;
        public Align isInfinitOnX;
        public Align isInfinitOnY;

        public Rectangle(in Space parent, byte layer, params string[] tags)
            : base(in parent, layer, tags)
        {
            offset = Vector2.Zero;
            SetScale(parent.W, parent.H);
        }

        public Rectangle() : this(Space.Zero, 0) { }
        public Rectangle(byte layer, params string[] tags) : this(Space.Zero, layer, tags) { }
        public Rectangle(in Space parent) : this(in parent, 0) { }
        public Rectangle(in Space parent, params string[] tags) : this(in parent, 0, tags) { }

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

        public bool Check(byte layer, ConditionToCheckCollision condition, out Rectangle collider)
        {
            this.UpdatePosition();

            foreach (Rectangle col in layers[layer])
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
            return Check(layer, condition, out Rectangle _);
        }

        public bool Check(byte layer, ConditionToCheckCollision condition, out Sides globalSide, out CollisionData<Rectangle>[] coliders, out int closestColId)
        {
            this.UpdatePosition();

            float mindx = float.PositiveInfinity, mindy = float.PositiveInfinity;
            bool globalIsFullX = false, globalIsFullY = false;
            bool globalSideIsRight = false, globalSideIsDown = false;

            bool[] corners = new bool[4];

            closestColId = 0;

            List<CollisionData<Rectangle>> result = new List<CollisionData<Rectangle>>();

            foreach (Rectangle col in layers[layer])
            {
                if (col.active && (col != this) && condition(col))
                {
                    col.UpdatePosition();

                    bool isRight, isDown, isfullx, isfully;
                    bool touchLeft, touchRight, touchUp, touchDown;

                    if (!MakeCollisionRange(this.left, this.right, col.left, col.right,
                        out touchLeft, out touchRight,
                        out float dx, out isRight, out isfullx)
                     || !MakeCollisionRange(this.up, this.down, col.up, col.down,
                        out touchUp, out touchDown,
                        out float dy, out isDown, out isfully))
                    {
                        continue;
                    }

                    corners[0] |= touchLeft && touchUp;
                    corners[1] |= touchRight && touchUp;
                    corners[2] |= touchLeft && touchDown;
                    corners[3] |= touchRight && touchDown;

                    if (dx < mindx)
                    {
                        mindx = dx;
                        globalSideIsRight = isRight;
                        closestColId = result.Count;
                    }

                    if (dy < mindy)
                    {
                        mindy = dy;
                        globalSideIsDown = isDown;
                        closestColId = result.Count;
                    }

                    globalIsFullX |= isfullx;
                    globalIsFullY |= isfully;

                    Sides sideCol, second;

                    if (DoIChoseTheSideX(isfullx, isfully, dx, dy))
                    {
                        sideCol = isRight ? Sides.Right : Sides.Left;
                        second = isDown ? Sides.Down : Sides.Up;
                    }
                    else
                    {
                        sideCol = isDown ? Sides.Down : Sides.Up;
                        second = isRight ? Sides.Right : Sides.Left;
                    }

                    result.Add(new CollisionData<Rectangle>(in col, sideCol, second));
                }
            }
                
            coliders = result.ToArray();

            if (result.Count > 0)
            {

                switch (corners[0], corners[1], corners[2], corners[3])
                {
                    case (true, true, false, false):
                        globalSide = Sides.Up;
                        return true;
                    case (false, false, true, true):
                        globalSide = Sides.Down;
                        return true;
                    case (true, false, true, false):
                        globalSide = Sides.Left;
                        return true;
                    case (false, true, false, true):
                        globalSide = Sides.Right;
                        return true;
                    case (true, true, true, true):
                        globalIsFullX = true;
                        globalIsFullY = true;
                        break;
                }

                if (DoIChoseTheSideX(globalIsFullX, globalIsFullY, mindx, mindy))
                    globalSide = globalSideIsRight ? Sides.Right : Sides.Left;
                else
                    globalSide = globalSideIsDown ? Sides.Down : Sides.Up;


                return true;
            }

            globalSide = Sides.Down;
            return false;
        }

        public bool Check(byte layer, ConditionToCheckCollision condition, out Sides globalSide)
        {
            return Check(layer, condition, out globalSide, out _, out _);
        }

        public bool Check(byte layer, string tagToCheck, out Sides globalSide)
        {
            return Check(layer, SelectTag(tagToCheck), out globalSide, out _, out _);
        }

        public bool Check(byte layer, out Sides globalSide)
        {
            return Check(layer, SelectAllHitboxs, out globalSide, out _, out _);
        }

        public bool Check(ConditionToCheckCollision condition, out Sides globalSide)
        {
            return Check(this.layer, condition, out globalSide, out _, out _);
        }

        public bool Check(string tagToCheck, out Sides globalSide)
        {
            return Check(this.layer, SelectTag(tagToCheck), out globalSide, out _, out _);
        }

        public bool Check(out Sides globalSide)
        {
            return Check(this.layer, SelectAllHitboxs, out globalSide, out _, out _);
        }

        public bool Check(byte layer, out Sides side, out CollisionData<Rectangle>[] coliders, out int closestColId)
        {
            return Check(layer, SelectAllHitboxs, out side, out coliders, out closestColId);
        }


        public bool Check(byte layer, string tagToCheck, out Sides side, out CollisionData<Rectangle>[] coliders, out int closestColId)
        {
            return Check(layer, SelectTag(tagToCheck), out side, out coliders, out closestColId);
        }


        public bool Check(string tagToCheck, out Sides side, out CollisionData<Rectangle>[] coliders, out int closestColId)
        {
            return Check(this.layer, SelectTag(tagToCheck), out side, out coliders, out closestColId);
        }

        private static bool MakeCollisionRange(float a, float b, float x, float y,
            out bool touchLeftCorner,
            out bool touchRightCorner,
            out float distance,
            out bool isRight,
            out bool both)
        {
            float dr = b - x;
            touchLeftCorner = false;
            touchRightCorner = false;
            both = false;

            if (dr < 0)
            {
                distance = 0f;
                isRight = false;
                return false;
            }

            float dl = y - a;
            if (dl < 0)
            {
                distance = 0f;
                isRight = false;
                return false;
            }

            if (dr < dl)
            {
                distance = dr;
                isRight = true;
            }
            else
            {
                distance = dl;
                isRight = false;
            }

            if (b <= y)
            {
                touchRightCorner = true;
                if (a >= x)
                {
                    touchLeftCorner = true;
                    both = true;
                    distance = float.Min(y - b, a - x);
                }
            }
            else
            {
                if (a < x)
                {
                    both = true;
                    distance = float.Min(b - y, x - a);
                }
                else
                {
                    touchLeftCorner = true;
                }
            }

            return true;
        }

        private static bool DoIChoseTheSideX(bool isfullx, bool isfully, float dx, float dy)
        {
            if (isfullx == isfully)
            {
                return dx < dy;
            }
            else
            {
                return isfully;
            }
        }

        public override void Draw(in SpriteBatch batch)
        {
            batch.DrawRectangle(
                new RectangleF(left, up, _width, _height), layers[layer].debugColor);
        }

        public Rectangle Copy()
        {
            return null;
        }
    }
}
