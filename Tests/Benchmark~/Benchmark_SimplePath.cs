using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using DirectionalPathingLayers;
using NUnit.Framework;
namespace PathfindingDirectionalLayers.Tests.Benchmark_
{
    [ShortRunJob]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    public class TestClass
    {
        private const int N = 10;

        int mapWidth = 25;
        int mapHeight = 250;
        MapDirectionalLayer map_250;
        MapDirectionalLayer map_250_complex;
        public TestClass()
        {
            this.map_250 = new MapDirectionalLayer(this.mapWidth, this.mapHeight, new DirectionalNode(1, 1, 1, 1, 0, 0));
            this.map_250_complex = new MapDirectionalLayer(this.mapWidth, this.mapHeight, new DirectionalNode(1, 1, 1, 1, 0, 0));

            DirectionalLayer horizontalWall = new DirectionalLayer(this.mapWidth - 1, 0);
            horizontalWall.Set(1);
            for (int i = -this.mapHeight + 5; i < this.mapHeight - 5; i += 6)
            {
                if (i / 2 % 2 == 0)
                {
                    this.map_250_complex.AddDirectionalLayerAtPoint(horizontalWall, -2, i);
                }

                else
                {
                    this.map_250_complex.AddDirectionalLayerAtPoint(horizontalWall, 2, i);

                }
            }

        }

        [Params(1, 5, 10, 20, 30, 40, 50, 100, 150, 200)]
        public static int pathIterations;


        [Benchmark]
        public void profile_SimpleStraightPath()
        {
            MapDirectionalLayer mapLayer = this.map_250;

            PathfindSettings pathfindSettings = new PathfindSettings() { };
            //Create our pathfinder.  
            Pathfinder pathFinderMap = new Pathfinder(this.mapWidth, 0, this.mapWidth, this.mapHeight * 2, mapLayer, pathfindSettings);

            //Run until all options are completed.
            int earlyExititer = 0;
            while (pathFinderMap.Iterate() == false && earlyExititer++ < pathIterations)
            {
                ;
            }

        }

        //[Benchmark]
        //public void profile_ComplexPath()
        //{
        //    MapDirectionalLayer mapLayer = this.map_250_complex;

        //    PathfindSettings pathfindSettings = new PathfindSettings() { };
        //    //Create our pathfinder.  
        //    Pathfinder pathFinderMap = new Pathfinder(this.mapWidth, 0, this.mapWidth, startPosition, mapLayer, pathfindSettings);

        //    //Run until all options are completed.
        //    int earlyExititer = 0;
        //    while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000000)
        //    {
        //        ;
        //    }

        //}

        //[Test]
        //public void SimpleVisualize_ComplexPath()
        //{
        //    MapDirectionalLayer mapLayer = this.map_250_complex;

        //    PathfindSettings pathfindSettings = new PathfindSettings() { };
        //    //Create our pathfinder.  
        //    Pathfinder pathFinderMap = new Pathfinder(this.mapWidth, 2, this.mapWidth, this.mapHeight * 2 - 2, mapLayer, pathfindSettings);

        //    //Run until all options are completed.
        //    int earlyExititer = 0;
        //    while (pathFinderMap.Iterate() == false && earlyExititer++ < 10000000)
        //    {
        //        ;
        //    }

        //    PathfinderVisualizer.SaveAsPng(pathFinderMap, "complexPath.png", 6);
        //}

#if RELEASE
        [Test]
        public void ProfilePaths()
        {
            ManualConfig config = new ManualConfig()
                .WithOptions(ConfigOptions.DisableOptimizationsValidator)
                .AddValidator(JitOptimizationsValidator.DontFailOnError)
                .AddLogger(ConsoleLogger.Default)
                .AddColumnProvider(DefaultColumnProviders.Instance);

            BenchmarkDotNet.Reports.Summary summary = BenchmarkRunner.Run<TestClass>(config);
        }
#endif
        //[Benchmark]
        //public void path_100()
        //{
        //    MapDirectionalLayer mapLayer = this.map_500;

        //    PathfindSettings pathfindSettings = new PathfindSettings() { };
        //    //Create our pathfinder.  
        //    Pathfinder pathFinderMap = new Pathfinder(100, 0, 0, 0, mapLayer, pathfindSettings);

        //    //Run until all options are completed.
        //    int earlyExititer = 0;
        //    while (pathFinderMap.Iterate() == false && earlyExititer++ < 10000)
        //    {
        //        ;
        //    }
        //}

        //[Benchmark]
        //public void path_500()
        //{
        //    MapDirectionalLayer mapLayer = this.map_500;

        //    PathfindSettings pathfindSettings = new PathfindSettings() { };
        //    //Create our pathfinder.  
        //    Pathfinder pathFinderMap = new Pathfinder(500, 0, 0, 0, mapLayer, pathfindSettings);

        //    //Run until all options are completed.
        //    int earlyExititer = 0;
        //    while (pathFinderMap.Iterate() == false && earlyExititer++ < 10000)
        //    {
        //        ;
        //    }
        //}

    }

}
