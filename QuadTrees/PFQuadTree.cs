﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Experimental.PlayerLoop;

namespace Pathfinder
{

	public class PFQuadTree
	{
		public const int PATH_FINDER_CONTAIN_NUMBER = 3;

		bool isActive;

		public int depath;

		public PFQuadTree root;

		//父节点
		public PFQuadTree parent;

		//子节点
		public PFQuadTree[] children;

		public LinkedList<PFIQuadNode> quadLinkedList;

		//区域范围
		public PFRect rect;

		public PFQuadTree(PFQuadTree root, PFQuadTree parent, PFRect rect, int depath)
		{
			this.isActive = true;
			this.rect = rect;
			this.root = root == null ? this : root;
			this.parent = parent;
			children = null;
			this.depath = depath;
			quadLinkedList = new LinkedList<PFIQuadNode>();
		}

		public void ResetRuntime(PFQuadTree root, PFQuadTree parent, PFRect rect, int depath)
		{
			isActive = true;
			this.root = root == null ? this : root;
			this.parent = parent;
			this.rect = rect;
			this.depath = depath;
		}

		/// <summary>
		/// 添加节点
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="quadNode">节点</param>
		public PFQuadNodeState AddQuadNode(PFIQuadNode quadNode)
		{
			PFQuadNodeState state = quadNode.CheckState(this);
			int index = 0;
			switch (state)
			{
				//在自己范围内，如果在子节点内，则由子节点持有。如果与子节点相交或者不存在子节点，则自己持有
				case PFQuadNodeState.In:
					if (children != null)
					{
						index = quadNode.point.x <= (rect.x + rect.x1) / 2 ? 0 : 1;
						index = quadNode.point.y <= (rect.y + rect.y1) / 2 ? index : index + 2;
						switch (children[index].AddQuadNode(quadNode))
						{
							case (PFQuadNodeState.In):
								return PFQuadNodeState.In;
							case (PFQuadNodeState.Intersect):
								break;
							case (PFQuadNodeState.Out):
								Debug.Fail("AddQuadNode 异常：子节点状态OUT");
								return PFQuadNodeState.Out;
						}
					}
					quadLinkedList.AddLast(quadNode);
					quadNode.parentLinkNode = quadLinkedList.Last;
					quadNode.parent = this;
					break;

				//相交则不保存，由父节点继续持有
				case PFQuadNodeState.Intersect:
					if(parent == null)
					{
						Debug.Fail("AddQuadNode 异常：节点不在根数节点内");
					}
					break;
				case PFQuadNodeState.Out:
					Debug.Fail("AddQuadNode 异常：状态OUT");
					break;
			}

			//如果数量超过容纳范围，则将当前格子分裂4块
			if ((children == null) && (quadLinkedList.Count > PATH_FINDER_CONTAIN_NUMBER))
			{
				children = new PFQuadTree[4];
				for (int i = 0; i < 4; i++)
				{
					PFQuadTree lastValue = null;
					int x = rect.x + (rect.x1 - rect.x) / 2 * (i % 2);
					int y = rect.y + (rect.y1 - rect.y) / 2 * (i / 2);
					int x1 = x + (rect.x1 - rect.x) / 2;
					int y1 = y + (rect.y1 - rect.y) / 2;
					if (PFQuadCache.quadTrees.Count > 0)
					{
						lastValue = PFQuadCache.quadTrees.Last.Value;
						PFQuadCache.quadTrees.RemoveLast();
						lastValue.ResetRuntime(root, this, new PFRect(x, y, x1, y1), depath + 1);
						children[i] = lastValue;
					}
					else
					{
						children[i] = new PFQuadTree(root, this, new PFRect(x, y, x1, y1), depath + 1);
					}
				}

				//将父节点的节点转到子节点
				LinkedListNode<PFIQuadNode> node;
				LinkedListNode<PFIQuadNode> nodeNext;
				node = quadLinkedList.First;
				while (node != null)
				{
					nodeNext = node.Next;
					PFIQuadNode tempQuadNode = node.Value;
					index = tempQuadNode.point.x <= (rect.x + rect.x1) / 2 ? 0 : 1;
					index = tempQuadNode.point.y <= (rect.y + rect.y1) / 2 ? index : index + 2;
					switch (children[index].AddQuadNode(tempQuadNode))
					{
						case (PFQuadNodeState.In):
							quadLinkedList.Remove(node);
							break;
						case (PFQuadNodeState.Intersect):
							break;
						case (PFQuadNodeState.Out):
							Debug.Fail("AddQuadNode 异常：分配子节点状态OUT");
							break;
					}
					node = nodeNext;
				}
			}
			return state;
		}

		/// <summary>
		///	移除节点
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="quadNode">节点</param>
		public void RemoveQuadNode(PFIQuadNode quadNode)
		{
			quadNode.parent.quadLinkedList.Remove(quadNode.parentLinkNode);
			quadNode.parentLinkNode = null;
			quadNode.parent = null;
		}

		/// <summary>
		/// 移除子节点
		/// </summary>
		public void RemoveChildren()
		{
			if (children == null)
			{
				return;
			}
			RecycleQuadNodesFromChildren(quadLinkedList);
			foreach(var quadTress in children)
			{
				quadTress.ReleaseToCache();
			}
			children = null;
		}

		/// <summary>
		/// 回收子节点
		/// </summary>
		/// <param name="parentQuadLinkedList"></param>
		void RecycleQuadNodesFromChildren(LinkedList<PFIQuadNode> parentQuadLinkedList)
		{
			if (children == null)
			{
				foreach (var quadNode in quadLinkedList)
				{
					parentQuadLinkedList.AddLast(quadNode);
				}
				quadLinkedList.Clear();
				return;
			}
			foreach (var quadTress in children)
			{
				quadTress.RecycleQuadNodesFromChildren(parentQuadLinkedList);
			}
		}

		public void Update()
		{
			if (IsNeedRemoveChildren())
			{
				RemoveChildren();
				if(parent != null) parent.Update();
			}
		}

		/// <summary>
		///	检测是否需要移除子节点
		/// </summary>
		/// <returns></returns>
		public bool IsNeedRemoveChildren()
		{
			int cout = GetQuadNodeNumbers();
			if (cout > PATH_FINDER_CONTAIN_NUMBER)
			{
				return false;
			}
			return true;
		}

		public int GetQuadNodeNumbers()
		{
			int cout = quadLinkedList.Count;
			if (children != null)
			{
				foreach (var quadTress in children)
				{
					cout += quadTress.GetQuadNodeNumbers();
				}
			}
			return cout;
		}

		public void ReleaseToCache()
		{
			isActive = false;
			parent = null;
			
			//将树节点加入缓存
			if (children != null)
			{
				PFQuadCache.AddQuadTree(children[0]);
				PFQuadCache.AddQuadTree(children[1]);
				PFQuadCache.AddQuadTree(children[2]);
				PFQuadCache.AddQuadTree(children[3]);
				children = null;
			}

			//将节点加入缓存
			if (quadLinkedList.Count > 0)
			{
				foreach(PFIQuadNode quadNode in quadLinkedList)
				{
					switch (quadNode.shape)
					{
						case (PFQuadShape.Circle):
							PFQuadCache.AddQuadCircle((PFQuadCircle)quadNode);
							break;
					}
				}
				quadLinkedList.Clear();

				Debug.Fail("清理树节点异常：不应该存在PFIQuadNode");
			}
		}

	}
}