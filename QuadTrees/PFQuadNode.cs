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

		public PFQuadCircle(PFPoint point, int radius)
		{
			shape = PFQuadShape.Circle;
			this.parent = null;
			this.parentLinkNode = null;
			this.point = point;
			this.radius = radius;
		}

		/// <summary>
		///判断节点与区域的关系
		/// </summary>
		/// <param name="quadTree"></param>
		/// <returns></returns>
		public override PFQuadNodeState CheckState(PFQuadTree quadTree)
		{
			if((point.x - radius >= quadTree.rect.x) && 
				(point.x + radius <= quadTree.rect.x1) && 
				(point.y - radius >= quadTree.rect.y) &&
				(point.y + radius <= quadTree.rect.y1)
			)
			{
				return PFQuadNodeState.In;
			}
			else
			{
				if(PFMath.RectCircleIntersect(quadTree.rect, point, radius))
				{
					return PFQuadNodeState.Intersect;
				}
				else
				{
					return PFQuadNodeState.Out;
				}
			}
		}

		/// <summary>
		///	更新节点
		/// </summary>
		public void Update()
		{
			if(parent == null)
			{
				return;
			}
			if(CheckState(parent) == PFQuadNodeState.In){
				return;
			}
			PFQuadTree tempParent = parent;
			parent.RemoveQuadNode(this);
			tempParent.root.AddQuadNode(this);
			tempParent.Update();
		}

		public override void ReleaseToCache()
		{
			parent = null;
		}
	}
}
