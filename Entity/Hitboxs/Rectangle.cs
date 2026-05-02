using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace FriteCollection2.Entity.Hitboxs;


public abstract partial class Hitbox
{
    public partial class Rectangle : Hitbox
    {
        public delegate float Critere(Rectangle rect);

        private float _left, _right, _up, _down;

        private float _width, _height;
        public float Width => _width;
        public float Height => _height;

        public float Left => _left;
        public float Right => _right;
        public float Up => _up;
        public float Down => _down;

        public Point Size => new Point(
            (int)float.Round(_width),
            (int)float.Round(_height)
            );

        public Point offset = Point.Zero;
        public bool infinitLeft, infinitRight, infinitUp, infinitDown;
        public Sides alwaysCollideWidthSide;
        public void SetAlwaysCollideBasedOnInfinit()
        {
            switch (infinitUp, infinitDown, infinitLeft, infinitRight)
            {
                case (true, true, true, false):
                alwaysCollideWidthSide = Sides.Left;
                return;

                case (true, true, false, true):
                alwaysCollideWidthSide = Sides.Right;
                return;

                case (true, false, true, true):
                alwaysCollideWidthSide = Sides.Up;
                return;

                case (false, true, true, true):
                alwaysCollideWidthSide = Sides.Down;
                return;
            }
        }

        public Vector2 CenterPoint => new Vector2((_left + _right) / 2f, (_up + _down) / 2f);

        public Rectangle(Space parent, byte layer, params string[] tags)
            : base(parent, layer, tags)
        {
            alwaysCollideWidthSide = Sides.Center;
            SetScale(parent.W, parent.H);
        }

        public Rectangle(Space parent, string tag = "", byte layer = 0) : this(parent, layer, tag)
        {

        }

        public Rectangle() : this(Space.Zero, 0) { }
        public Rectangle(byte layer, params string[] tags) : this(Space.Zero, layer, tags) { }
        public Rectangle(Space parent) : this(parent, 0) { }
        public Rectangle(Space parent, params string[] tags) : this(parent, 0, tags) { }

        public void UpdateScale()
        {
            this.SetScale(_parent.W, _parent.H);
        }

        public void SetScale(float width, float height)
        {
            this._width = width;
            this._height = height;
        }

        public void SetScale(Point value)
        {
            SetScale(value.X, value.Y);
        }


        public override void UpdatePosition(float x, float y)
        {
            _left = x + offset.X;
            _right = x + _width + offset.X;
            _up = y + offset.Y;
            _down = y + _height + offset.Y;
        }


        private bool Intersect(Rectangle col)
        {
            if (!col.infinitRight && _left > col._right)
                return false;
            if (!col.infinitLeft && _right < col._left)
                return false;
            if (!col.infinitDown && _up > col._down)
                return false;
            if (!col.infinitUp && _down < col._up)
                return false;
            return true;
        }

        public bool Check(byte layer, ConditionToCheckCollision condition, out Rectangle collider)
        {
            if (this.active)
            {
                this.UpdatePosition();

                Rectangle col;

                foreach (Hitbox hit in layers[layer])
                {
                    if (hit is Rectangle)
                    {
                        col = (Rectangle)hit;
                    }
                    else
                        continue;

                    if (col.active && (col != this) && condition(col))
                    {
                        col.UpdatePosition();
                        if (Intersect(col))
                        {
                            collider = col;
                            return true;
                        }
                    }
                }
            }

            collider = null;
            return false;
        }

        public bool Check(ConditionToCheckCollision condition, out Rectangle collider)
        {
            return Check(this.layer, condition, out collider);
        }

        public override bool Check(byte layer, ConditionToCheckCollision condition)
        {
            return Check(layer, condition, out _);
        }

        public bool Check(out Rectangle collider)
        {
            return Check(this.layer, SelectAllHitboxs, out collider);
        }

