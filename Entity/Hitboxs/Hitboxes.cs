using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using static FriteCollection2.Tools.TileMap.MetroidMaker;

namespace FriteCollection2.Entity.Hitboxs;

public abstract class Hitbox
{
    private const int _numberOfLayers = 3;
    private const float Pis2 = float.Pi / 2f;

    public delegate bool MessageBox(short message);

    public MessageBox SendMessage { get; init; }

    private readonly static List<Hitbox>[] _hitBoxesList = new List<Hitbox>[_numberOfLayers]
    {
        new(),
        new(),
        new()
    };

    public virtual Point Size => Point.Zero;

    /// <summary>
    /// Supprime TOUTES les hitboxs.
    /// </summary>
    public static void ClearAllLayers()
    {
        _hitBoxesList[0] = new();
        _hitBoxesList[1] = new();
        _hitBoxesList[2] = new();
    }

    /// <summary>
    /// Supprime TOUTES les hitboxs d'une couche.
    /// </summary>
    /// <param name="id">numéro de la couche</param>
    public static void ClearLayer(byte id)
    {
        _hitBoxesList[id] = new();
    }

    /// <summary>
    /// Supprime TOUTES les hitboxs avec le tag 'tag' d'une couche.
    /// </summary>
    /// <param name="id">numéro de la couche</param>
    /// <param name="tag">étiquette à supprimer</param>
    public static void ClearLayer(byte id, string tag)
    {
        ushort i = 0;
        while (i < _hitBoxesList[id].Count)
        {
            if (_hitBoxesList[id][i]._tag.Equals(tag))
            {
                _hitBoxesList[id].RemoveAt(i);
            }
            else
                ++i;
        }
    }

    /// <summary>
    /// Supprime TOUTES les hitboxs avec un truc spésifique d'une couche.
    /// </summary>
    /// <param name="id">numéro de la couche</param>
    /// <param name="discr">fonction</param>
    /// <example>ClearLayer(0, (Hitbox hit) => hit is Hitbox.Rectangle && hit.PositionOffset.X > 30)</example>
    public static void ClearLayer(byte id, GameManager.Discriminent<Hitbox> discr)
    {
        ushort i = 0;
        while (i < _hitBoxesList[id].Count)
        {
            if (discr(_hitBoxesList[id][i]))
            {
                _hitBoxesList[id].RemoveAt(i);
            }
            else
                ++i;
        }
    }

    private static Color[] _color = new Color[_numberOfLayers]
    {
        new Color(255, 0, 0),
        new Color(0, 255, 0),
        new Color(0, 0, 255)
    };

    internal static Color DebugColor => _color[0];

    /// <summary>
    /// Modifie la couleur d'une couche.
    /// </summary>
    /// <example>SetLayerColor(1, Color.White)</example>
    public static void SetLayerColor(int layer, Color color)
    {
        _color[layer] = color;
    }

    protected bool isStatic = false;
    /// <summary>
    /// Ne bouge plus même si son 'Space' référant le fait.
    /// Utile pour les platformes solides par exemple. Grain de performance.
    /// </summary>
    public bool IsStatic
    {
        get => isStatic;
        set
        {
            UpdatePos();
            isStatic = value;
        }
    }

    private readonly byte _layer = 0, _secondlayer;
    internal string _tag = "";

    public string Tag => _tag;

    public bool Active = true;

    public readonly Space SpaceRef;

    public Point PositionOffset;

    private Hitbox(in Space _space, string tag = "", byte primaryLayer = 0, byte secondaryLayer = 255)
    {
        SpaceRef = _space;
        this._tag = tag;
        _hitBoxesList[primaryLayer].Add(this);
        if (secondaryLayer < 200)
            _hitBoxesList[secondaryLayer].Add(this);
        _layer = primaryLayer;
        _secondlayer = secondaryLayer;
    }

    /// <summary>
    /// Seulement les hitboxs de la meme couche peuvent se toucher.
    /// </summary>
    public byte Layer => _layer;

    public enum Sides
    {
        Up, Down, Left, Right, None
    }

    private protected Vector2 _point;
    private bool positionLocked = false;

    /// <summary>
    /// Epingle la hitbox sur une position.
    /// </summary>
    public virtual void LockPosition(Vector2 value)
    {
        positionLocked = true;
        _point = value;
    }

    public void UnlockPosition()
    {
        positionLocked = false;
    }

    private protected virtual void UpdatePos() { }

