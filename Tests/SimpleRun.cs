using System;
using NUnit.Framework;
using DirectionalPathingLayers;
using PathfindingDirectionalLayers.Utility;

namespace PathfindingDirectionalLayers.Tests
{
    class VisualTests
    {
        [Test]
        public void Test_Visual_Simple()
        {
            string outputImagePath = "visual_simple.png";

            MapDirectionalLayer mapLayer = new MapDirectionalLayer(10, 10, new DirectionalNode( 1, 1, 1, 1, 0, 0 ));

            DirectionalLayer addObject = new DirectionalLayer(1, 1);
            addObject.Set(new DirectionalNode( 1, 1, 1, 1, 1,1 ));
            
            //Create long horizontal line
            mapLayer.AddDirectionalLayerAtPoint(addObject, 0, 0);
            mapLayer.AddDirectionalLayerAtPoint(addObject, 2, 0);
            mapLayer.AddDirectionalLayerAtPoint(addObject,  -2, 0);
            mapLayer.AddDirectionalLayerAtPoint(addObject,  4, 0);
            mapLayer.AddDirectionalLayerAtPoint(addObject, -4, 0);
            mapLayer.AddDirectionalLayerAtPoint(addObject,  6, 0);
            mapLayer.AddDirectionalLayerAtPoint(addObject, -6, 0);
            //Create vertical lines down
            mapLayer.AddDirectionalLayerAtPoint(addObject,  6, -3);
            mapLayer.AddDirectionalLayerAtPoint(addObject, -6, -3);
            //Create 'V' towards bottom center.
            mapLayer.AddDirectionalLayerAtPoint(addObject, 4, -5);
            mapLayer.AddDirectionalLayerAtPoint(addObject, -4, -5);
            mapLayer.AddDirectionalLayerAtPoint(addObject,  2, -7);
            mapLayer.AddDirectionalLayerAtPoint(addObject, -2, -7);
            //Bottom of shape is open - the rest of the shape is enclosed.

            Pathfinder pathFinderMap = new Pathfinder(10, 0, 10, 18, mapLayer, new PathfindSettings());

            int earlyExititer = 0;
            while (pathFinderMap.Iterate() == false && earlyExititer++ < 1000)
            {
                ;
            }

            PathfinderVisualizer.SaveAsPng(pathFinderMap, outputImagePath, 6);
        }
    }
}