

/*
QUESTIONS
    Need of system - what do I need to support
        3D pathing
        Method to link to actions (entered doorway)
    Individual node cost?
        How to have open areas cost more
        Unless it is simply 'open area pathing layer' 


ROUGH DESIGN
    Pointed to list of [DirectionalNode, DirectionalNode]

WISHLIST
    pathfinding thread?
        Keep this as a reuseable thread, where pathfinding can be safely updated outside of this.
        
    async?
        Allow pathfinding to do work while user does other things?
  
    Size?
        Probably a functino of the pathfidning supplied, not this.  IE if you want a 2x2 pathfinding operation, supply a grid where it can only move to.

    Area scope, max cost, max iterations
        Adjustable settings to limit pathfinding.  
    


TODO
    Create several loadable states for the game map.
        Blank.
        Line.
        Maze.
        Forest with lines.
        Boxes (allows us to fail)

    Create an agent class that knows it wants to start at A, end at B.




Agent says I want to pathfind from A to B.
    Creates pathinding object.
    Tells this object "You start here and want to end here.  Here is pathing map.  Do N iterations and report back to me."
        Also pass in config of things like "start,end", "threaded" ect
    Check if pathfinding object is complete or has a failure.
        Complete : has path
        Failure : Why it failed
    
    
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public struct Vector2_int
{
    public int x;
    public int y;
};

public class PathfinderNode
{
    public PathfinderNode parent;
    public Vector2_int pos;
    public int cost;

    public PathfinderNode(Vector2_int pos, PathfinderNode parent, int cost)
    {
        this.parent = parent;
        this.pos = pos;
        this.cost = cost;
    }
}
public class Pathfinder
{
    public PathfindSettings settings;

    //Request?  A to B?

    Vector2_int startPoint;

    Vector2_int endPoint;

    MapDirectionalLayer mapLayer;

    Stack completedPath;
    List<PathfinderNode> openList;
    List<PathfinderNode> closedList;

    public Pathfinder(int startX, int startY, int endX, int endY, MapDirectionalLayer mapLayer, PathfindSettings settings)
    {
        this.startPoint.x = startX;
        this.startPoint.y = startY;

        this.endPoint.x = endX;
        this.endPoint.y = endY;

        this.mapLayer = mapLayer;

        this.settings = settings;

        //--Setup initial iteration.
        this.openList = new List<PathfinderNode>();
        this.openList.Add(new PathfinderNode(this.startPoint, null, 0));

        this.closedList = new List<PathfinderNode>();

    }

    public bool Iterate()
    {
        //Return true when we have a path from start to end
        //list of seen nodes
        //method of storing paths
        //Early exits
        if (this.openList.Count == 0) { return true; }
        if (this.completedPath != null) { return true; }

        //Extract next best node.
        PathfinderNode currentNode = this.openList[0];
        this.openList.RemoveAt(0);
        this.closedList.Add(currentNode);

        if (currentNode.pos.x == this.endPoint.x && currentNode.pos.y == this.endPoint.y)
        {
            this.completedPath = new Stack();

            PathfinderNode pathNode = currentNode;
            //Console.WriteLine(pathNode.pos.x + ", " + pathNode.pos.y);
            do
            {
                this.completedPath.Push(pathNode);
                Console.Write(pathNode.pos.x + ", " + pathNode.pos.y + "\n");
                pathNode = pathNode.parent;

            } while (pathNode != null && !(pathNode.pos.x == this.startPoint.x && pathNode.pos.y == this.startPoint.y));


            return true;
        }

        //Get neighbors
        Stack neighborNodes = this.GetNeighbors(currentNode);
        while (neighborNodes.Count > 0)
        {
            PathfinderNode neighborNode = (PathfinderNode)neighborNodes.Pop();
            //If neighbornode not in 
            if (!this.PointInList(neighborNode.pos, this.openList) && !this.PointInList(neighborNode.pos, this.closedList))
            {
                neighborNode.cost = this.GetDist(neighborNode, this.endPoint);
                this.openList.Add(neighborNode);
            }

            //Sort list
        }
        this.openList = this.openList.OrderBy(node => node.cost).ToList<PathfinderNode>();

        return false;
    }
    bool PointInList(Vector2_int point, List<PathfinderNode> list)
    {
        foreach (PathfinderNode node in list)
        {
            if (node.pos.x == point.x && node.pos.y == point.y) { return true; }
        }
        return false;
    }
    public Stack GetNeighbors(PathfinderNode centerNode)
    {
        //Get neighbors able to be traveled to
        //Remove neighbors from open list
        //Remove neighbors from closed list
        //Return final list.  May be empty.
        DirectionalNode centerMapNode = this.mapLayer.DirectionLayer.directionalNodes[centerNode.pos.x, centerNode.pos.y];
        Stack neighborNodes = new Stack();
        //Forward
        if (centerMapNode.directions[0] != 0)
        {
            Vector2_int newPos = new Vector2_int() { x = centerNode.pos.x + 1, y = centerNode.pos.y };
            if (newPos.x >= 0 &&
                newPos.y >= 0 &&
                newPos.x < this.mapLayer.DirectionLayer.directionalNodes.GetLength(0) &&
                newPos.y < this.mapLayer.DirectionLayer.directionalNodes.GetLength(1))
            {
                neighborNodes.Push(new PathfinderNode(newPos, centerNode, 0));
            }
        }

        //Back
        if (centerMapNode.directions[1] != 0)
        {
            Vector2_int newPos = new Vector2_int() { x = centerNode.pos.x - 1, y = centerNode.pos.y };
            if (newPos.x >= 0 &&
                newPos.y >= 0 &&
                newPos.x < this.mapLayer.DirectionLayer.directionalNodes.GetLength(0) &&
                newPos.y < this.mapLayer.DirectionLayer.directionalNodes.GetLength(1))
            {
                neighborNodes.Push(new PathfinderNode(newPos, centerNode, 0));
            }
        }

        //Right
        if (centerMapNode.directions[2] != 0)
        {
            Vector2_int newPos = new Vector2_int() { x = centerNode.pos.x, y = centerNode.pos.y + 1 };
            if (newPos.x >= 0 &&
                newPos.y >= 0 &&
                newPos.x < this.mapLayer.DirectionLayer.directionalNodes.GetLength(0) &&
                newPos.y < this.mapLayer.DirectionLayer.directionalNodes.GetLength(1))
            {
                neighborNodes.Push(new PathfinderNode(newPos, centerNode, 0));
            }
        }

        //Left
        if (centerMapNode.directions[3] != 0)
        {
            Vector2_int newPos = new Vector2_int() { x = centerNode.pos.x, y = centerNode.pos.y - 1 };
            if (newPos.x >= 0 &&
                newPos.y >= 0 &&
                newPos.x < this.mapLayer.DirectionLayer.directionalNodes.GetLength(0) &&
                newPos.y < this.mapLayer.DirectionLayer.directionalNodes.GetLength(1))
            {
                neighborNodes.Push(new PathfinderNode(newPos, centerNode, 0));
            }
        }

        //Other up down later supported


        return neighborNodes;
    }

    public int GetDist(PathfinderNode centerNode, Vector2_int offsetNode)
    {
        return (int)Math.Sqrt((Math.Pow((centerNode.pos.x - offsetNode.x), 2) + Math.Pow((centerNode.pos.y - offsetNode.y), 2)));
    }


}






/// <summary>
/// Settings for the pathfinding instance.  
/// </summary>
public class PathfindSettings
{
    //use async
    //use threading


    int earlyExit_maxIterations;
    int earlyExit_maxRange;

    public PathfindSettings()
    {
    }

    //If early exit is -1, disable and use max value instead.
    public int EarlyExit_maxIterations
    {
        get
        {
            if (this.earlyExit_maxIterations < 0) { return int.MaxValue; }
            else { return this.earlyExit_maxIterations; }
        }
        set => this.earlyExit_maxIterations = value;
    }

    //If early exit is -1, disable and use max value instead.
    public int EarlyExit_maxRange
    {
        get
        {
            if (this.earlyExit_maxRange < 0) { return int.MaxValue; }
            else { return this.earlyExit_maxRange; }
        }
        set => this.earlyExit_maxRange = value;
    }
}