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

		public long SquareDistance(PFPoint point)
		{
			long dx = x - point.x;
			long dy = y - point.y;
			return (dx * dx + dy * dy);
		}

		public PFPoint normalized(int len)
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
			return new PFPoint((int)(tempLen * tempX / totalLen), (int)(tempLen * tempY / totalLen));
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

	public class PFPathfinderMath
	{
		public static PFPoint MoveTo(PFPoint startPoint, PFPoint endPoint, int stepLen)
		{
			long dx = endPoint.x - startPoint.x;
			long dy = endPoint.y - startPoint.y;
			long len = endPoint.Distance(startPoint);
			if (stepLen > len)
			{
				return endPoint;
			}
			else
			{
				long moveDistanceX = (int)(stepLen * dx / len);
				long moveDistanceY = (int)(stepLen * dy / len);
				long nextX = startPoint.x + moveDistanceX;
				long nextY = startPoint.y + moveDistanceY;
				return new PFPoint((int)nextX, (int)nextY);
			}
		}
	}
}
