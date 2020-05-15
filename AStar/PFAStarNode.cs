

using MctClient.Framework;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace Pathfinder
{

    public enum PFAStarPointState
    {
        Open,

        Close,
    }


    //不可以超过新增类型，最大值0x80
    public enum PFAStarMask
    {
        None = 0,

        //障碍物
        Obstacle = 0x1,

        //普通单位
        Agent = 0x2,

        //大型单位
        Monster = 0x4,

        //自定义
        Custom1 = 0x8,

        //自定义
        Custom2 = 0x10,

        //自定义
        Custom3 = 0x20,

        //自定义
        Custom4 = 0x40,

        //自定义
        Custom5 = 0x80,
    }

    public enum PFAStarResult
    {
        None,

        Success,

        Failure,

        OutOfStep,
    }

    public class PFAStarPoint : IMctCache
    {
        public PFPoint tiledPoint;

        public int F;
        public int G;
        public int H;

        //寻路节点的父节，用于串联路径
        public PFAStarPoint parent;

        //寻路节点状态，Open or Close
        public PFAStarPointState state;

        //寻路步数，用于排除寻路深度，当深度过深，则终端寻路
        public int step;

        public PFAStarPoint(PFAStarPoint parent, PFPoint tiledPoint)
        {
            ResetRuntime(parent, tiledPoint);
        }

        public void ResetRuntime(PFAStarPoint parent, PFPoint tiledPoint)
        {
            this.parent = parent;
            this.tiledPoint = tiledPoint;
            F = 0;
            G = 0;
            H = 0;
            state = PFAStarPointState.Open;
            step = 0;
        }

        public void ResetFromCache()
        {

        }

        public void ReleaseToCache()
        {
            state = PFAStarPointState.Close;
            this.parent = null;
        }

    }
    public class PFAStarNode : IMctCache
    {
        public PFPoint point;

        public byte mask;

        public PFAStarMap parent;

        public LinkedListNode<PFAStarNode> parentLinkedNode;


        public PFAStarNode(PFPoint point, byte mask)
        {
            this.point = point;
            this.mask = mask;
            parent = null;
            parentLinkedNode = null;
        }

        public void InitRuntime(PFPoint point, byte mask)
        {
            this.point = point;
            this.mask = mask;
        }

        public void UpdatePosition(PFPoint point)
        {
            int lastTiledX = this.point.x / parent.tiledWidth;
            int lastTiledY = this.point.y / parent.tiledHeight;
            int tiledX = point.x / parent.tiledWidth;
            int tiledY = point.y / parent.tiledHeight;
            if ((lastTiledX == tiledX) && (lastTiledY == tiledY))
            {
                this.point = point;
                return;
            }
            parent.RemoveAStarNode(this);
            this.point = point;
            parent.AddAStarNode(this);
        }

        public void ResetFromCache()
        {
        }

        public void ReleaseToCache()
        {
            parentLinkedNode = null;
            parent = null;
        }
    }
}
