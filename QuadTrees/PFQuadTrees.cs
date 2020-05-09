using System;



namespace Pathfinder
{

	public class PFQuadTrees
	{
		//父节点
		PFQuadTrees parent;

		//子节点
		PFQuadTrees[] children;

		//可容纳节点数
		int containNumber;

		//区域范围
		int minX;
		int minY;
		int maxX;
		int maxY;
		
		public PFQuadTrees(PFQuadTrees parent, ref PFRect rect, int containNumber = 8)
		{

		}
	}
}
