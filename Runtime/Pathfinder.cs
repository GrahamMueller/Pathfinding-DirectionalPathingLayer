

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


    

TODO
    Test performance of open list using a linked list to increase sort performance.    
 
    Add proper cost
        Base cost : Cost of parent.base_cost + this node
        Distance cost : Cost based on distance.  
        Parent suggestion : Suggestion from parent?  
        Node cost : base + distance 
            Use this to determine pathing.
    
    Add debug tool to create texture for this, especially for use in testing.
    
    Add benchmarking
    
    
*/
using DirectionalPathingLayers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PathfindingDirectionalLayers
{

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    /// <summary> Index coordinates struct.</summary>
    public struct Vector2_int
    {
        public int x;
        public int y;
        public Vector2_int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Vector2_int left, Vector2_int right)
        {
            return (left.x == right.x) && (left.y == right.y);
        }

        public static bool operator !=(Vector2_int left, Vector2_int right)
        {
            return (left.x != right.x) || (left.y != right.y);
        }

    };

    /// <summary> Pathfinding node used to find a path from start to end.  </summary>
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
        //Completed path if it exists.
        List<PathfinderNode> completedPath;

        //Internal counter.
        int iteration_count;

        /// <summary> Configuration of pathfinder. </summary>
        public PathfindSettings Settings { get => this.settings; }
        PathfindSettings settings;

        /// <summary> Entry point pathfinding will start at. </summary>
        public Vector2_int StartPoint { get => this.startPoint; }
        Vector2_int startPoint;

        /// <summary> End point pathfinding will attempt to reach. </summary>
        public Vector2_int EndPoint { get => this.endPoint; }
        Vector2_int endPoint;

        /// <summary> Map layer pathfinding will use. </summary>
        public MapDirectionalLayer MapLayer { get => this.mapLayer; }
        MapDirectionalLayer mapLayer;

        /// <summary> Sorted list of points with [0] being lowest cost. Used to explore nodes until a path from start to end is found.</summary>
        public LinkedList<PathfinderNode> OpenList { get => this.openList; }
        LinkedList<PathfinderNode> openList;

        /// <summary> Unsorted list of points previously in 'OpenList' which can path back to start point.</summary>
        public LinkedList<PathfinderNode> ClosedList { get => this.closedList; }
        LinkedList<PathfinderNode> closedList;

        /// <summary> If the pathfinding is complete, either by a path having been found or after exhausting all options.</summary>
        public bool IsComplete { get => this.isComplete; }
        bool isComplete;

        public Pathfinder(int startX, int startY, int endX, int endY, MapDirectionalLayer mapLayer, PathfindSettings settings)
        {
            this.startPoint.x = startX;
            this.startPoint.y = startY;

            this.endPoint.x = endX;
            this.endPoint.y = endY;

            this.mapLayer = mapLayer;

            this.settings = settings;

            //--Setup initial iteration.
            this.openList = new LinkedList<PathfinderNode>();
            this.openList.AddFirst(new PathfinderNode(this.StartPoint, null, 0));

            this.closedList = new LinkedList<PathfinderNode>();

            this.iteration_count = 0;

            this.isComplete = false;
            //Early exit if either start or end point fall outside bounds.
            if (!this.mapLayer.IsIndexPointInLayer(this.startPoint.x, this.startPoint.y)) { this.isComplete = true; }
            if (!this.mapLayer.IsIndexPointInLayer(this.endPoint.x, this.endPoint.y)) { this.isComplete = true; }
        }

        /// <summary> Returns true if the pathfinder is in a completed state. </summary>
        bool EarlyExit()
        {
            if (this.isComplete) { return true; }
            if (this.openList.Count == 0) { return true; }
            if (this.completedPath != null) { return true; }
            if (this.iteration_count++ > this.Settings.EarlyExit_maxIterations) { return true; }
            return false;
        }

        /// <summary> Calculates a path from the 'bottom' node back to the starting node.</summary>
        List<PathfinderNode> PathBackwardsFromNode(PathfinderNode bottomNode)
        {
            List<PathfinderNode> backwardsPath = new List<PathfinderNode>();

            PathfinderNode pathNode = bottomNode;
            do
            {
                backwardsPath.Add(pathNode);
                pathNode = pathNode.parent;

            } while (pathNode != null && !(pathNode.pos.x == this.StartPoint.x && pathNode.pos.y == this.StartPoint.y));

            //If exited due to running out of options, that indicates a failure.
            if (pathNode == null)
            {
                backwardsPath = null;
            }
            else //Add final node, which is the start node.
            {
                backwardsPath.Add(pathNode);
            }
            return backwardsPath;
        }

        /// <summary> Calculates a path from the top node down to the 'bottomNode'.</summary>
        List<PathfinderNode> PathForwardsFromNode(PathfinderNode bottomNode)
        {
            List<PathfinderNode> forwardsPath = this.PathBackwardsFromNode(bottomNode);
            forwardsPath.Reverse();
            return forwardsPath;
        }

        /// <summary> Iterates a number of times sequentually. </summary>
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
        /// Returns next node to explore.
        /// Takes next node in 'openList', removes it, and adds it to closedList before returning that node.
        /// </summary>
        PathfinderNode GetNextBestNode()
        {
            PathfinderNode bestNode = this.openList.First.Value;
            this.openList.RemoveFirst();

            this.InsertIntoLinkedList(ref this.closedList, bestNode);
            return bestNode;
        }

        /// <summary> Gets cost of the node given. </summary>
        int GetCostOfNode(PathfinderNode node)
        {
            return this.GetDist(node, this.endPoint);
        }

        /// <summary> Adds nodes in 'neighbors' to open list if they do not already exist in open or closed lists.</summary>
        void AddNeighborsToOpen(Stack neighbors)
        {
            while (neighbors.Count > 0)
            {
                PathfinderNode neighborNode = (PathfinderNode)neighbors.Pop();

                //Only add neighbors to open if we are unaware of them.
                if (!this.PointInLinkedList(neighborNode, this.openList) && !this.PointInLinkedList(neighborNode, this.closedList))
                {
                    //Cost function
                    neighborNode.cost = this.GetCostOfNode(neighborNode);
                    this.InsertIntoLinkedList(ref this.openList, neighborNode);

                }
            }
        }

        /// <summary>
        /// Performs 1 node iteration on the pathfinding.
        /// Takes next best node and adds its neighbors to the open list.  
        /// </summary>
        /// <returns>True if unable to iterate (out of options, found exit)</returns>
        public bool Iterate()
        {
            //Return true when unable to path further.
            if (this.isComplete) { return true; }
            if (this.EarlyExit() == true) { this.isComplete = true; return true; }

            //Extract next best node.
            PathfinderNode currentNode = this.GetNextBestNode();

            //Exit found detection.
            if (currentNode.pos == this.endPoint)
            {
                this.isComplete = true;
                this.completedPath = this.PathForwardsFromNode(currentNode);
                return true;
            }

            //Get neighbors around best node.
            Stack neighborNodes = this.GetNeighbors(currentNode);

            //Add neighbors to list.
            this.AddNeighborsToOpen(neighborNodes);

            //Not at end!
            return false;
        }


        bool PointInLinkedList(PathfinderNode node, LinkedList<PathfinderNode> linkedList)
        {
            LinkedListNode<PathfinderNode> linkedNode = linkedList.First;
            if (linkedNode is null)
            {
                return false;
            }

            while (linkedNode is not null)
            {
                //If we reach a cost which is this value or higher, insert at the index instead.
                if (linkedNode.Value.pos == node.pos)
                {
                    return true;
                }
                else
                {
                    linkedNode = linkedNode.Next;
                }
            }
            return false;
        }

        void InsertIntoLinkedList(ref LinkedList<PathfinderNode> linkedList, in PathfinderNode node)
        {
            LinkedListNode<PathfinderNode> linkedNode = linkedList.First;

            //If unable to retrieve first node, we have an empty list.
            if (linkedNode is null)
            {
                linkedList.AddFirst(node);
                return;
            }
            
            //Find first node whose 'cost' value equals or exceeds the node being added. 
            while (linkedNode is not null)
            {
                //If we reach a cost which is this value or higher, insert at the index instead.
                if (linkedNode.Value.cost >= node.cost)
                {
                    linkedList.AddBefore(linkedNode, node);
                    return;
                }
                else
                {
                    linkedNode = linkedNode.Next;
                }
            }
            //Failed to find a cost greater than node.  Add node to last position.
            linkedList.AddLast(node);
        }

        Stack GetNeighbors(PathfinderNode centerNode)
        {
            DirectionalNode centerMapNode = this.mapLayer.DirectionLayer.directionalNodes[centerNode.pos.x, centerNode.pos.y];
            Stack neighborNodes = new Stack();
            //Forward
            if (centerMapNode.Forward != 0)
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
            if (centerMapNode.Backward != 0)
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
            if (centerMapNode.Right != 0)
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
            if (centerMapNode.Left != 0)
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
            if (centerMapNode.Up != 0)
            {
                //throw new Exception("Vertical movement unsupported");
            }
            if (centerMapNode.Down != 0)
            {
                //throw new Exception("Vertical movement unsupported");
            }

            return neighborNodes;
        }


        public int GetDist(PathfinderNode centerNode, Vector2_int offsetNode)
        {
            return (int)Math.Sqrt((Math.Pow((centerNode.pos.x - offsetNode.x), 2) + Math.Pow((centerNode.pos.y - offsetNode.y), 2)));
        }

        /// <summary>
        /// If a path from start to end has been found, returns that path.
        /// </summary>
        /// <returns>Array of nodes from start to end.</returns>
        public PathfinderNode[] GetCompletedPath()
        {
            if (!this.isComplete) { return null; }
            if (this.completedPath == null) { return null; }

            return this.completedPath.ToArray();
        }
    }


    /// <summary>
    /// Settings for the pathfinding instance.  
    /// </summary>
    public class PathfindSettings
    {
        //use async
        //use threading

        /// <summary> </summary>
        public PathfindSettings()
        {
            this.earlyExit_maxIterations = -1; //Set to ignore
        }


        /// <summary> If set, limits upper number of iterations pathfinding may perform until it marks itself complete.  -1 for no limit.</summary>
        public int EarlyExit_maxIterations
        {
            get
            {
                if (this.earlyExit_maxIterations < 0) { return int.MaxValue; }
                else { return this.earlyExit_maxIterations; }
            }
            set => this.earlyExit_maxIterations = value;
        }
        int earlyExit_maxIterations;

    }
}