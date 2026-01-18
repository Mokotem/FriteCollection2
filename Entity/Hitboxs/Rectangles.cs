using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FriteCollection2.Entity.Hitboxs;

public partial class Hitbox
{
    public class Rectangle : Hitbox, IEnumerable, ICollider, IDraw, ICopy<Rectangle>
    {
        public bool ApplyColision(Collision<Rectangle> col, Vector2 vitesse, Action OnUp, Action OnDown, Action OnLeft, Action OnRight)
        {
            switch (col.side)
            {
                case Sides.Down:
                    if (vitesse.Y >= 0)
                    {
                        SpaceRef.Y = col.collider.Top - Size.Y - PositionOffset.Y;
                        OnDown();
                        return true;
                    }
                    else return false;
                case Sides.Up:
                    if (vitesse.Y <= 0)
                    {
                        SpaceRef.Y = col.collider.Bottom - PositionOffset.Y;
                        OnUp();
                        return true;
                    }
                    else return false;
                case Sides.Left:
                    if (vitesse.X <= 0)
                    {
                        SpaceRef.X = col.collider.Right - PositionOffset.X;
                        OnLeft();
                        return true;
                    }
                    else return false;
                case Sides.Right:
                    if (vitesse.X >= 0)
                    {
                        SpaceRef.X = col.collider.Left - Size.X - PositionOffset.X;
                        OnRight();
                        return true;
                    }
                    else return false;
                default:
                    return false;
            }
        }

        public bool ApplyColision(Collision<Rectangle> col, Vector2 vitesse, Action OnUp, Action OnDown, Action OnLeft, Action OnRight,
            ref Vector2 position)
        {
            switch (col.side)
            {
                case Sides.Down:
                    if (vitesse.Y >= 0)
                    {
                        position.Y = col.collider.Top - Size.Y - PositionOffset.Y;
                        OnDown();
                        return true;
                    }
                    else return false;
                case Sides.Up:
                    if (vitesse.Y <= 0)
                    {
                        position.Y = col.collider.Bottom - PositionOffset.Y;
                        OnUp();
                        return true;
                    }
                    else return false;
                case Sides.Left:
                    if (vitesse.X <= 0)
                    {
                        position.X = col.collider.Right - PositionOffset.X;
                        OnLeft();
                        return true;
                    }
                    else return false;
                case Sides.Right:
                    if (vitesse.X >= 0)
                    {
                        position.X = col.collider.Left - Size.X - PositionOffset.X;
                        OnRight();
                        return true;
                    }
                    else return false;
                default:
                    return false;
            }
        }

        private struct RectangleEnum : IEnumerator
        {
            public Vector2 _point, _p2;
            public short index;

            public RectangleEnum(Vector2 p1, Vector2 p2)
            {
                _point = p1;
                _p2 = p2;
                index = -1;
            }

            public bool MoveNext()
            {
                index += 1;
                if (index > 3)
                    return false;
                return true;
            }

            void IEnumerator.Reset()
            {
                index = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    return new Vector2(
                        index % 2 == 0 ? _point.X : _p2.X,
                        index < 2 ? _point.Y : _p2.Y
                        );
                }
            }
        }

        private RectangleEnum rectenum;
        IEnumerator IEnumerable.GetEnumerator()
        {
            rectenum._point = this._point;
            rectenum._p2 = this.p2;
            rectenum.index = -1;
            return rectenum;
        }

        public Rectangle(in Space _space, string tag = "", byte layer = 0, byte secondaryLayer = 255) : base(in _space, tag, layer, secondaryLayer)
        {
            rectenum = new RectangleEnum();
            this.UpdatePos();
        }

        public Align IsInfinitOnY = Align.Center;
        public Align IsInfinitOnX = Align.Center;

        private Vector2 p2;

        public float Left => _point.X;
        public float Right => p2.X;
        public float Top => _point.Y;
        public float Bottom => p2.Y;

        private bool sizeLocked = false;
        private Point lockSize;
        public void LockSize(Point value)
        {
            sizeLocked = true;
            lockSize = value;
        }

        public void LockSize(int w, int h)
        {
            sizeLocked = true;
            lockSize = new Point(w, h);
        }

