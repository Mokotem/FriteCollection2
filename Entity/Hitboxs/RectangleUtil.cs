

using Microsoft.Xna.Framework;

namespace FriteCollection2.Entity.Hitboxs;


public abstract partial class Hitbox
{
	public partial class Rectangle : Hitbox
	{
		public static bool Collinear(Sides s1, Sides s2)
		{
			bool up1 = s1 == Sides.Up || s1 == Sides.Down;
			bool up2 = s2 == Sides.Up || s2 == Sides.Down;
			return up1 && up2;
		}

        public static int GetMost(CollisionData<Rectangle>[] cols, Sides side, Critere crit)
        {
			bool n = true;
			int max = 0;

            for (int i = 1; i < cols.Length; i++)
            {
                if (cols[i].side == side && (n || crit(cols[i].collider) > crit(cols[max].collider)))
                {
                    max = i;
					n = false;
                }
            }

            return max;
        }

        public static int ChoseTheBestFor(Hitbox.CollisionData<Rectangle>[] rects, Sides side)
        {
            return (side) switch
            {
                Sides.Up => GetMost(rects, Sides.Up, (Hitbox.Rectangle r) => r._down),
                Sides.Left => GetMost(rects, Sides.Left, (Hitbox.Rectangle r) => r._right),
                Sides.Right => GetMost(rects, Sides.Right, (Hitbox.Rectangle r) => -r._left),
                Sides.Down => GetMost(rects, Sides.Down, (Hitbox.Rectangle r) => -r._up),
            };
        }

        public static void ApplyCollision(Space mec, Rectangle colider, Sides side)
		{
			switch (side)
			{
				case Sides.Right:
					mec.X = colider._left - mec.W;
					return;
				case Sides.Left:
					mec.X = colider._right;
					return;
				case Sides.Up:
					mec.Y = colider._down;
					return;
				case Sides.Down:
					mec.Y = colider._up - mec.H;
					return;
			}
		}

		public bool ApplyCollision(Rectangle colider, Sides side, Vector2 vitesse, Vector2 input, out Vector2 pos)
		{
			pos = input;
			switch (side)
			{
				case Sides.Right:
				if (vitesse.X < 0)
					return false;

				pos.X = colider._left - _width - offset.X;

                return true;
				case Sides.Left:
				if (vitesse.X > 0)
					return false;

                pos.X = colider._right - offset.X;

                return true;
				case Sides.Up:
				if (vitesse.Y > 0)
					return false;

                pos.Y = colider._down - offset.Y;
                return true;

				default:
				if (vitesse.Y < 0)
					return false;

                pos.Y = colider._up - _height - offset.Y;
                return true;
			}
		}

		public bool ApplyCollision(CollisionData<Rectangle> col, Vector2 vitesse, Vector2 input, out Vector2 colPos)
        {
			return ApplyCollision(col.collider, col.side, vitesse, input, out colPos);
		}


        public bool ApplyCollision(CollisionData<Rectangle> col, Vector2 vitesse)
        {
            return ApplyCollision(col.collider, col.side, vitesse, _parent.Position, out _parent.Position);
        }

        public bool ApplyCollision(Rectangle collider, Sides side, Vector2 vitesse)
        {
            return ApplyCollision(collider, side, vitesse, _parent.Position, out _parent.Position);
        }
    }
}