    /// <summary>
    /// Informations sur une collision détectée.
    /// </summary>
    public class Collision<H> where H: Hitbox
    {
        public H collider;
        public Sides side;

        public void SetSide(Sides value)
        {
            side = value;
        }
    }

    private interface ICollider
    {
        /// <summary>
        /// Vérifie si il y a une collision
        /// </summary>
        /// <param name="tag">uniquement avec les hitboxs de tag</param>
        public bool Check(string tag = null);

        /// <summary>
        /// Vérifie la collision avec une hitbox spécifique.
        /// </summary>
        public bool CheckWith(Hitbox collider);
    }

    private bool destroyed = false;
    /// <summary>
    /// Retire la hitbox de la liste.
    /// </summary>
    public void Destroy()
    {
        if (!destroyed)
        {
            _hitBoxesList[_layer].Remove(this);
            if (_secondlayer < 250)
                _hitBoxesList[_secondlayer].Remove(this);
            destroyed = true;
        }
    }

    /// <summary>
    /// Remet la hitbox de la liste.
    /// </summary>
    public void Reactivate()
    {
        _hitBoxesList[_layer].Add(this);
        destroyed = false;
    }

    /// <summary>
    /// déssine toutes les hitboxs.
    /// </summary>
    public static void Debug()
    {
        foreach (List<Hitbox> list in _hitBoxesList)
            foreach (IDraw hit in list)
                hit.Draw();
    }

    public static bool Check(Vector2 pos, byte _layer, string tag = null)
    {
        foreach (Hitbox col in _hitBoxesList[_layer])
        {
            if (tag is null ? true : tag.Equals(col._tag))
            {
                if (col is Rectangle)
                {
                    Rectangle hit = col as Rectangle;
                    hit.UpdatePos();
                    if (hit.PointInRange(pos))
                        return true;
                }
                else if (col is Circle)
                {
                    Circle hit = col as Circle;
                    hit.UpdatePos();
                    if (Vector2.Distance(pos, hit._point) < hit.Radius)
                        return true;
                }
            }
        }

        return false;
    }

