

using MctClient.Framework;
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

    class PFAStarMath
    {
    }
}
