using NUnit.Framework;

namespace PathfindingDirectionalLayers.Tests.NUnit_
{
    public class test_DirectionalPathing
    {
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

            Assert.IsTrue(pathFinderMap.completedPath == null);
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
            Assert.IsTrue(pathFinderMap.completedPath == null);
            Assert.IsTrue(pathFinderMap.OpenList.Count == 0);
            Assert.IsTrue(pathFinderMap.ClosedList.Count > 0);
        }

        [Test]
        public void Test_FailTravelRight()
        {
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));

            //Create a long line which will block the direction we want to move in.
            DirectionalLayer addObject = new DirectionalLayer(0, 20);
            addObject.Set(new DirectionalNode(new int[] { 0, 0, 1, 0, 0, 0 }));
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
            Assert.IsTrue(pathFinderMap.completedPath == null);
            Assert.IsTrue(pathFinderMap.OpenList.Count == 0);
            Assert.IsTrue(pathFinderMap.ClosedList.Count > 0);
        }

        [Test]
        public void Test_FailTravelLeft()
        {
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));

            //Create a long line which will block the direction we want to move in.
            DirectionalLayer addObject = new DirectionalLayer(0, 20);
            addObject.Set(new DirectionalNode(new int[] { 0, 0, 0, 1, 0, 0 }));
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
            Assert.IsTrue(pathFinderMap.completedPath == null);
            Assert.IsTrue(pathFinderMap.OpenList.Count == 0);
            Assert.IsTrue(pathFinderMap.ClosedList.Count > 0);
        }


    }

}