        public override Point Size => lockSize;

        public void UnlockSize()
        {
            sizeLocked = false;
        }

        public override void LockPosition(Vector2 value)
        {
            positionLocked = true;
            _point = value;
        }

        private protected override void UpdatePos()
        {
            if (!isStatic)
            {
                if (!positionLocked)
                    _point = SpaceRef.Position;

                if (sizeLocked)
                    p2 = new Vector2(_point.X + lockSize.X, _point.Y + lockSize.Y);
                else
                    p2 = new Vector2(_point.X + SpaceRef.Scale.X, _point.Y + SpaceRef.Scale.Y);

                _point.X += PositionOffset.X;
                _point.Y += PositionOffset.Y;
                p2.X += PositionOffset.X;
                p2.Y += PositionOffset.Y;
                _centerPoint = new Vector2((_point.X + p2.X) / 2f, (_point.Y + p2.Y) / 2f);
            }
        }

        private protected void UpdatePos(Vector2 cp)
        {
            if (!isStatic)
            {
                UpdatePos();
                if (_centerPoint.Y < cp.Y)
                    IsInfinitOnY = Align.Left;
                else
                    IsInfinitOnY = Align.Right;
                if (_centerPoint.X < cp.X)
                    IsInfinitOnX = Align.Left;
                else
                    IsInfinitOnX = Align.Right;
            }
        }

        public Rectangle Copy()
        {
            return new Rectangle(SpaceRef, _tag, this._layer)
            {
                _point = this._point,
                p2 = this.p2,
                PositionOffset = this.PositionOffset
            };
        }

        private bool InRange(Vector2 _1, Vector2 _2)
        {
            if ((_point.X > _1.X && _point.X < _2.X) || (p2.X > _1.X && p2.X < _2.X))
            {
                if (_point.Y > _1.Y)
                {
                    if (_point.Y < _2.Y)
                    {
                        return true;
                    }
                }
                else if (p2.Y > _1.Y)
                {
                    return true;
                }
            }
            else if ((_point.Y > _1.Y && _point.Y < _2.Y) || (p2.Y > _1.Y && p2.Y < _2.Y))
            {
                if (_point.X > _1.X)
                {
                    if (_point.X < _2.X)
                    {
                        return true;
                    }
                }
                else if (p2.X > _1.X)
                {
                    return true;
                }
            }

            return false;
        }

        private bool InRangeVertical(Vector2 _1, Vector2 _2)
        {
            if ((_point.Y > _1.Y && _point.Y < _2.Y) || (p2.Y > _1.Y && p2.Y < _2.Y))
            {
                return true;
            }

            return false;
        }

        private bool IsCrossedVertical(Vector2 _1, Vector2 _2)
        {
            if (_point.X > _1.X && p2.X < _2.X
             && _point.Y <= _1.Y && p2.Y > _2.Y)
            {
                return true;
            }

            return false;
        }

        private static bool PointInRange(Vector2 p, Vector2 _1, Vector2 _2, bool up, bool right, out float distX, out float distY)
        {
            if (PointInRange(p, _1, _2))
            {
                distX = right ? p.X - _1.X : _2.X - p.X;
                distY = up ? _2.Y - p.Y : p.Y - _1.Y;
                return true;
            }

            distX = float.PositiveInfinity;
            distY = float.PositiveInfinity;
            return false;
        }

        private static bool PointInRange(Vector2 p, Vector2 _1, Vector2 _2, Align infinitX, Align infinitY, bool up, bool right, out float distX, out float distY)
        {
            if (PointInRange(p, _1, _2, infinitX, infinitY))
            {
                distX = right ? p.X - _1.X : _2.X - p.X;
                distY = up ? _2.Y - p.Y : p.Y - _1.Y;
                return true;
            }

            distX = float.PositiveInfinity;
            distY = float.PositiveInfinity;
            return false;
        }

        protected static bool PointInRange(Vector2 p, Vector2 _1, Vector2 _2)
        {
            return p.X >= _1.X && p.X <= _2.X && p.Y >= _1.Y && p.Y <= _2.Y;
        }

