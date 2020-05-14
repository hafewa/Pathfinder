

using MctClient.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Pathfinder
{
    class PFAStarAlgorithm
    {
        int[] AROUND_POINT_POS = { -1, -1, -1, 0, -1, 1, 0, -1, 0, 1, 1, -1, 1, 0, 1, 1 };
        int SQRT2 = 1414;

        public int width;
        public int height;

        //掩码信息
        byte[,] tiledMasks;
        byte[,] tiledMasksRuntime;

        LinkedList<PFAStarPoint> openLinkedList;
        LinkedList<PFAStarPoint> closeLinkedList;

        Dictionary<int, PFAStarPoint> starPointMap;

        public PFAStarAlgorithm(int width, int height)
        {
            if(width > 10000 || width > 10000)
            {
                Debug.LogError("PFAStartAlgorithm Width和Height > 10000");
            }
            this.width = width;
            this.height = height;
            openLinkedList = new LinkedList<PFAStarPoint>();
            closeLinkedList = new LinkedList<PFAStarPoint>();
            starPointMap = new Dictionary<int, PFAStarPoint>();
            tiledMasks = new byte[width, height];
            tiledMasksRuntime = new byte[width, height];
        }

        public void InitStaticObjstacle(PFPoint[] obstacleList)
        {
            foreach (PFPoint point in obstacleList)
            {
                AddObstacle(point);
            }
        }

        public void ResetRuntime()
        {
            Array.Copy(tiledMasks, 0, tiledMasksRuntime, 0, tiledMasks.Length);
            ClearOpenAndCloseList();
            starPointMap.Clear();
        }

        public void CalculatePath(PFPoint startPoint, PFPoint endPoint, byte limitMask, List<PFAStarPoint> resultList, int maxStep = -1)
        {
            if (maxStep < 0)
            {
                maxStep = width * height;
            }
            ResetRuntime();
            PFAStarPoint aStarPoint = CreatAStartPoint(null, startPoint);
            aStarPoint.G = 0;
            aStarPoint.H = Math.Abs(startPoint.x - endPoint.x) + Math.Abs(startPoint.y - endPoint.y);
            aStarPoint.F = aStarPoint.G + aStarPoint.H;

            openLinkedList.AddLast(aStarPoint);

            PFAStarResult flag = PFAStarResult.None;
            PFAStarPoint currentAStarPoint;
            LinkedListNode<PFAStarPoint> tempNode;
            LinkedListNode<PFAStarPoint> tempNodeNext;
            LinkedListNode<PFAStarPoint> tempNodeCurrent;
            PFAStarPoint tempAStarPoint;
            while (true)
            {
                if(openLinkedList.Count == 0)
                {
                    flag = PFAStarResult.Failure;
                    break;
                }

                //获取最小F值的节点
                tempNodeCurrent = openLinkedList.First;
                tempNode = tempNodeCurrent.Next;
                while (tempNode != null)
                {
                    tempNodeNext = tempNode.Next;
                    if (tempNode.Value.F < tempNodeCurrent.Value.F)
                    {
                        tempNodeCurrent = tempNode;
                    }
                    tempNode = tempNodeNext;
                }
                currentAStarPoint = tempNodeCurrent.Value;

                //把该节点移动到closeList
                openLinkedList.Remove(tempNodeCurrent);
                currentAStarPoint.state = PFAStarPointState.Close;
                closeLinkedList.AddLast(currentAStarPoint);

                //打开该节点的周围节点
                if(currentAStarPoint.step > maxStep)
                {
                    flag = PFAStarResult.OutOfStep;
                    break;
                }
                bool isFind = false;
                PFPoint currentPoint = currentAStarPoint.tiledPoint;
                int cTiledX = currentAStarPoint.tiledPoint.x;
                int cTiledY = currentAStarPoint.tiledPoint.y;
                for (int index = 0; index < AROUND_POINT_POS.Length; index += 2)
                {
                    int dx = AROUND_POINT_POS[index];
                    int dy = AROUND_POINT_POS[index + 1];
                    int openTiledX = cTiledX + dx;
                    int openTiledY = cTiledY + dy;
                    if(openTiledX < 0 || openTiledX >= width || openTiledY < 0 || openTiledY >= height)
                    {
                        continue;
                    }

                    //如果阻挡掩码匹配，则视为不可移动
                    int openMask = tiledMasksRuntime[openTiledX, openTiledY];
                    if((openMask & limitMask) > 0)
                    {
                        continue;
                    }
                    int gValue = 0;
                    bool isCanTurnAround = false;
                    if(index == 1 || index == 5 || index == 11 || index == 15)
                    {
                        gValue = SQRT2;
                        int walkX1 = openTiledX + dx + 1;
                        int walkY1 = openTiledY + 1;
                        if ((walkX1 >= 0) && (walkX1 < width) && (walkY1 >= 0) && (walkY1 < height) && ((tiledMasksRuntime[walkX1, walkY1] & limitMask) == 0))
                        {
                            walkX1 = openTiledX + 1;
                            walkY1 = openTiledY + dy + 1;
                            if((walkX1 >= 0) && (walkX1 < width) && (walkY1 >= 0) && (walkY1 < height) && ((tiledMasksRuntime[walkX1, walkY1] & limitMask) == 0))
                            {
                                isCanTurnAround = true;
                            }
                        }
                    }
                    else
                    {
                        gValue = 1;
                        isCanTurnAround = true;
                    }
                    if (!isCanTurnAround)
                    {
                        continue;
                    }
                    int key = openTiledX * height + openTiledY;
                    if(starPointMap.TryGetValue(key, out tempAStarPoint))
                    {
                        if (tempAStarPoint.state == PFAStarPointState.Open)
                        {
                            int pointG = currentAStarPoint.G + gValue;
                            if (pointG < tempAStarPoint.G)
                            {
                                tempAStarPoint.G = pointG;
                                tempAStarPoint.H = pointG + tempAStarPoint.H;
                                tempAStarPoint.parent = currentAStarPoint;
                            }
                        }
                    }
                    else
                    {
                        PFAStarPoint newAStarPoint = CreatAStartPoint(currentAStarPoint, new PFPoint(openTiledX, openTiledY));
                        newAStarPoint.G = currentAStarPoint.G + gValue;
                        newAStarPoint.H = currentAStarPoint.H = Math.Abs(openTiledX - endPoint.x) + Math.Abs(openTiledY - endPoint.y);
                        newAStarPoint.F = newAStarPoint.G + newAStarPoint.H;
                        newAStarPoint.step = currentAStarPoint.step + 1;
                        openLinkedList.AddLast(newAStarPoint);
                    }
                    if((openTiledX == endPoint.x) && (openTiledY == endPoint.y))
                    {
                        isFind = true;
                        break;
                    }
                }
                if (isFind)
                {
                    flag = PFAStarResult.Success;
                    break;
                }
            }
            if (flag != PFAStarResult.Success)
            {
                return;
            }
            currentAStarPoint = starPointMap[endPoint.x * height + endPoint.y];
            while (currentAStarPoint != null)
            {
                resultList.Add(currentAStarPoint);
                currentAStarPoint = currentAStarPoint.parent;
            }
        }

        public void AddObstacle(PFPoint point)
        {
            if (point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
            {
                return;
            }
            tiledMasks[point.x, point.y] |= (byte)PFAStarMask.Obstacle;
        }

        public void AddMask(PFPoint point, PFAStarMask astarMask)
        {
            if (point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
            {
                return;
            }
            tiledMasks[point.x, point.y] |= (byte)astarMask;
        }

        public void AddObstacleRuntime(PFPoint point)
        {
            if(point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
            {
                return;
            }
            tiledMasksRuntime[point.x, point.y] |= (byte)PFAStarMask.Obstacle;
        }

        public void AddMaskRuntime(PFPoint point, PFAStarMask astarMask)
        {
            if (point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
            {
                return;
            }
            tiledMasksRuntime[point.x, point.y] |= (byte)astarMask;
        }

        public void ClearOpenAndCloseList()
        {
            foreach (var starPoint in openLinkedList)
            {
                MctCacheManager.AddInstantiateToCache(starPoint);
            }
            openLinkedList.Clear();
            foreach (var starPoint in closeLinkedList)
            {
                MctCacheManager.AddInstantiateToCache(starPoint);
            }
            closeLinkedList.Clear();
        }

        public PFAStarPoint CreatAStartPoint(PFAStarPoint parent, PFPoint tiledPoint)
        {
            PFAStarPoint aStartPoint = MctCacheManager.GetInstantiateFromCache<PFAStarPoint>();
            if(aStartPoint == null)
            {
                aStartPoint = new PFAStarPoint(parent, tiledPoint);
            }
            else
            {
                aStartPoint.ResetRuntime(parent, tiledPoint);
            }
            int key = tiledPoint.x * height + tiledPoint.y;
            starPointMap.Add(key, aStartPoint);
            return aStartPoint;
        }
    }
}
