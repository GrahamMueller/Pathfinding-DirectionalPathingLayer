using NUnit.Framework;

namespace PathfindingDirectionalLayers.Tests.NUnit_
{
    class test_Settings
    {

        //Test early exit max iterations activating correctly.  
        [Test]
        public void Test_EarlyExit_maxIterations()
        {
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));

            PathfindSettings pathfindSettings = new PathfindSettings() { EarlyExit_maxIterations = 10 };
            //Create our pathfinder.  
            Pathfinder pathFinderMap = new Pathfinder(20, 0, 0, 0, mapLayer, pathfindSettings);

            //Run until all options are completed.
            int earlyExititer = 0;
            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
            {
                ;
            }

            Assert.AreEqual(pathfindSettings.EarlyExit_maxIterations, pathFinderMap.ClosedList.Count - 1);
            Assert.IsNull(pathFinderMap.GetCompletedPath());

            pathfindSettings = new PathfindSettings() { };
            //Create our pathfinder.  
            pathFinderMap = new Pathfinder(20, 0, 0, 0, mapLayer, pathfindSettings);

            //Run until all options are completed.
            earlyExititer = 0;
            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
            {
                ;
            }
            Assert.NotNull(pathFinderMap.GetCompletedPath());

        }

    }

}
