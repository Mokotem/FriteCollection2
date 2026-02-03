
namespace FriteCollection2.Entity.Hitboxs;


public abstract partial class Hitbox
{
	public partial class Rectangle : Hitbox
	{
		public void ApplyCollition(Sides globalSide, CollisionData<Rectangle>[] collisions, int closestColId)
		{
			ApplyCollition(in parent, collisions[closestColId].colider, globalSide);
			for (byte i = 0; i < collisions.Length; i++)
			{
				if (i != closestColId && Intersect(collisions[i].colider))
				{
					// suite
				}
			}
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