        protected static bool PointInRange(Vector2 p, Vector2 _1, Vector2 _2, Align infinitX, Align infinitY)
        {
            bool x;

            if (infinitX == Align.Left)
            {
                x = p.X <= _2.X;
            }
            else if (infinitX == Align.Right)
            {
                x = p.X >= _1.X;
            }
            else
            {
                x = p.X >= _1.X && p.X <= _2.X;
            }

            if (x)
            {
                if (infinitY == Align.Left)
                {
                    return p.Y <= _2.Y;
                }
                else if (infinitY == Align.Right)
                {
                    return p.Y >= _1.Y;
                }
                else
                {
                    return p.Y >= _1.Y && p.Y <= _2.Y;
                }
            }

            return false;
        }

        public bool PointInRange(Vector2 p)
        {
            bool x;

            if (this.IsInfinitOnX == Align.Left)
            {
                x = p.X <= p2.X;
            }
            else if (this.IsInfinitOnX == Align.Right)
            {
                x = p.X >= _point.X;
            }
            else
            {
                x = p.X >= _point.X && p.X <= p2.X;
            }

            if (x)
            {
                if (this.IsInfinitOnY == Align.Left)
                {
                    return p.Y <= p2.Y;
                }
                else if (this.IsInfinitOnY == Align.Right)
                {
                    return p.Y >= _point.Y;
                }
                else
                {
                    return p.Y >= _point.Y && p.Y <= p2.Y;
                }
            }

            return false;
        }

        public Vector2 CenterPoint => _centerPoint;
        private Vector2 _centerPoint;

        private byte _CountBools(bool[] b)
        {
            byte n = 0;
            foreach (bool b2 in b)
            {
                if (b2) n++;
            }
            return n;
        }

        private static Sides ApplyInfinit(Sides side, Align x, Align y)
        {
            switch (side)
            {
                case Sides.Up:
                    if (y == Align.Right)
                        return Sides.Down;
                    return Sides.Up;

                case Sides.Down:
                    if (y == Align.Left)
                        return Sides.Up;
                    return Sides.Down;

                case Sides.Left:
                    if (x == Align.Right)
                        return Sides.Right;
                    return Sides.Left;

                case Sides.Right:
                    if (x == Align.Left)
                        return Sides.Left;
                    return Sides.Right;

                default:
                    return side;
            }
        }

        private bool _MakeCollisionWith(
            Hitbox.Rectangle hit,
            out Collision<Rectangle> col,
            ref readonly bool[] global)
        {
            hit.UpdatePos();
            col = new Collision<Rectangle>();
            bool[] bools = new bool[4] { false, false, false, false };
            bool colided = false;

            Vector2 closePoint = new Vector2(-1, -1);

            byte i = 0;
            foreach (Vector2 p in this)
            {
                if (PointInRange(p, hit._point, hit.p2, hit.IsInfinitOnX, hit.IsInfinitOnY, i < 2, i % 2 == 1, out float dx, out float dy))
                {
                    bools[i] = true;
                    global[i] = true;
                    colided = true;

                    if (closePoint.X < 0 || closePoint.Y < 0)
                        closePoint = new Vector2(dx, dy);
                    else
                        closePoint = new Vector2(
                            MathF.Min(closePoint.X, dx),
                            MathF.Min(closePoint.Y, dy));
                }

                i += 1;
            }
            i = 0;
            foreach (Vector2 p in hit)
            {
                if (PointInRange(p))
                {
                    bools[3 - i] = true;
                    global[3 - i] = true;
                    colided = true;
                }

                i += 1;
            }

            if (colided)
            {
                byte n = _CountBools(bools);
                if (n == 2)
                {
                    switch (bools[0], bools[1], bools[2], bools[3])
                    {
                        case (true, true, false, false):
                            col.side = Sides.Up;
                            break;

                        case (false, false, true, true):
                            col.side = Sides.Down;
                            break;

                        case (true, false, true, false):
                            col.side = Sides.Left;
                            break;

                        case (false, true, false, true):
                            col.side = Sides.Right;
                            break;
                    }
                }
                else
                {
                    if (closePoint.X < closePoint.Y)
                    {
                        if (_centerPoint.X > hit.CenterPoint.X)
                        {
                            col.side = Sides.Left;
                        }
                        else
                        {
                            col.side = Sides.Right;
                        }
                    }
                    else
                    {
                        if (_centerPoint.Y > hit.CenterPoint.Y)
                        {
                            col.side = Sides.Up;
                        }
                        else
                        {
                            col.side = Sides.Down;
                        }
                    }
                }

                col.side = ApplyInfinit(col.side, hit.IsInfinitOnX, hit.IsInfinitOnY);

                col.collider = hit;
                return true;
            }
            else
            {
                if (this.IsCrossedVertical(hit._point, hit.p2))
                {
                    col.collider = hit;
                    if (_centerPoint.Y > hit._centerPoint.Y)
                        col.side = Sides.Up;
                    else
                        col.side = Sides.Down;
                    col.side = ApplyInfinit(col.side, hit.IsInfinitOnX, hit.IsInfinitOnY);
                    return true;
                }
            }

            return false;
        }

