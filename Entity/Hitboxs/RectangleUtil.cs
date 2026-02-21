

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

        public static int GetMost(in CollisionData<Rectangle>[] rects, Critere crit)
        {
            int max = 0;
            float maxv = crit(rects[0].collider);

            for (int i = 1; i < rects.Length; i++)
            {
                if (crit(rects[i].collider) > maxv)
                {
                    maxv = crit(rects[i].collider);
                    max = i;
                }
            }

            return max;
        }

        public static int GetMost(in Hitbox.Rectangle[] rects, Critere crit)
		{
			int max = 0;
			float maxv = crit(rects[0]);

			for (int i = 1; i < rects.Length; i++)
			{
				if (crit(rects[i]) > maxv)
				{
					maxv = crit(rects[i]);
					max = i;
				}
			}

			return max;
		}

        public static int ChoseTheBestFor(in Hitbox.Rectangle[] rects, Sides side)
        {
			return (side) switch
			{
				Sides.Up => GetMost(in rects, (Hitbox.Rectangle r) => r._down),
                Sides.Left => GetMost(in rects, (Hitbox.Rectangle r) => r._right),
                Sides.Right => GetMost(in rects, (Hitbox.Rectangle r) => -r._left),
                _ => GetMost(in rects, (Hitbox.Rectangle r) => -r._up),
            };
        }

        public static int ChoseTheBestFor(in Hitbox.CollisionData<Rectangle>[] rects, Sides side)
        {
            return (side) switch
            {
                Sides.Up => GetMost(in rects, (Hitbox.Rectangle r) => r._down),
                Sides.Left => GetMost(in rects, (Hitbox.Rectangle r) => r._right),
                Sides.Right => GetMost(in rects, (Hitbox.Rectangle r) => -r._left),
                _ => GetMost(in rects, (Hitbox.Rectangle r) => -r._up),
            };
        }

        public static void ApplyCollition(in Space mec, Rectangle colider, Sides side)
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

		public static Vector2 ApplyCollition(in Space mec, Rectangle colider, Sides side, Vector2 vitesse)
		{
			Vector2 result = mec.Position;
			switch (side)
			{
				case Sides.Right:
					if (vitesse.X < 0)
						return mec.Position;

					result.X = colider._left - mec.W;
					return result;
				case Sides.Left:
					if (vitesse.X > 0)
						return mec.Position;

					result.X = colider._right;
					return result;
				case Sides.Up:
					if (vitesse.Y > 0)
						return mec.Position;

					result.Y = colider._down;
					return result;
				default:
					if (vitesse.Y < 0)
						return mec.Position;

					result.Y = colider._up - mec.H;
					return result;
			}
		}

        public static void ApplyCollition(in Space mec, CollisionData<Rectangle> collision)
		{
			ApplyCollition(in mec, collision.collider, collision.side);
		}

        public static void ApplyCollition(in Space mec, CollisionData<Rectangle> collision, Vector2 vitesse)
        {
            ApplyCollition(in mec, collision.collider, collision.side, vitesse);
        }
    }
}