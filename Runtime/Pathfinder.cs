

/*
QUESTIONS
    Need of system - what do I need to support
        3D pathing
        Method to link to actions (entered doorway)
    Individual node cost?
        How to have open areas cost more
        Unless it is simply 'open area pathing layer' 


WISHLIST
    pathfinding thread?
        Keep this as a reuseable thread, where pathfinding can be safely updated outside of this.
        
    async?
        Allow pathfinding to do work while user does other things?
  
    Size?
        Probably a functino of the pathfidning supplied, not this.  IE if you want a 2x2 pathfinding operation, supply a grid where it can only move to.

    

TODO
    Create several loadable states for the game map.
        Blank.
        Line.
        Maze.
        Forest with lines.
        Boxes (allows us to fail)

    Create an agent class that knows it wants to start at A, end at B.
 
    Add cost
        Base cost : Cost of parent.base_cost + this node
        Distance cost : Cost based on distance.  
        Parent suggestion : Suggestion from parent?  
        Node cost : base + distance 
            Use this to determine pathing.
    
    Add debug tool to create texture for this
    
    Add benchmarking

    
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathfindingDirectionalLayers
{
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public struct Vector2_int
    {
        public int x;
        public int y;

        public static bool operator ==(Vector2_int left, Vector2_int right)
        {
            return (left.x == right.x) && (left.y == right.y);
        }

        public static bool operator !=(Vector2_int left, Vector2_int right)
        {
            return (left.x != right.x) && (left.y != right.y);
        }

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
        PathfindSettings settings;

        Vector2_int startPoint;

        Vector2_int endPoint;

        MapDirectionalLayer mapLayer;


        public Stack completedPath;
        List<PathfinderNode> openList;
        List<PathfinderNode> closedList;
        int iteration_count;
        bool isComplete;

        public PathfindSettings Settings { get => this.settings; }
        public Vector2_int StartPoint { get => this.startPoint; }
        public Vector2_int EndPoint { get => this.endPoint; }
        public MapDirectionalLayer MapLayer { get => this.mapLayer; }
        public List<PathfinderNode> OpenList { get => this.openList; }
        public List<PathfinderNode> ClosedList { get => this.closedList; }
        public bool IsComplete { get => this.isComplete; }

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
            this.openList.Add(new PathfinderNode(this.StartPoint, null, 0));

            this.closedList = new List<PathfinderNode>();

            this.iteration_count = 0;
            this.isComplete = false;
        }


        bool EarlyExit()
        {
            if (this.isComplete) { return true; }
            if (this.openList.Count == 0) { return true; }
            if (this.completedPath != null) { return true; }
            if (this.iteration_count++ > this.Settings.EarlyExit_maxIterations) { return true; }
            return false;
        }


        Stack PathBackwardsFromNode(PathfinderNode topNode)
        {
            Stack backwardsPath = new Stack();

            PathfinderNode pathNode = topNode;
            do
            {
                backwardsPath.Push(pathNode);
                pathNode = pathNode.parent;

            } while (pathNode != null && !(pathNode.pos.x == this.StartPoint.x && pathNode.pos.y == this.StartPoint.y));

            //If exited due to running out of options, that indicates a failure.
            if (pathNode == null)
            {
                backwardsPath = null;
            }
            return backwardsPath;
        }


        public bool Iterate(int iterateCount)
        {
            bool returnValue = false;
            for (int i = 0; i < iterateCount; ++i)
            {
                returnValue |= this.Iterate();
            }
            return returnValue;
        }

        /// <summary>
        /// Performs 1 node iteration on the pathfinding.  Exits early on multiple occasions.
        /// </summary>
        /// <returns>True if unable to iterate (out of options, found exit)</returns>
        public bool Iterate()
        {
            //Return true when unable to path further.
            if (this.EarlyExit() == true) { this.isComplete = true; return true; }

            //Extract next best node.
            PathfinderNode currentNode = this.openList[0];
            this.openList.RemoveAt(0);
            this.closedList.Add(currentNode);

            //Exit found detection.
            if (currentNode.pos == this.endPoint)
            {
                this.completedPath = this.PathBackwardsFromNode(currentNode);
                return true;
            }

            //Get neighbors around best node.
            Stack neighborNodes = this.GetNeighbors(currentNode);
            while (neighborNodes.Count > 0)
            {
                PathfinderNode neighborNode = (PathfinderNode)neighborNodes.Pop();
                //Only add neighbors to open if we are unaware of them.
                if (!this.PointInList(neighborNode, this.openList) && !this.PointInList(neighborNode, this.closedList))
                {
                    //Cost function
                    neighborNode.cost = this.GetDist(neighborNode, this.endPoint);
                    this.openList.Add(neighborNode);
                }
            }
            this.openList = this.openList.OrderBy(node => node.cost).ToList<PathfinderNode>();

            return false;
        }


        bool PointInList(PathfinderNode point, List<PathfinderNode> list)
        {
            PathfinderNode node;
            for (int i = 0; i < list.Count; ++i)
            {
                node = list[i];
                if (node.pos == point.pos) { return true; }
            }
            return false;
        }


        public Stack GetNeighbors(PathfinderNode centerNode)
        {
            DirectionalNode centerMapNode = this.mapLayer.DirectionLayer.directionalNodes[centerNode.pos.x, centerNode.pos.y];
            Stack neighborNodes = new Stack();
            //Forward
            if (centerMapNode.directions[0] != 0)
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

            //Back
            if (centerMapNode.directions[1] != 0)
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

            //right
            if (centerMapNode.directions[2] != 0)
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

            //left
            if (centerMapNode.directions[3] != 0)
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

            //Other up down later supported
            if (centerMapNode.directions[4] != 0)
            {
                throw new Exception("Vertical movement unsupported");
            }
            if (centerMapNode.directions[5] != 0)
            {
                throw new Exception("Vertical movement unsupported");
            }

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

        public PathfindSettings()
        {
            this.earlyExit_maxIterations = -1; //Set to ignore
        }

        /// <summary>
        /// If set, limits of pathfind iterations.
        /// </summary>
        public int EarlyExit_maxIterations
        {
            get
            {
                if (this.earlyExit_maxIterations < 0) { return int.MaxValue; }
                else { return this.earlyExit_maxIterations; }
            }
            set => this.earlyExit_maxIterations = value;
        }


    }
}