        private Sides _MakeCollisionWith(Rectangle hit, out bool[] bools)
        {
            hit.UpdatePos();
            bools = new bool[4] { false, false, false, false };
            bool colided = false;

            Vector2 closePoint = new Vector2(-1, -1);

            byte i = 0;
            foreach (Vector2 p in this)
            {
                if (PointInRange(p, hit._point, hit.p2, hit.IsInfinitOnX, hit.IsInfinitOnY, i < 2, i % 2 == 1, out float dx, out float dy))
                {
                    bools[i] = true;
                    colided = true;

                    if (closePoint.X < 0 || closePoint.Y < 0)
                        closePoint = new Vector2(dx, dy);
                    else
                        closePoint = new Vector2(
                            MathF.Min(closePoint.X, dx),
                            MathF.Min(closePoint.Y, dy));
                }

                i += 1;
            }
            i = 0;
            foreach (Vector2 p in hit)
            {
                if (PointInRange(p))
                {
                    bools[3 - i] = true;
                    colided = true;
                }

                i += 1;
            }

            if (colided)
            {
                byte n = _CountBools(bools);
                if (n == 2)
                {
                    return (bools[0], bools[1], bools[2], bools[3]) switch
                    {
                        (true, true, false, false) => Sides.Up,
                        (true, false, true, false) => Sides.Left,
                        (false, true, false, true) => Sides.Right,
                        _ => Sides.Down
                    };
                }
                else
                {
                    if (closePoint.X < closePoint.Y)
                    {
                        if (_centerPoint.X > hit.CenterPoint.X)
                            return Sides.Left;
                        else
                            return Sides.Right;
                    }
                    else
                    {
                        if (_centerPoint.Y > hit.CenterPoint.Y)
                            return Sides.Up;
                        else
                            return Sides.Down;
                    }
                }
            }

            return Sides.None;
        }

        public bool Check(string tag = null)
        {
            if (this.Active)
            {
                this.UpdatePos();
                foreach (Hitbox col in _hitBoxesList[_layer])
                {
                    if (col is Rectangle && col.Active && (tag is null ? true : col._tag.Equals(tag)) && col != this)
                    {
                        Rectangle hit = col as Rectangle;
                        hit.UpdatePos();

                        foreach (Vector2 p in hit)
                            if (this.PointInRange(p))
                                return true;
                        foreach (Vector2 p in this)
                            if (hit.PointInRange(p))
                                return true;
                    }
                }
            }

            return false;
        }

        public bool Check(Hitbox.Discriminent<Hitbox> discr)
        {
            if (this.Active)
            {
                this.UpdatePos();
                foreach (Hitbox col in _hitBoxesList[_layer])
                {
                    if (col is Rectangle && col.Active && discr(col) && col != this)
                    {
                        Rectangle hit = col as Rectangle;
                        hit.UpdatePos();

                        foreach (Vector2 p in hit)
                            if (this.PointInRange(p))
                                return true;
                        foreach (Vector2 p in this)
                            if (hit.PointInRange(p))
                                return true;
                    }
                }
            }

            return false;
        }

