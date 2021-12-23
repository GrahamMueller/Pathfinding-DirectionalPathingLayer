//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Running;
//using BenchmarkDotNet.Configs;
//using BenchmarkDotNet.Loggers;
//using BenchmarkDotNet.Columns;
//using BenchmarkDotNet.Validators;

//namespace pathfinding.directional.layers.Tests.Benchmark_
//{
//    public class TestClass
//    {
//        private const int N = 1000;
//        MapDirectionalLayer smallMap;
//        MapDirectionalLayer largeMap;
//        public TestClass()
//        {
//            this.smallMap =  new MapDirectionalLayer(10, 10, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));
//            this.largeMap =  new MapDirectionalLayer(100, 100, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));
//        }


//        [Benchmark]
//        public void small_path()
//        {
//            MapDirectionalLayer mapLayer = this.smallMap;

//            PathfindSettings pathfindSettings = new PathfindSettings() { };
//            //Create our pathfinder.  
//            Pathfinder pathFinderMap = new Pathfinder(20, 0, 0, 0, mapLayer, pathfindSettings);

//            //Run until all options are completed.
//            int earlyExititer = 0;
//            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
//            {
//                ;
//            }
//        }

//        [Benchmark]
//        public void big_path()
//        {
//            MapDirectionalLayer mapLayer = this.largeMap;

//            PathfindSettings pathfindSettings = new PathfindSettings() { };
//            //Create our pathfinder.  
//            Pathfinder pathFinderMap = new Pathfinder(100, 0, 0, 0, mapLayer, pathfindSettings);

//            //Run until all options are completed.
//            int earlyExititer = 0;
//            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
//            {
//                ;
//            }


//        }
//    }

//    class Benchmark_Main
//    {
//        static void Main(string[] args)
//        {
//            var config = new ManualConfig()
//                .WithOptions(ConfigOptions.DisableOptimizationsValidator)
//                .AddValidator(JitOptimizationsValidator.DontFailOnError)
//                .AddLogger(ConsoleLogger.Default)
//                .AddColumnProvider(DefaultColumnProviders.Instance);

//            var summary = BenchmarkRunner.Run<TestClass>(config);
//        }
//    }

//}
