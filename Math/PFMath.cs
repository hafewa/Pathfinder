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
	}
	public struct PFRect
	{
		public int x;
		public int y;
		public int z;
		public int w;

		public PFRect(int x, int y, int z, int w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
	}
}
