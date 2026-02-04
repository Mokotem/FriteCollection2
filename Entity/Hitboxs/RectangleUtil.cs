
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
					mec.X = colider.left - mec.W;
					return;
				case Sides.Left:
					mec.X = colider.right;
					return;
				case Sides.Up:
					mec.Y = colider.down;
					return;
				case Sides.Down:
					mec.Y = colider.up - mec.H;
					return;
			}
		}

		public static void ApplyCollition(in Space mec, CollisionData<Rectangle> collision)
		{
			ApplyCollition(in mec, collision.colider, collision.side);
		}
	}
}