        public bool Check(byte layer, ConditionToCheckCollision condition, out Sides globalSide, out CollisionData<Rectangle>[] coliders)
        {
            if (!this.active)
            {
                globalSide = Sides.Center;
                coliders = null;
                return false;
            }

            this.UpdatePosition();

            globalSide = Sides.Center;
            bool[] corners = new bool[4];

            List<CollisionData<Rectangle>> result = new List<CollisionData<Rectangle>>();

            Rectangle col;

            foreach (Hitbox hit in layers[layer])
            {
                if (hit is Rectangle)
                {
                    col = (Rectangle)hit;
                }
                else
                    continue;

                if (col.active && (col != this) && condition(col))
                {
                    col.UpdatePosition();

                    bool isRight, isDown, isfullx, isfully;
                    bool touchLeft, touchRight, touchUp, touchDown;

                    if (!MakeCollisionRange(this._left, this._right, col._left, col._right,
                        col.infinitLeft, col.infinitRight,
                        out touchLeft, out touchRight,
                        out float dx, out isRight, out isfullx)
                     || !MakeCollisionRange(this._up, this._down, col._up, col._down,
                        col.infinitUp, col.infinitDown,
                        out touchUp, out touchDown,
                        out float dy, out isDown, out isfully))
                    {
                        continue;
                    }


                    corners[0] |= touchLeft && touchUp;
                    corners[1] |= touchRight && touchUp;
                    corners[2] |= touchLeft && touchDown;
                    corners[3] |= touchRight && touchDown;

                    Sides sideCol;
                    if (col.alwaysCollideWidthSide == Sides.Center)
                    {
                        if (DoIChoseTheSideX(isfullx, isfully, dx, dy))
                        {
                            sideCol = isRight ? Sides.Right : Sides.Left;
                        }
                        else
                        {
                            sideCol = isDown ? Sides.Down : Sides.Up;
                        }
                    }
                    else
                        sideCol = col.alwaysCollideWidthSide;

                    result.Add(new CollisionData<Rectangle>(col, sideCol));
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
                }

                if (result.Count > 1)
                {
                    if (CheckIfIsSameSide(coliders[0], coliders[1],
                        (Rectangle r) => r._down,
                        Sides.Up))
                    {
                        globalSide = Sides.Up;
                        return true;
                    }

                    if (CheckIfIsSameSide(coliders[0], coliders[1],
                        (Rectangle r) => r._up,
                        Sides.Down))
                    {
                        globalSide = Sides.Down;
                        return true;
                    }

                    if (CheckIfIsSameSide(coliders[0], coliders[1],
                        (Rectangle r) => r._right,
                        Sides.Left))
                    {
                        globalSide = Sides.Left;
                        return true;
                    }

                    if (CheckIfIsSameSide(coliders[0], coliders[1],
                        (Rectangle r) => r._left,
                        Sides.Right))
                    {
                        globalSide = Sides.Right;
                        return true;
                    }
                }

                return true;
            }

            globalSide = Sides.Center;
            coliders = null;
            return false;
        }

        private delegate float GetRectangleSide(Rectangle rect);

