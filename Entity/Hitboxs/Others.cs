using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FriteCollection2.Entity.Hitboxs;

public abstract partial class Hitbox
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
    public virtual bool IsStatic
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
}