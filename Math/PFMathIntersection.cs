using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Pathfinder
{
	public class PFMathIntersection
	{
		/// <summary>
		/// AABB于圆相交
		/// </summary>
		/// <param name="rect">AABB</param>
		/// <param name="point">圆心</param>
		/// <param name="radius">半径</param>
		/// <returns></returns>
		public static bool RectCircleIntersect(PFRect rect, PFPoint point, int radius)
		{
			long cx = (rect.x + rect.x1) / 2;
			long cy = (rect.y + rect.y1) / 2;

			long vx = Math.Abs(point.x - cx);
			long vy = Math.Abs(point.y - cy);

			long hx = rect.x1 - cx;
			long hy = rect.y1 - cy;

			long ux = Math.Max(vx - hx, 0);
			long uy = Math.Max(vy - hy, 0);
			return ux * ux + uy * uy <= (long)radius * (long)radius;
		}

		/// <summary>
		/// 圆和圆相交
		/// </summary>
		/// <param name="point1">圆心1</param>
		/// <param name="radius1">半径1</param>
		/// <param name="point2">圆心2</param>
		/// <param name="radius2">半径2</param>
		/// <returns></returns>
		public static bool CircleCircleIntersect(PFPoint point1, int radius1, PFPoint point2, int radius2)
		{
			long distanceSquare = point1.SquareDistance(point2);
			long dr = (long)(radius1 + radius2);
			return distanceSquare < dr * dr;
		}
	}
}
