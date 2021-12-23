using NUnit.Framework;

namespace pathfinding.directional.layers.Tests.NUnit_
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

            Assert.AreEqual(pathfindSettings.EarlyExit_maxIterations, pathFinderMap.closedList.Count - 1);
            Assert.IsNull(pathFinderMap.completedPath);

            pathfindSettings = new PathfindSettings() { };
            //Create our pathfinder.  
            pathFinderMap = new Pathfinder(20, 0, 0, 0, mapLayer, pathfindSettings);

            //Run until all options are completed.
            earlyExititer = 0;
            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
            {
                ;
            }
            Assert.NotNull(pathFinderMap.completedPath);

        }


//#if RELEASE
//        [Test]
//#endif
//        public void Test_Summary()
//        {
//            //NBenchRunner.Run<Program>();
//        }
    }

    //public class TestClass
    //{
    //    private const int N = 1000;
    //    [Benchmark]
    //    public void Test_Benchmark()
    //    {
    //        MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));

    //        PathfindSettings pathfindSettings = new PathfindSettings() { };
    //        //Create our pathfinder.  
    //        Pathfinder pathFinderMap = new Pathfinder(20, 0, 0, 0, mapLayer, pathfindSettings);

    //        //Run until all options are completed.
    //        int earlyExititer = 0;
    //        while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
    //        {
    //            ;
    //        }


    //    }
    //}


}

//namespace NBench.Tests.Performance
//{
//    class Program
//    {
//#if RELEASE
//        [Test]
//#endif
//        public void Test_SimpleBenchmark()
//        {
//            NBenchRunner.Run<Program>();
//        }
//    }
//}



//namespace NBench.Tests.Performance
//{
//    /// <summary>
//    /// Test to see gauge the impact of having multiple things to measure on a benchmark
//    /// </summary>
//    public class CombinedPerfSpecs
//    {

//        MapDirectionalLayer mapLayer;
//        private readonly string CounterName = "Counter";
//        private Counter counter;

//        [PerfSetup]
//        public void SetUp(BenchmarkContext context)
//        {
//            this.counter = context.GetCounter(this.CounterName);
//            this.mapLayer = new MapDirectionalLayer(100, 100, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));
//        }

//        [PerfBenchmark(NumberOfIterations = 5, RunMode = RunMode.Throughput, RunTimeMilliseconds = 1000, TestMode = TestMode.Measurement, SkipWarmups = true)]
//        [CounterMeasurement("Counter")]
//       // [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
//        public void BenchmarkMethod(BenchmarkContext context)
//        {

//            MapDirectionalLayer mapLayer = this.mapLayer;

//            PathfindSettings pathfindSettings = new PathfindSettings() { };
//            //Create our pathfinder.  
//            Pathfinder pathFinderMap = new Pathfinder((int)(counter.Current%25 + 25), 0, 0, 0, mapLayer, pathfindSettings);

//            //Run until all options are completed.
//            int earlyExititer = 0;
//            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
//            {
//                ;
//            }
//            counter.Increment();
//        }





//        //private Counter _counter;
//        //MapDirectionalLayer mapLayer;
//        //[PerfSetup]
//        //public void Setup(BenchmarkContext context)
//        //{
//        //    //_counter = context.GetCounter("TestCounter");
//        //    this.mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));
//        //    //System.Console.Write("dah");
//        //}


//        //[PerfBenchmark(NumberOfIterations = 2, RunMode = RunMode.Iterations, TestMode = TestMode.Measurement, SkipWarmups = false)]
//        //[ElapsedTimeAssertion(MaxTimeMilliseconds = 5000)]
//        //public void Benchmark_Performance_ElaspedTime()
//        //{
//        //    System.Console.WriteLine("dah");
//        //    MapDirectionalLayer mapLayer = this.mapLayer;

//        //    PathfindSettings pathfindSettings = new PathfindSettings() { };
//        //    //Create our pathfinder.  
//        //    Pathfinder pathFinderMap = new Pathfinder(20, 0, 0, 0, mapLayer, pathfindSettings);

//        //    //Run until all options are completed.
//        //    int earlyExititer = 0;
//        //    while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
//        //    {
//        //        ;
//        //    }
//        //}
//    }
//}
