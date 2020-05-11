

using System.Collections.Generic;

namespace Pathfinder
{
    public class PFQuadCache
    {
        static public LinkedList<PFQuadTree> quadTrees = new LinkedList<PFQuadTree>();


        static public LinkedList<PFQuadCircle> quadCircles = new LinkedList<PFQuadCircle>();

        public static void AddQuadTree(PFQuadTree quadtree)
        {
            quadtree.ReleaseToCache();
            quadTrees.AddLast(quadtree);
        }

        public static void AddQuadCircle(PFQuadCircle quadCircle)
        {
            quadCircle.ReleaseToCache();
            quadCircles.AddLast(quadCircle);
        }
    }
}