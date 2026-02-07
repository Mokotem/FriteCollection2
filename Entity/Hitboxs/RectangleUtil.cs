

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

        public static void ApplyCollition(in Space mec, Rectangle colider, Sides side, Vector2 vitesse)
        {
            switch (side)
            {
                case Sides.Right:
					if (vitesse.X < 0)
						return;

                    mec.X = colider._left - mec.W;
                    return;
                case Sides.Left:
					if (vitesse.X > 0)
						return;

                    mec.X = colider._right;
                    return;
                case Sides.Up:
					if (vitesse.Y > 0)
						return;

                    mec.Y = colider._down;
                    return;
                case Sides.Down:
					if (vitesse.Y < 0)
						return;

                    mec.Y = colider._up - mec.H;
                    return;
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