        public bool Check(byte layer, string tag = null)
        {
            if (this.Active)
            {
                this.UpdatePos();
                foreach (Hitbox col in _hitBoxesList[layer])
                {
                    if (col is Rectangle && col.Active && (tag is null ? true : col._tag.Equals(tag)) && col != this)
                    {
                        Rectangle hit = col as Rectangle;
                        hit.UpdatePos();

                        foreach (Vector2 p in hit)
                            if (this.PointInRange(p))
                                return true;
                        foreach (Vector2 p in this)
                            if (hit.PointInRange(p))
                                return true;
                    }
                }
            }

            return false;
        }

        public bool Check(byte layer, out Hitbox collider, string tag = null)
        {
            if (this.Active)
            {
                this.UpdatePos();
                foreach (Hitbox col in _hitBoxesList[layer])
                {
                    if (col is Rectangle && col.Active && (tag is null ? true : col._tag.Equals(tag)) && col != this)
                    {
                        Rectangle hit = col as Rectangle;
                        hit.UpdatePos();

                        if (InRange(hit._point, hit.p2))
                        {
                            collider = hit;
                            return true;
                        }
                    }
                }
            }

            collider = null;
            return false;
        }

        public bool Check(byte layer, out Hitbox[] colliders, string tag = null)
        {
            if (this.Active)
            {
                bool c = false;
                List<Hitbox> result = new List<Hitbox>();
                this.UpdatePos();
                foreach (Hitbox col in _hitBoxesList[layer])
                {
                    if (col is Rectangle && col.Active && (tag is null ? true : col._tag.Equals(tag)) && col != this)
                    {
                        Rectangle hit = col as Rectangle;
                        hit.UpdatePos();

                        if (InRange(hit._point, hit.p2))
                        {
                            result.Add(hit);
                            c = true;
                        }
                    }
                }

                colliders = result.ToArray();
                return c;
            }

            colliders = null;
            return false;
        }

        public bool Check(out Hitbox.Rectangle collider, Discriminent<Hitbox> discr)
        {
            if (this.Active)
            {
                this.UpdatePos();
                foreach (Hitbox col in _hitBoxesList[_layer])
                {
                    if (col is Rectangle && col.Active && discr(col) && col != this)
                    {
                        Rectangle hit = col as Rectangle;
                        hit.UpdatePos();

                        if (InRange(hit._point, hit.p2))
                        {
                            collider = hit;
                            return true;
                        }
                    }
                }
            }

            collider = null;
            return false;
        }

        public bool Check(byte layer, Discriminent<Hitbox> discr, string tag = null)
        {
            if (this.Active)
            {
                this.UpdatePos();
                foreach (Hitbox col in _hitBoxesList[layer])
                {
                    if (col is Rectangle && col.Active && discr(col) && col != this)
                    {
                        Rectangle hit = col as Rectangle;
                        hit.UpdatePos();

                        foreach (Vector2 p in hit)
                            if (this.PointInRange(p))
                                return true;
                        foreach (Vector2 p in this)
                            if (hit.PointInRange(p))
                                return true;
                    }
                }
            }

            return false;
        }

        public bool Check(byte layer, Discriminent<Hitbox> discr, out Hitbox.Rectangle colider)
        {
            if (this.Active)
            {
                this.UpdatePos();
                foreach (Hitbox col in _hitBoxesList[layer])
                {
                    if (col is Rectangle && col.Active && discr(col) && col != this)
                    {
                        Rectangle hit = col as Rectangle;
                        hit.UpdatePos();

                        if (InRange(hit._point, hit.p2))
                        {
                            colider = hit;
                            return true;
                        }
                    }
                }
            }

            colider = null;
            return false;
        }

        public bool Check(out Hitbox.Rectangle rect, string tag = null)
        {
            this.UpdatePos();
            foreach (Hitbox col in _hitBoxesList[_layer])
            {
                if (col is Rectangle && col.Active && (tag is null ? true : col._tag.Equals(tag)) && col != this)
                {
                    Rectangle hit = col as Rectangle;
                    hit.UpdatePos();

                    if (InRange(hit._point, hit.p2))
                    {
                        rect = hit;
                        return true;
                    }
                }
            }

            rect = null;
            return false;
        }

