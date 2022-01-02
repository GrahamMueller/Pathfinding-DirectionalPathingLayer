using DirectionalPathingLayers;

namespace PathfindingDirectionalLayers.Utility
{
    public static class PathfinderVisualizer
    {
        /// <summary>
        /// Saves the pathfinding information as well as the map itself to a png file.
        /// </summary>
        public static void SaveAsPng(Pathfinder pathfinder, string savePath, int tileSize)
        {
            DirectionalNodeVisualize vis = new DirectionalNodeVisualize(pathfinder.MapLayer.DirectionLayer.directionalNodes, tileSize);

            //Open path
            foreach (PathfinderNode v in pathfinder.OpenList)
            {
                vis.SetBackgroundTileColor(v.pos.x, v.pos.y, System.Drawing.Color.LightGreen);
            }

            //Closed path
            foreach (PathfinderNode v in pathfinder.ClosedList)
            {
                vis.SetBackgroundTileColor(v.pos.x, v.pos.y, System.Drawing.Color.Pink);
            }

            //Final path
            //PathfinderNode[] completedPath = pathfinder.GetCompletedPath();
            foreach (PathfinderNode v in pathfinder.GetCompletedPath())
            {
                vis.SetBackgroundTileColor(v.pos.x, v.pos.y, System.Drawing.Color.DarkGray);
            }

            
            vis.SaveAsPng(savePath);
        }
    }
}
