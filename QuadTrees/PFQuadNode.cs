using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Pathfinder
{
	public enum PFQuadNodeState
	{
		In,

		Out,

		Intersect,
	}

	public enum PFQuadShape
	{
		Circle,
	}

	public abstract class PFIQuadNode
	{
		//挂载的父节点
		public PFQuadTree parent;

		public LinkedListNode<PFIQuadNode> parentLinkNode;

		//形状
		public PFQuadShape shape;

		//坐标
		public PFPoint point;

		public abstract PFQuadNodeState CheckState(PFQuadTree quadTree);

		public abstract void ReleaseToCache();
	}

	public class PFQuadCircle : PFIQuadNode
	{

		//半径
		int radius;

		public PFQuadCircle(PFQuadTree parent,  PFPoint point, int radius)
		{
			shape = PFQuadShape.Circle;
			this.parent = parent;
			this.parentLinkNode = null;
			this.point = point;
			this.radius = radius;
		}

		public override PFQuadNodeState CheckState(PFQuadTree quadTree)
		{
			return PFQuadNodeState.In;
		}

		public override void ReleaseToCache()
		{
			parent = null;
		}
	}
}
