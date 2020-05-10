using System;


namespace Pathfinder
{
	public struct PFPoint
	{
		public int x;
		public int y;

		public PFPoint(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static PFPoint operator +(PFPoint a, PFPoint b)
		{
			return new PFPoint(a.x + b.x, a.y + b.y);
		}

		public static PFPoint operator -(PFPoint a, PFPoint b)
		{
			return new PFPoint(a.x - b.x, a.y - b.y);
		}

		public static PFPoint operator *(PFPoint a, PFPoint b)
		{
			return new PFPoint(a.x * b.x, a.y * b.y);
		}

		public static PFPoint operator /(PFPoint a, PFPoint b)
		{
			return new PFPoint(a.x / b.x, a.y / b.y);
		}
	}


	public struct PFRect
	{
		public int x;
		public int y;
		public int x1;
		public int y1;

		public PFRect(int x, int y, int z, int w)
		{
			this.x = x;
			this.y = y;
			this.x1 = z;
			this.y1 = w;
		}

		public static PFRect operator +(PFRect a, PFRect b)
		{
			return new PFRect(a.x + b.x, a.y + b.y, a.x1 + b.x1, a.y1 + b.y1);
		}

		public static PFRect operator -(PFRect a, PFRect b)
		{
			return new PFRect(a.x - b.x, a.y - b.y, a.x1 - b.x1, a.y1 - b.y1);
		}

		public static PFRect operator *(PFRect a, PFRect b)
		{
			return new PFRect(a.x * b.x, a.y * b.y, a.x1 * b.x1, a.y1 * b.y1);
		}

		public static PFRect operator /(PFRect a, PFRect b)
		{
			return new PFRect(a.x / b.x, a.y / b.y, a.x1 / b.x1, a.y1 / b.y1);
		}
	}
}
