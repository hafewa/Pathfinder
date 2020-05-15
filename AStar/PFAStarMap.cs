using MctClient.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.PlayerLoop;

namespace Pathfinder
{
    public class PFAStarMap
    {
        public PFAStarAlgorithm algorithm;

        public int mapWidth;
        public int mapHeight;
        public int tiledWidth;
        public int tiledHeight;

        Dictionary<int, LinkedList<PFAStarNode>> aStarNodeDictionary;

        public PFAStarMap(int mapWidth, int mapHeight, int tiledWidth, int tiledHeight)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.tiledWidth = tiledWidth;
            this.tiledHeight = tiledHeight;
            algorithm = new PFAStarAlgorithm(mapWidth / tiledWidth, mapHeight / tiledHeight);
            aStarNodeDictionary = new Dictionary<int, LinkedList<PFAStarNode>>();
        }

        public void AddAStarNode(PFAStarNode aStarNode)
        {
#if DEBUG
            if (aStarNode.parentLinkedNode != null)
            {
                Debug.Log(string.Format("添加A星节点异常：节点已有父节点"));
                return;
            }
#endif
            aStarNode.parent = this;
            int tiledX = aStarNode.point.x / tiledWidth;
            int tiledY = aStarNode.point.y / tiledHeight;
            LinkedList<PFAStarNode> nodeLinkedList;
            int key = tiledX * algorithm.height + tiledY;
            if(!aStarNodeDictionary.TryGetValue(key, out nodeLinkedList)){
                nodeLinkedList = new LinkedList<PFAStarNode>();
                aStarNodeDictionary.Add(key, nodeLinkedList);
            }
            nodeLinkedList.AddLast(aStarNode);
            aStarNode.parentLinkedNode = nodeLinkedList.Last;

            byte resultMask = 0;
            foreach (PFAStarNode tempNode in nodeLinkedList)
            {
                resultMask |= tempNode.mask;
            }
            algorithm.ResetMaskRuntime(new PFPoint(tiledX, tiledY), resultMask);
        }

        public void RemoveAStarNode(PFAStarNode aStarNode)
        {
#if DEBUG
            if(aStarNode.parentLinkedNode == null)
            {
                Debug.Log(string.Format("添加A星节点异常：节点无有父节点"));
                return;
            }
#endif
            int tiledX = aStarNode.point.x / tiledWidth;
            int tiledY = aStarNode.point.y / tiledHeight;
            int key = tiledX * algorithm.height + tiledY;
            LinkedList<PFAStarNode> nodeLinkedList = aStarNodeDictionary[key];
            nodeLinkedList.Remove(aStarNode.parentLinkedNode);
            aStarNode.parentLinkedNode = null;

            byte resultMask = 0;
            foreach(PFAStarNode tempNode in nodeLinkedList)
            {
                resultMask |= tempNode.mask;
            }
            algorithm.ResetMaskRuntime(new PFPoint(tiledX, tiledY), resultMask);
        }

        public void ResetRuntime()
        {
            algorithm.ResetRuntime();
            foreach(LinkedList<PFAStarNode> nodeLinedList in aStarNodeDictionary.Values)
            {
                foreach(PFAStarNode aStarPoint in nodeLinedList)
                {
                    MctCacheManager.AddInstantiateToCache(aStarPoint);
                }
                nodeLinedList.Clear();
            }
        }
    }
}
