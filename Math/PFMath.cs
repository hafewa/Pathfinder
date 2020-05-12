using System;
using UnityEngine;

namespace Pathfinder
{
	public struct PFPoint
	{
		public long x;
		public long y;

		public PFPoint(long x, long y)
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

		public long Distance(PFPoint point)
		{
			long dx = x - point.x;
			long dy = y - point.y;
			double lenS = (double)(dx * dx + dy * dy);
			long len = (long)Math.Sqrt(lenS);
			return len;
		}

		public long SquareDistance(PFPoint point)
		{
			long dx = x - point.x;
			long dy = y - point.y;
			return dx * dx + dy * dy;
		}

		public PFPoint normalized(long len)
		{
			if(x == 0 && y == 0)
			{
				return new PFPoint(0, 0);
			}
			long tempLen = len;
			long tempX = x;
			long tempY = y;
			double lenS = tempX * tempX + tempY * tempY;
			long totalLen = (long)Math.Sqrt(lenS);
			return new PFPoint(len * tempX / totalLen, tempLen * tempY / totalLen);
		}
	}


	public struct PFRect
	{
		public long x;
		public long y;
		public long x1;
		public long y1;

		public PFRect(long x, long y, long x1, long y1)
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
		/// <summary>
		/// AABB于圆相交
		/// </summary>
		/// <param name="rect">AABB</param>
		/// <param name="point">圆心</param>
		/// <param name="radius">半径</param>
		/// <returns></returns>
		public static bool RectCircleIntersect(PFRect rect, PFPoint point, long radius)
		{
			long cx = (rect.x + rect.x1) / 2;
			long cy = (rect.y + rect.y1) / 2;

			long vx = Math.Abs(point.x - cx);
			long vy = Math.Abs(point.y - cy);

			long hx = rect.x1 - cx;
			long hy = rect.y1 - cy;

			long ux = Math.Max(vx - hx, 0);
			long uy = Math.Max(vy - hy, 0);
			return ux * ux + uy * uy <= radius * radius;
		}

		/// <summary>
		/// 圆和圆相交
		/// </summary>
		/// <param name="point1">圆心1</param>
		/// <param name="radius1">半径1</param>
		/// <param name="point2">圆心2</param>
		/// <param name="radius2">半径2</param>
		/// <returns></returns>
		public static bool CircleCircleIntersect(PFPoint point1, long radius1, PFPoint point2, long radius2)
		{
			long distanceSquare = point1.SquareDistance(point2);
			long dr = radius1 + radius2;
			return distanceSquare < dr * dr;
		}
	}
}