    public static bool Check(Vector2 pos, byte _layer, out Hitbox colider, string tag = null)
    {
        foreach (Hitbox col in _hitBoxesList[_layer])
        {
            if (tag is null ? true : tag.Equals(col._tag))
            {
                if (col is Rectangle)
                {
                    Rectangle hit = col as Rectangle;
                    hit.UpdatePos();
                    if (hit.PointInRange(pos))
                    {
                        colider = hit;
                        return true;
                    }
                }
                else if (col is Circle)
                {
                    Circle hit = col as Circle;
                    hit.UpdatePos();
                    if (Vector2.Distance(pos, hit._point) < hit.Radius)
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

    /// <summary>
    /// Ligne infini.
    /// </summary>
    public class Line : Hitbox, ICollider, IDraw
    {
        float _dir;
        private Vector2 norme;
        private float thickness;

        public Line(SpaceDirection _space, float thickness = 0, string tag = "", byte layer = 0, byte slayer = 255)
            : base(_space, tag, layer, slayer)
        {
            this.thickness = thickness;
            this.UpdatePos();
            norme = new Vector2(float.Cos(_space.direction), float.Sin(_space.direction));
        }

        private bool directionLocked;
        public void LockDirection(float value)
        {
            directionLocked = true;
            norme = new Vector2(float.Cos(value), float.Sin(value));
        }

        /// <summary>
        /// Norme de la droite. (pas vecteur directeur)
        /// </summary>
        public Vector2 Norme => norme;

        /// <summary>
        /// épaisseur.
        /// </summary>
        public float Thickness
        {
            get => thickness;
            set => thickness = value;
        }

        private protected override void UpdatePos()
        {
            if (!positionLocked)
            {
                _point = SpaceRef.Position;
                _point.X += PositionOffset.X;
                _point.Y += PositionOffset.Y;
            }

            if (!directionLocked)
            {
                _dir = (SpaceRef as SpaceDirection).direction;
                norme = new Vector2(float.Cos(_dir),
                    float.Sin(_dir));
            }
        }

        public bool Check(string tag = null)
        {
            this.UpdatePos();
            foreach (Hitbox col in _hitBoxesList[_layer])
            {
                if (col is Circle && (tag is null ? true : tag.Equals(col._tag)))
                {
                    Circle hit = col as Circle;
                    hit.UpdatePos();

                    Vector2 v = new Vector2(hit._point.X - _point.X, hit._point.Y - _point.Y);
                    if (float.Abs(Vector2.Dot(norme, v)) < hit.Radius + (thickness / 2f))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckWith(Hitbox col)
        {
            this.UpdatePos();
            if (col is Circle)
            {
                Circle hit = col as Circle;
                hit.UpdatePos();

                Vector2 v = new Vector2(hit._point.X - _point.X, hit._point.Y - _point.Y);
                return float.Abs(Vector2.Dot(norme, v)) < hit.Radius;
            }

            return false;
        }

        private float f(float x) => (float.Tan(_dir + Pis2) * (x - _point.X)) + _point.Y;
        private float g(float y) => (float.Tan(-_dir ) * (y - _point.Y)) + _point.X;

        public void Draw()
        {
            if (this.Active)
            {
                this.UpdatePos();
                if (float.Abs(float.Sin(_dir)) < 0.001f)
                {
                    GameManager.Draw.Batch.DrawLine
                    (
                        _point.X, 0,
                        _point.X, Screen.height,
                        _color[_layer] * (thickness == 0 ? 1 : 0.2f),
                        thickness: thickness + 1
                    );
                }
                else
                {
                    Vector2 p1;
                    Vector2 p2;
                    if (float.Abs(norme.Y) > float.Abs(norme.X))
                    {
                        p1 = new Vector2(0, f(0));
                        p2 = new Vector2(Screen.widht, f(Screen.widht));
                    }
                    else
                    {
                        p1 = new Vector2(g(0), 0);
                        p2 = new Vector2(g(Screen.height), Screen.height);
                    }

                    GameManager.Draw.Batch.DrawLine
                    (
                        p1.X, p1.Y,
                        p2.X, p2.Y,
                        _color[_layer] * (thickness == 0 ? 1 : 0.2f),
                        thickness: thickness + 1
                    );
                }
            }
        }
    }

    public class Circle : Hitbox, ICollider, IDraw, ICopy<Circle>
    {
        private float _radius;

        /// <summary>
        /// rayon
        /// </summary>
        public float Radius => _radius;

        public Circle(in Space _space, string tag = "", byte layer = 0, byte slayer = 255) : base(_space, tag, layer, slayer)
        {
            this.UpdatePos();
            radiusLocked = false;
            _radius = _space.Scale.X / 2f;
        }

        public Circle Copy()
        {
            return new Circle(SpaceRef, this._tag, this._layer)
            {
                PositionOffset = this.PositionOffset,
                _point = this._point,
                _radius = this._radius,
                positionLocked = this.positionLocked,
                radiusLocked = this.radiusLocked
            };
        }

        private protected override void UpdatePos()
        {
            if (!positionLocked)
            {
                _point = SpaceRef.Position;
                _point.X += PositionOffset.X;
                _point.Y += PositionOffset.Y;
            }
        }

        public bool Check(string tag = null)
        {
            this.UpdatePos();
            foreach (Hitbox col in _hitBoxesList[_layer])
            {
                if ((col is Circle || col is Line) && col.Active && (tag is null ? true : col._tag == tag) && col != this)
                {
                    if (col is Circle)
                    {
                        Circle hit = col as Circle;
                        hit.UpdatePos();

                        if (Vector2.Distance(_point, hit._point) < _radius + hit._radius)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        Line hit = col as Line;
                        hit.UpdatePos();

                        Vector2 v = new Vector2(_point.X - hit._point.X, _point.Y - hit._point.Y);
                        if (float.Abs(Vector2.Dot(hit.Norme, v)) < Radius + (hit.Thickness / 2f))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Donne des informations supplémentaires sur les collisions.
        /// </summary>
        public Collision<Circle>[] AdvancedCheck(
                string tag = null)
        {
            List<Collision<Circle>> result = new List<Collision<Circle>>();
            this.UpdatePos();
            foreach (Hitbox col in _hitBoxesList[_layer])
            {
                if (col is Circle && col.Active && (tag is null ? true : col._tag == tag) && col != this)
                {
                    Circle hit = col as Circle;
                    hit.UpdatePos();

                    if (Vector2.Distance(_point, hit._point) < _radius + hit._radius)
                    {
                        Collision<Circle> c = new Collision<Circle>();
                        c.collider = hit;
                        c.side = Sides.None;
                        result.Add(c);
                    }
                }
            }

            return result.ToArray();
        }

        public bool CheckWith(Hitbox col)
        {
            this.UpdatePos();
            if (col is Circle)
            {
                Circle hit = col as Circle;
                hit.UpdatePos();

                if (Vector2.Distance(_point, hit._point) < _radius + hit._radius)
                {
                    return true;
                }
            }
            else
            {
                Line hit = col as Line;
                hit.UpdatePos();

                Vector2 v = new Vector2(_point.X - hit._point.X, _point.Y - hit._point.Y);
                if (float.Abs(Vector2.Dot(hit.Norme, v)) < Radius + (hit.Thickness / 2f))
                {
                    return true;
                }
            }

            return false;
        }

        private bool radiusLocked;
        public void LockRadius(float value)
        {
            radiusLocked = true;
            _radius = value;
        }

        public void Draw()
        {
            if (this.Active)
            {
                this.UpdatePos();
                GameManager.Draw.Batch.DrawCircle
                (
                    new CircleF(_point, _radius),
                    (int)(float.Sqrt(_radius + 10) * 2),
                    Hitbox._color[this._layer]
                );
            }
        }
    }

    public class Rectangle : Hitbox, IEnumerable, ICollider, IDraw, ICopy<Rectangle>
    {
        public bool ApplyColision(Collision<Rectangle> col, Vector2 vitesse, Action OnUp, Action OnDown, Action OnLeft, Action OnRight)
        {
            if (col.collider.positionLocked)
            {
                switch (col.side)
                {
                    case Sides.Down:
                        if (vitesse.Y >= 0)
                        {
                            SpaceRef.Y = col.collider._point.Y + col.collider.PositionOffset.Y - SpaceRef.Scale.Y
                                + (SpaceRef.Scale.Y - Size.Y - PositionOffset.Y);
                            OnDown();
                            return true;
                        }
                        else return false;
                    case Sides.Up:
                        if (vitesse.Y <= 0)
                        {
                            SpaceRef.Y = col.collider._point.Y + col.collider.PositionOffset.Y + col.collider.Size.Y
                                 - (SpaceRef.Scale.Y - Size.Y - PositionOffset.Y);
                            OnUp();
                            return true;
                        }
                        else return false;
                    case Sides.Left:
                        if (vitesse.X <= 0)
                        {
                            SpaceRef.X = col.collider._point.X + col.collider.PositionOffset.X + col.collider.Size.X
                                 - (SpaceRef.Scale.X - Size.X - PositionOffset.X);
                            OnLeft();
                            return true;
                        }
                        else return false;
                    case Sides.Right:
                        if (vitesse.X >= 0)
                        {
                            SpaceRef.X = col.collider._point.X + col.collider.PositionOffset.X - SpaceRef.Scale.X
                                + (SpaceRef.Scale.X - Size.X - PositionOffset.X);
                            OnRight();
                            return true;
                        }
                        else return false;
                    default:
                        return false;
                }
            }
            else
            {
                switch (col.side)
                {
                    case Sides.Down:
                        if (vitesse.Y >= 0)
                        {
                            SpaceRef.Y = col.collider.SpaceRef.Y + col.collider.PositionOffset.Y - SpaceRef.Scale.Y
                                + (SpaceRef.Scale.Y - Size.Y - PositionOffset.Y);
                            OnDown();
                            return true;
                        }
                        else return false;
                    case Sides.Up:
                        if (vitesse.Y <= 0)
                        {
                            SpaceRef.Y = col.collider.SpaceRef.Y + col.collider.PositionOffset.Y + col.collider.Size.Y
                                 - (SpaceRef.Scale.Y - Size.Y - PositionOffset.Y);
                            OnUp();
                            return true;
                        }
                        else return false;
                    case Sides.Left:
                        if (vitesse.X <= 0)
                        {
                            SpaceRef.X = col.collider.SpaceRef.X + col.collider.PositionOffset.X + col.collider.Size.X
                                 - (SpaceRef.Scale.X - Size.X - PositionOffset.X);
                            OnLeft();
                            return true;
                        }
                        else return false;
                    case Sides.Right:
                        if (vitesse.X >= 0)
                        {
                            SpaceRef.X = col.collider.SpaceRef.X + col.collider.PositionOffset.X - SpaceRef.Scale.X
                                + (SpaceRef.Scale.X - Size.X - PositionOffset.X);
                            OnRight();
                            return true;
                        }
                        else return false;
                    default:
                        return false;
                }
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

        public Bounds IsInfinitOn = Bounds.Center;

        private Vector2 p2;

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
                {
                    p2 = new Vector2(_point.X + lockSize.X, _point.Y + lockSize.Y);
                }
                else
                {
                    p2 = new Vector2(_point.X + SpaceRef.Scale.X, _point.Y + SpaceRef.Scale.Y);
                }

                _point.X += PositionOffset.X;
                _point.Y += PositionOffset.Y;
                p2.X += PositionOffset.X;
                p2.Y += PositionOffset.Y;
                _centerPoint = new Vector2((_point.X + p2.X) / 2f, (_point.Y + p2.Y) / 2f);
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

        private static bool PointInRange(Vector2 p, Vector2 _1, Vector2 _2, bool up, bool right, out float distX, out float distY)
        {
            if (p.X >= _1.X && p.X <= _2.X && p.Y >= _1.Y && p.Y <= _2.Y)
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

        public bool PointInRange(Vector2 p)
        {
            return p.X >= _point.X && p.X <= p2.X && p.Y >= _point.Y && p.Y <= p2.Y;
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

        private static Sides ApplyInfinit(Sides side, Bounds infinit)
        {
            switch (side)
            {
                case Sides.Up:
                    if ((byte)infinit > 5)
                        return Sides.Down;
                    return Sides.Up;

                case Sides.Down:
                    if ((byte)infinit < 3)
                        return Sides.Up;
                    return Sides.Down;

                case Sides.Left:
                    if ((byte)infinit % 3 == 2)
                        return Sides.Right;
                    return Sides.Left;

                case Sides.Right:
                    if ((byte)infinit % 3 == 0)
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
                if (PointInRange(p, hit._point, hit.p2, i < 2, i % 2 == 1, out float dx, out float dy))
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

                col.side = ApplyInfinit(col.side, hit.IsInfinitOn);

                col.collider = hit;
                return true;
            }

            col.side = ApplyInfinit(col.side, hit.IsInfinitOn);

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
                if (PointInRange(p, hit._point, hit.p2, i < 2, i % 2 == 1, out float dx, out float dy))
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

            return false;
        }

        public bool Check(GameManager.Discriminent<Hitbox> discr)
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

            return false;
        }

        public bool Check(byte layer, string tag = null)
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

            return false;
        }

        public bool Check(byte layer, out Hitbox collider, string tag = null)
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

            collider = null;
            return false;
        }

        public bool Check(byte layer, out Hitbox[] colliders, string tag = null)
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

        public bool Check(out Hitbox.Rectangle collider, GameManager.Discriminent<Hitbox> discr)
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

            collider = null;
            return false;
        }

        public bool Check(byte layer, GameManager.Discriminent<Hitbox> discr, string tag = null)
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

            return false;
        }

        public bool Check(byte layer, GameManager.Discriminent<Hitbox> discr, out Hitbox.Rectangle colider)
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
            out Sides side, GameManager.Discriminent<Hitbox> discr)
        {
            return _advancedCheck(out side, discr);
        }

        private Collision<Rectangle>[] _advancedCheck(
            out Sides side, GameManager.Discriminent<Hitbox> discr)
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
            if (n == 2)
            {
                switch (global[0], global[1], global[2], global[3])
                {
                    case (true, true, false, false):
                        side = Sides.Up;
                        break;

                    case (false, false, true, true):
                        side = Sides.Down;
                        break;

                    case (true, false, true, false):
                        side = Sides.Left;
                        break;

                    case (false, true, false, true):
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
                return ApplyInfinit(_MakeCollisionWith(col as Rectangle, out _), (col as Rectangle).IsInfinitOn);
            }

            return Sides.None;
        }

        public Sides AdvancedCheckWith(Hitbox col, out bool[] cornerInfo)
        {
            this.UpdatePos();
            Vector2 centerPoint = this.CenterPoint;
            if (col is Rectangle && col.Active && col != this)
            {
                return ApplyInfinit(_MakeCollisionWith(col as Rectangle, out cornerInfo), (col as Rectangle).IsInfinitOn);
            }

            cornerInfo = Array.Empty<bool>();
            return Sides.None;
        }

        public void Draw()
        {
            if (Active)
            {
                UpdatePos();
                GameManager.Draw.Batch.DrawRectangle
                (
                    new Microsoft.Xna.Framework.Rectangle
                    (
                        (int)(float.Round(_point.X) - Camera.Position.X),
                        (int)(float.Round(_point.Y) - Camera.Position.Y),
                        (int)float.Round(p2.X - _point.X),
                        (int)float.Round(p2.Y - _point.Y)
                    ),
                    _color[_layer],
                    1,
                    layerDepth: 0
                );
            }
        }
    }
}