        public bool Check(out Hitbox.Rectangle rect, byte layer, string tag = null)
        {
            this.UpdatePos();
            foreach (Hitbox col in _hitBoxesList[layer])
            {
                if (col is Rectangle && col.Active && (tag is null ? true : col._tag.Equals(tag)) && col != this)
                {
                    Rectangle hit = col as Rectangle;
                    hit.UpdatePos();

                    if (InRange(hit._point, hit.p2))
                    {
                        rect = hit;
                        return true;
                    }
                }
            }
            rect = null;
            return false;
        }

        public bool CheckWith(Hitbox col)
        {
            if (col is Rectangle && col.Active && col != this)
            {
                this.UpdatePos();
                Rectangle hit = col as Rectangle;
                hit.UpdatePos();
                return InRange(hit._point, hit.p2);
            }
            return false;
        }

        public bool CheckWithVeritcal(Hitbox col)
        {
            if (col is Rectangle && col.Active && col != this)
            {
                this.UpdatePos();
                Rectangle hit = col as Rectangle;
                hit.UpdatePos();
                return InRangeVertical(hit._point, hit.p2);
            }
            return false;
        }

        /// <summary>
        /// Donne des informations supplémentaires sur les collisions.
        /// </summary>
        public Collision<Rectangle>[] AdvancedCheck(
            out Sides side,
            string tag = null)
        {
            if (tag is null)
            {
                return _advancedCheck(out side, (Hitbox h) => true);
            }
            else
            {
                return _advancedCheck(out side, (Hitbox h) => h.Tag.Equals(tag));
            }
        }

        public Collision<Rectangle>[] AdvancedCheck(
            out Sides side, Discriminent<Hitbox> discr)
        {
            return _advancedCheck(out side, discr);
        }

        private Collision<Rectangle>[] _advancedCheck(
            out Sides side, Discriminent<Hitbox> discr)
        {
            this.UpdatePos();
            List<Collision<Rectangle>> cols = new List<Collision<Rectangle>>();
            side = Sides.None;

            bool[] global = new bool[4] { false, false, false, false };

            foreach (Hitbox col in _hitBoxesList[_layer])
            {
                if (col is Rectangle && col.Active && discr(col) && col != this)
                {
                    Rectangle hit = col as Rectangle;
                    if (_MakeCollisionWith(hit, out Collision<Rectangle> c, ref global))
                    {
                        cols.Add(c);
                    }
                }
            }

            byte n = _CountBools(global);
            if (n == 2 && cols.Count > 1)
            {
                switch (global[0], global[1], global[2], global[3])
                {
                    case (true, true, false, false):
                        if (float.Abs(cols[0].collider.p2.Y - cols[1].collider.p2.Y) < 2)
                            side = Sides.Up;
                        break;

                    case (false, false, true, true):
                        if (float.Abs(cols[0].collider._point.Y - cols[1].collider._point.Y) < 2)
                            side = Sides.Down;
                        break;

                    case (true, false, true, false):
                        if (float.Abs(cols[0].collider.p2.X - cols[1].collider.p2.X) < 2)
                            side = Sides.Left;
                        break;

                    case (false, true, false, true):
                        if (float.Abs(cols[0].collider._point.X - cols[1].collider._point.X) < 2)
                            side = Sides.Right;
                        break;
                }
            }
            else if (n == 3)
            {
                switch (global[0], global[1], global[2], global[3])
                {
                    case (true, true, true, false):
                        foreach (Collision<Rectangle> col in cols)
                        {
                            if (col.side == Sides.Down)
                            {
                                col.SetSide(Sides.Left);
                            }
                            if (col.side == Sides.Right)
                            {
                                col.SetSide(Sides.Up);
                            }
                        }
                        break;

                    case (true, true, false, true):
                        foreach (Collision<Rectangle> col in cols)
                        {
                            if (col.side == Sides.Down)
                            {
                                col.SetSide(Sides.Right);
                            }
                            if (col.side == Sides.Left)
                            {
                                col.SetSide(Sides.Up);
                            }
                        }
                        break;

                    case (false, true, true, true):
                        foreach (Collision<Rectangle> col in cols)
                        {
                            if (col.side == Sides.Left)
                            {
                                col.SetSide(Sides.Down);
                            }
                            if (col.side == Sides.Up)
                            {
                                col.SetSide(Sides.Right);
                            }
                        }
                        break;

                    case (true, false, true, true):
                        foreach (Collision<Rectangle> col in cols)
                        {
                            if (col.side == Sides.Right)
                            {
                                col.SetSide(Sides.Down);
                            }
                            if (col.side == Sides.Up)
                            {
                                col.SetSide(Sides.Left);
                            }
                        }
                        break;
                }
            }

            return cols.ToArray();
        }