        private static bool CheckIfIsSameSide(
            CollisionData<Rectangle> col1,
            CollisionData<Rectangle> col2,
            GetRectangleSide side,
            Sides sideToCheck)
        {
            if (col1.side == sideToCheck || col2.side == sideToCheck)
            {
                if (float.Abs(side(col1.collider) - side(col2.collider)) < 1f)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool MakeCollisionRange(float a, float b, float x, float y,
            bool ileft, bool iright,
            out bool touchLeftCorner,
            out bool touchRightCorner,
            out float distance,
            out bool isRight,
            out bool both)
        {
            if (ileft && iright)
            {
                distance = -1;
                isRight = true;
                both = true;
                touchLeftCorner = true;
                touchRightCorner = true;
                return true;
            }

            float dr = b - x;
            touchLeftCorner = false;
            touchRightCorner = false;
            both = false;

            if (dr <= 0 && !ileft)
            {
                distance = 0f;
                isRight = false;
                return false;
            }

            float dl = y - a;
            if (dl <= 0 && !iright)
            {
                distance = 0f;
                isRight = true;
                return false;
            }

            if (dr < 0 || dl < 0)
            {
                distance = -1f;
                both = true;
                isRight = false;
                if (ileft && !iright)
                {
                    touchLeftCorner = true;
                    touchRightCorner = false;
                    isRight = false;
                    both = false;
                }
                else if (iright && !ileft)
                {
                    touchRightCorner = true;
                    touchLeftCorner = false;
                    isRight = true;
                    both = false;
                }
                return true;
            }

            if (ileft)
            {
                isRight = false;
                touchLeftCorner = true;
                distance = dl;
                return true;
            }

            if (iright)
            {
                isRight = true;
                touchRightCorner = true;
                distance = dr;
                return true;
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

        public bool Check(ConditionToCheckCollision condition, out Sides globalSide, out CollisionData<Rectangle>[] coliders)
        {
            return Check(this.layer, condition, out globalSide, out coliders);
        }

        public bool Check(out Sides globalSide, string tagToCheck, out CollisionData<Rectangle>[] coliders)
        {
            return Check(this.layer, SelectTag(tagToCheck), out globalSide, out coliders);
        }

        public bool Check(out Sides globalSide, out CollisionData<Rectangle>[] coliders)
        {
            return Check(this.layer, SelectAllHitboxs, out globalSide, out coliders);
        }

        public static bool Check(byte layer, Vector2 point, ConditionToCheckCollision condition, out Rectangle collider)
        {
            foreach(Hitbox rect in layers[layer])
            {
                if (rect.active && rect is Rectangle && condition(rect))
                {
                    Rectangle r = (Rectangle)rect;
                    rect.UpdatePosition();
                    if (point.X > r._left && point.X < r._right
                        && point.Y > r._up && point.Y < r._down)
                    {
                        collider = r;
                        return true;
                    }
                }
            }

            collider = null;
            return false;
        }

        public static bool Check(Vector2 point, ConditionToCheckCollision condition)
        {
            return Check(0, point, condition, out _);
        }

        public static bool Check(Vector2 point)
        {
            return Check(0, point, SelectAllHitboxs, out _);
        }

        public static bool Check(byte layer, Vector2 point, string tagToCheck)
        {
            return Check(layer, point, SelectTag(tagToCheck), out _);
        }

        public static bool Check(Vector2 point, string tagToCheck)
        {
            return Check(0, point, SelectTag(tagToCheck), out _);
        }

        public static bool Check(Vector2 point, ConditionToCheckCollision condition, out Rectangle collider)
        {
            return Check(0, point, condition, out collider);
        }

        public static bool Check(Vector2 point, out Rectangle collider)
        {
            return Check(0, point, SelectAllHitboxs, out collider);
        }

        public static bool Check(byte layer, Vector2 point, string tagToCheck, out Rectangle collider)
        {
            return Check(layer, point, SelectTag(tagToCheck), out collider);
        }

        public static bool Check(Vector2 point, string tagToCheck, out Rectangle collider)
        {
            return Check(0, point, SelectTag(tagToCheck), out collider);
        }


        public Sides CheckWith(Rectangle col, out bool[] corners)
        {
            if (!this.active || !col.active)
            {
                corners = null;
                return Sides.Center;
            }

            corners = new bool[4];

            this.UpdatePosition();
            col.UpdatePosition();

            bool isRight, isDown, isfullx, isfully;
            bool touchLeft, touchRight, touchUp, touchDown;

            if (!MakeCollisionRange(this._left, this._right, col._left, col._right,
                col.infinitLeft, col.infinitRight,
                out touchLeft, out touchRight,
                out float dx, out isRight, out isfullx)
             || !MakeCollisionRange(this._up, this._down, col._up, col._down,
                col.infinitUp, col.infinitDown,
                out touchUp, out touchDown,
                out float dy, out isDown, out isfully))
            {
                return Sides.Center;
            }

            Sides sideCol;

            if (DoIChoseTheSideX(isfullx, isfully, dx, dy))
            {
                sideCol = isRight ? Sides.Right : Sides.Left;
            }
            else
            {
                sideCol = isDown ? Sides.Down : Sides.Up;
            }

            corners[0] |= touchLeft && touchUp;
            corners[1] |= touchRight && touchUp;
            corners[2] |= touchLeft && touchDown;
            corners[3] |= touchRight && touchDown;

            return sideCol;
        }

        public bool CheckWith(Rectangle col)
        {
            if (!this.active || !col.active)
            {
                return false;
            }

            this.UpdatePosition();
            col.UpdatePosition();

            return !(this._right < col._left || this._left > col._right
                || this._up > col._down || this._down < col._up);
        }

        public Microsoft.Xna.Framework.Rectangle ToRectangle()
        {
            return new Microsoft.Xna.Framework.Rectangle(
                (int)float.Round(_left),
                (int)float.Round(_up),
                (int)float.Round(_width),
                (int)float.Round(_height));
        }

        private static bool DoIChoseTheSideX(bool isfullx, bool isfully, float dx, float dy)
        {
            if (dx < 0)
                return true;
            if (dy < 0)
                return false;

            if (isfullx == isfully)
            {
                return dx < dy;
            }
            else
            {
                return isfully;
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            if (active)
            {
                batch.DrawRectangle(
                    new RectangleF(_left - Space.Camera.X, _up - Space.Camera.Y, _width, _height), layers[layer].debugColor,
                    layerDepth: 0);
            }
        }

        public Rectangle Copy()
        {
            Rectangle r = new Rectangle(this._parent, _tags);
            r._left = this._left;
            r._up = this._up;
            r._down = this._down;
            r._right = this._right;
            r._width = this._width;
            r._height = this._height;
            return r;
        }

        public override string ToString()
        {
            return this.Tags[0] + " " + _width + " " + _height
                + (isStatic ? " static" : "")
                + (infinitUp ? " up" : "")
                + (infinitDown ? " down" : "")
                + (infinitLeft ? " left" : "")
                + (infinitRight ? " right" : "");
        }
    }
}
