using System;

namespace Assets.Tests
{
    class SimpleRun{
        static void Main(string[] args)

        {
            MapDirectionalLayer mapLayer = new MapDirectionalLayer(5, 5, new DirectionalNode(1));


            //Blocked center nodes
            DirectionalLayer horizontalBlocker = new DirectionalLayer(3, 0);
            horizontalBlocker.Set(1);
            mapLayer.AddDirectionalLayerAtPoint(horizontalBlocker, 0, 2);
            mapLayer.AddDirectionalLayerAtPoint(horizontalBlocker, -3, -3);
            mapLayer.AddDirectionalLayerAtPoint(horizontalBlocker, 3, -1);
            //mapLayer.AddDirectionalLayerAtPoint(horizontalBlocker, 0, -2);


            Vector2_int startPos = new Vector2_int() { x = mapLayer.DirectionLayer.WorldXToIndex(0), y = mapLayer.DirectionLayer.WorldYToIndex(-4) };
            Vector2_int endPos = new Vector2_int() { x = mapLayer.DirectionLayer.WorldXToIndex(0), y = mapLayer.DirectionLayer.WorldYToIndex( 4 ) };

            PathfindSettings pathfindSettings = new PathfindSettings();
            pathfindSettings.EarlyExit_maxIterations = -1;
            pathfindSettings.EarlyExit_maxRange = -1;

            Pathfinder pathfind = new Pathfinder(startPos.x, startPos.y, endPos.x, endPos.y, mapLayer, pathfindSettings);

            //Iterate.
            while (pathfind.Iterate() == false)
            {
                int a = 0;
            }
        }
    }

    //Get in unityworking, showing iteration 
        //Button
        //Use grids
        //Show path
        //show open, closed list entities
        //Show pathing (steal from possum)

}
