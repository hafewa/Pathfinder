using System;
using UnityEngine;

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

		public static bool operator ==(PFPoint a, PFPoint b)
		{
			if(a.x == b.x && a.y == b.y)
			{
				return true;
			}
			return false;
		}

		public static bool operator !=(PFPoint a, PFPoint b)
		{
			if (a.x == b.x && a.y == b.y)
			{
				return false;
			}
			return true;
		}

		public int Distance(PFPoint point)
		{
			long dx = x - point.x;
			long dy = y - point.y;
			double lenS = (double)(dx * dx + dy * dy);
			int len = (int)Math.Sqrt(lenS);
			return len;
		}
	}


	public struct PFRect
	{
		public int x;
		public int y;
		public int x1;
		public int y1;

		public PFRect(int x, int y, int x1, int y1)
		{
			this.x = x;
			this.y = y;
			this.x1 = x1;
			this.y1 = y1;
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

	public class PFMath
	{
		public static bool RectCircleIntersect(PFRect rect, PFPoint point, int radius)
		{
			PFPoint c = new PFPoint((rect.x + rect.x1) / 2, (rect.y + rect.y1) / 2);
			PFPoint v = new PFPoint(Math.Abs(point.x - c.x), Math.Abs(point.y - c.y));
			PFPoint h = new PFPoint(rect.x1 - c.x, rect.y1 - c.y);
			PFPoint u = new PFPoint(Math.Max(v.x - h.x, 0), Math.Max(v.y - h.y, 0));
			return u.x * u.x + u.y * u.y <= radius * radius;
		}
	}
}
