using MctClient.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

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

	public abstract class PFIQuadNode : IMctCache
	{

		//挂载的父节点
		public PFQuadTree parent;
		public LinkedListNode<PFQuadCircle> parentLinkNode;

		//与父节点关系
		public PFQuadNodeState nodeState;

		//形状
		public PFQuadShape shape;

		//坐标
		public PFPoint point;

		public abstract PFQuadNodeState CheckState(PFQuadTree quadTree);

		public abstract void ReleaseToCache();

		public abstract void ResetFromCache();
	}

	public class PFQuadCircle : PFIQuadNode
	{

		List<PFQuadCircle> nearQuadNodes;

		//半径
		public int radius;

		public PFQuadCircle(PFPoint point, int radius)
		{
			shape = PFQuadShape.Circle;
			nodeState = PFQuadNodeState.Out;
			this.parent = null;
			this.parentLinkNode = null;
			this.point = point;
			this.radius = radius;
			nearQuadNodes = new List<PFQuadCircle>();
		}

		public void	InitRuntime(PFPoint point, int radius)
		{
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
				if(PFMathIntersection.RectCircleIntersect(quadTree.rect, point, radius))
				{
					return PFQuadNodeState.Intersect;
				}
				else
				{
					return PFQuadNodeState.Out;
				}
			}
		}

		public void UpdatePosition(PFPoint point)
		{
			nearQuadNodes.Clear();
			parent.FindNearQuadNodes(this, nearQuadNodes);
			if(nearQuadNodes.Count > 0)
			{
				PFPoint movePoint = new PFPoint(0, 0);
				foreach (var quadNode in nearQuadNodes)
				{
					if (quadNode == this)
					{
						continue;
					}
					long distanceSquare = point.SquareDistance(quadNode.point);
					long dr = radius + quadNode.radius;
					if (distanceSquare < dr * dr)
					{
						int distance = (int)Math.Sqrt(distanceSquare);
						PFPoint tempMovePoint = point - quadNode.point;
						movePoint = movePoint + tempMovePoint.normalized(radius + quadNode.radius - distance);
					}
				}
				point += movePoint;
			}
			this.point = point;
			Update();
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
				if(parent.children == null)
				{
					return;
				}
				int index;
				index = point.x <= (parent.rect.x + parent.rect.x1) / 2 ? 0 : 1;
				index = point.y <= (parent.rect.y + parent.rect.y1) / 2 ? index : index + 2;
				if (CheckState(parent.children[index]) != PFQuadNodeState.In)
				{
					return;
				}
			}
			PFQuadTree tempParent = parent;
			parent.RemoveQuadNode(this);
			tempParent.root.AddQuadNode(this);
			tempParent.Update();
		}

		public override void ResetFromCache()
		{

		}

		public override void ReleaseToCache()
		{
			nearQuadNodes.Clear();
			parent = null;
			parentLinkNode = null;
			nodeState = PFQuadNodeState.Out;
		}
	}
}