        /// <summary>
        /// Donne des informations supplémentaires sur la collision.
        /// </summary>
        public Sides AdvancedCheckWith(Hitbox col)
        {
            this.UpdatePos();
            Vector2 centerPoint = this.CenterPoint;
            if (col is Rectangle && col.Active && col != this)
            {
                Rectangle ccc = col as Rectangle;
                return ApplyInfinit(_MakeCollisionWith(ccc, out _), ccc.IsInfinitOnX, ccc.IsInfinitOnY);
            }

            return Sides.None;
        }

        public Sides AdvancedCheckWith(Hitbox col, out bool[] cornerInfo)
        {
            this.UpdatePos();
            Vector2 centerPoint = this.CenterPoint;
            if (col is Rectangle && col.Active && col != this)
            {
                Rectangle ccc = col as Rectangle;
                return ApplyInfinit(_MakeCollisionWith(ccc, out cornerInfo), ccc.IsInfinitOnX, ccc.IsInfinitOnY);
            }

            cornerInfo = Array.Empty<bool>();
            return Sides.None;
        }

        public void Draw(in SpriteBatch batch)
        {
            if (Active)
            {
                UpdatePos();
                batch.DrawRectangle
                (
                    new Microsoft.Xna.Framework.Rectangle
                    (
                        (int)(float.Round(_point.X) - Space.Camera.X),
                        (int)(float.Round(_point.Y) - Space.Camera.Y),
                        (int)float.Round(p2.X - _point.X),
                        (int)float.Round(p2.Y - _point.Y)
                    ),
                    _color[_layer],
                    1,
                    layerDepth: 0
                );

                if (IsInfinitOnX == Align.Left)
                {
                    batch.DrawLine(
                        (int)(float.Round(p2.X - 2) - Space.Camera.X),
                        (int)(float.Round(_point.Y + 2) - Space.Camera.Y),
                        (int)(float.Round(p2.X - 2) - Space.Camera.X),
                        (int)(float.Round(p2.Y - 2) - Space.Camera.Y),
                    _color[_layer],
                        layerDepth: 0);
                }
                else if (IsInfinitOnX == Align.Right)
                {
                    batch.DrawLine(
                        (int)(float.Round(_point.X + 3) - Space.Camera.X),
                        (int)(float.Round(_point.Y + 2) - Space.Camera.Y),
                        (int)(float.Round(_point.X + 3) - Space.Camera.X),
                        (int)(float.Round(p2.Y - 2) - Space.Camera.Y),
                    _color[_layer],
                        layerDepth: 0);
                }

                if (IsInfinitOnY == Align.Left)
                {
                    batch.DrawLine(
                        (int)(float.Round(_point.X + 2) - Space.Camera.X),
                        (int)(float.Round(p2.Y - 2) - Space.Camera.Y),
                        (int)(float.Round(p2.X - 2) - Space.Camera.X),
                        (int)(float.Round(p2.Y - 2) - Space.Camera.Y),
                    _color[_layer],
                        layerDepth: 0);
                }
                else if (IsInfinitOnY == Align.Right)
                {
                    batch.DrawLine(
                        (int)(float.Round(_point.X + 2) - Space.Camera.X),
                        (int)(float.Round(_point.Y + 3) - Space.Camera.Y),
                        (int)(float.Round(p2.X - 2) - Space.Camera.X),
                        (int)(float.Round(_point.Y + 3) - Space.Camera.Y),
                    _color[_layer],
                        layerDepth: 0);
                }
            }
        }
    }
}
