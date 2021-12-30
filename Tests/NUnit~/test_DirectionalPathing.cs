using NUnit.Framework;
using DirectionalPathingLayers;
namespace PathfindingDirectionalLayers.Tests.NUnit_
{
    public class test_DirectionalPathing
    {


        [Test]
        public void Test_PathingOrder() //Tests that a path can be found forward and in the correct order. 
        {
            Vector2_int[] expectedPath = new Vector2_int[6] { new Vector2_int(0, 0), new Vector2_int(0, 1), new Vector2_int(0, 2), new Vector2_int(0, 3), new Vector2_int(0, 4), new Vector2_int(0, 5) };
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));

            Pathfinder pathFinderMap = new Pathfinder(0, 0, 0, 5, mapLayer, new PathfindSettings());

            int earlyExititer = 0;
            while (pathFinderMap.Iterate() == false && earlyExititer++ < 10)
            {
                ;
            }
            Assert.IsTrue(pathFinderMap.IsComplete);

            PathfinderNode[] path = pathFinderMap.GetCompletedPath();

            Assert.AreEqual(path.Length, expectedPath.Length);

            //Test expected positions and ordering
            for (int i = 0; i < path.Length; ++i)
            {
                Assert.AreEqual(path[i].pos, expectedPath[i]);
            }
        }

        [Test]
        public void Test_FailStartOutside() //Test expected failure to path when start is outside bounds.
        {
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));
            Pathfinder pathFinderMap = new Pathfinder(100, 0, 0, 0, mapLayer, new PathfindSettings());

            Assert.IsTrue(pathFinderMap.IsComplete);
            Assert.IsNull(pathFinderMap.GetCompletedPath());
        }

        [Test]
        public void Test_FailEndOutside() //Test expected failure to path when end is outside bounds.
        {
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));
            Pathfinder pathFinderMap = new Pathfinder(0, 0, 100, 0, mapLayer, new PathfindSettings());

            Assert.IsTrue(pathFinderMap.IsComplete);
            Assert.IsNull(pathFinderMap.GetCompletedPath());
        }

        [Test]
        public void Test_FailTravelForward()
        {
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));

            DirectionalLayer addObject = new DirectionalLayer(20, 0);
            addObject.Set(new DirectionalNode(new int[] { 1, 0, 0, 0, 0, 0 }));


            mapLayer.AddDirectionalLayerAtPoint(addObject, 0, 0);

            Pathfinder pathFinderMap = new Pathfinder(0, 0, 0, 20, mapLayer, new PathfindSettings());

            //Run until all options are completed.
            int earlyExititer = 0;
            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
            {
                ;
            }

            Assert.IsTrue(pathFinderMap.GetCompletedPath() == null);
            Assert.IsTrue(pathFinderMap.OpenList.Count == 0);
            Assert.IsTrue(pathFinderMap.ClosedList.Count > 0);
        }

        [Test]
        public void Test_FailTravelBack()
        {
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));

            //Create a long line which will block the direction we want to move in.
            DirectionalLayer addObject = new DirectionalLayer(20, 0);
            addObject.Set(new DirectionalNode(new int[] { 0, 1, 0, 0, 0, 0 }));
            mapLayer.AddDirectionalLayerAtPoint(addObject, 0, 0);

            //Create our pathfinder.  
            Pathfinder pathFinderMap = new Pathfinder(0, 20, 0, 0, mapLayer, new PathfindSettings());

            //Run until all options are completed.
            int earlyExititer = 0;
            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
            {
                ;
            }

            //Because of the long line added, we should be unable to reach the end point.  All options should be explored.
            Assert.IsTrue(pathFinderMap.GetCompletedPath() == null);
            Assert.IsTrue(pathFinderMap.OpenList.Count == 0);
            Assert.IsTrue(pathFinderMap.ClosedList.Count > 0);
        }

        [Test]
        public void Test_FailTravelRight()
        {
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));

            //Create a long line which will block the direction we want to move in.
            DirectionalLayer addObject = new DirectionalLayer(0, 20);
            addObject.Set(new DirectionalNode(new int[] { 0, 0, 0, 1, 0, 0 }));
            mapLayer.AddDirectionalLayerAtPoint(addObject, 0, 0);

            //Create our pathfinder.  
            Pathfinder pathFinderMap = new Pathfinder(0, 0, 20, 0, mapLayer, new PathfindSettings());

            //Run until all options are completed.
            int earlyExititer = 0;
            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
            {
                ;
            }

            //Because of the long line added, we should be unable to reach the end point.  All options should be explored.
            Assert.IsTrue(pathFinderMap.GetCompletedPath() == null);
            Assert.IsTrue(pathFinderMap.OpenList.Count == 0);
            Assert.IsTrue(pathFinderMap.ClosedList.Count > 0);
        }

        [Test]
        public void Test_FailTravelLeft()
        {
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));

            //Create a long line which will block the direction we want to move in.
            DirectionalLayer addObject = new DirectionalLayer(0, 20);
            addObject.Set(new DirectionalNode(new int[] { 0, 0, 1, 0, 0, 0 }));
            mapLayer.AddDirectionalLayerAtPoint(addObject, 0, 0);

            //Create our pathfinder.  
            Pathfinder pathFinderMap = new Pathfinder(20, 0, 0, 0, mapLayer, new PathfindSettings());

            //Run until all options are completed.
            int earlyExititer = 0;
            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
            {
                ;
            }

            //Because of the long line added, we should be unable to reach the end point.  All options should be explored.
            Assert.IsTrue(pathFinderMap.GetCompletedPath() == null);
            Assert.IsTrue(pathFinderMap.OpenList.Count == 0);
            Assert.IsTrue(pathFinderMap.ClosedList.Count > 0);
        }


    }

}