using System.Collections.Generic;
using UnityEngine;

public class UnityEditorTest_Simple : MonoBehaviour
{
    // Start is called before the first frame update
    public bool iterate;


    public Vector2Int mapSize = new Vector2Int(10, 10);
    public Vector2Int startPoint = new Vector2Int(10, 10);
    public Vector2Int endPoint = new Vector2Int(10, 10);



    public GameObject node_open;
    public GameObject node_closed;
    public GameObject node_path;

    public GameObject node_start;
    public GameObject node_end;

    public DirectionalLayerToTexture2D layerTo2D;

    public List<GameObject> spawned_open;
    public List<GameObject> spawned_closed;
    public List<GameObject> spawned_path;

    public Pathfinder pathFinderMap;


    void Start()
    {
        this.layerTo2D.simple = this;

        this.spawned_open = new List<GameObject>();
        this.spawned_closed = new List<GameObject>();
        this.spawned_path = new List<GameObject>();


        MapDirectionalLayer mapLayer = new MapDirectionalLayer(this.mapSize.x, this.mapSize.y, new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));

        DirectionalLayer horizontal_object = new DirectionalLayer(3, 1 );
        horizontal_object.Set(new DirectionalNode(new int[] { 1, 1, 0, 0, 0, 0 }));

        mapLayer.AddDirectionalLayerAtPoint(horizontal_object, 0, 0);
        mapLayer.AddDirectionalLayerAtPoint(horizontal_object, -2, 0);
        mapLayer.AddDirectionalLayerAtPoint(horizontal_object, -3, 0);
        mapLayer.AddDirectionalLayerAtPoint(horizontal_object, 0, 5);

        DirectionalLayer vertical_object = new DirectionalLayer(1, 5);
        vertical_object.Set(new DirectionalNode(new int[] { 0, 0, 1, 1, 0, 0 }));
        mapLayer.AddDirectionalLayerAtPoint(vertical_object, 0, 0);

        vertical_object.Set(new DirectionalNode(new int[] { 0, 0, 0, 1, 0, 0 }));
        mapLayer.AddDirectionalLayerAtPoint(vertical_object, -1, -2);




        DirectionalLayer simpleNode = new DirectionalLayer(0, 0);
        simpleNode.Set(new DirectionalNode(new int[] { 1, 1, 1, 1, 0, 0 }));
        mapLayer.AddDirectionalLayerAtPoint(simpleNode, -10, -10);
        mapLayer.AddDirectionalLayerAtPoint(simpleNode, 10, -10);
        mapLayer.AddDirectionalLayerAtPoint(simpleNode, -10, 10);
        mapLayer.AddDirectionalLayerAtPoint(simpleNode, -10, 9);

        this.pathFinderMap = new Pathfinder(this.startPoint.x + this.mapSize.x, this.startPoint.y + this.mapSize.y, this.endPoint.x + this.mapSize.x, this.endPoint.y + this.mapSize.y, mapLayer, new PathfindSettings());
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.iterate) { return; }

        this.iterate = false;
        this.pathFinderMap.Iterate();
        this.UpdateVisuals();
    }

    public Vector2 IndexToWorld(Vector2 index)
    {
        return new Vector2(this.pathFinderMap.mapLayer.DirectionLayer.IndexXToWorld((int)index.x), this.pathFinderMap.mapLayer.DirectionLayer.IndexXToWorld((int)index.y));
    }

    public Vector2 WorldToIndex(Vector2 index)
    {
        return new Vector2(this.pathFinderMap.mapLayer.DirectionLayer.IndexXToWorld((int)index.x), this.pathFinderMap.mapLayer.DirectionLayer.IndexXToWorld((int)index.y));
    }


    public void UpdateVisuals()
    {
        Vector2 tempStart = this.startPoint;// WorldToIndex(this.startPoint);
        Vector2 tempEnd = this.endPoint;// WorldToIndex(this.endPoint);
        this.node_start.transform.position = new Vector3(tempStart.x, 2, tempStart.y);
        this.node_end.transform.position = new Vector3(tempEnd.x, 2, tempEnd.y);

        if (this.spawned_open != null)
        {
            foreach (GameObject g in this.spawned_open)
            {
                Destroy(g);
            }
        }
        if (this.spawned_closed != null)
        {
            foreach (GameObject g in this.spawned_closed)
            {
                Destroy(g);
            }
        }
        if (this.spawned_path != null)
        {
            foreach (GameObject g in this.spawned_path)
            {
                Destroy(g);
            }
        }

        if (this.pathFinderMap.openList != null)
        {
            foreach (PathfinderNode node in this.pathFinderMap.openList)
            {
                GameObject g = GameObject.Instantiate(this.node_open);
                Vector2 tempPoint = IndexToWorld(new Vector2(node.pos.x, node.pos.y));
                g.transform.position = new Vector3(tempPoint.x, 0, tempPoint.y);
                this.spawned_open.Add(g);
            }
        }


        if (this.pathFinderMap.closedList != null)
        {
            foreach (PathfinderNode node in this.pathFinderMap.closedList)
            {
                GameObject g = GameObject.Instantiate(this.node_closed);
                Vector2 tempPoint = IndexToWorld(new Vector2(node.pos.x, node.pos.y));
                g.transform.position = new Vector3(tempPoint.x, 0, tempPoint.y);
                this.spawned_closed.Add(g);
            }
        }

        if (this.pathFinderMap.completedPath != null)
        {
            foreach (PathfinderNode node in this.pathFinderMap.completedPath)
            {
                GameObject g = GameObject.Instantiate(this.node_path);
                Vector2 tempPoint = IndexToWorld(new Vector2(node.pos.x, node.pos.y));
                g.transform.position = new Vector3(tempPoint.x, 0, tempPoint.y);
                this.spawned_path.Add(g);
            }
        }

        this.layerTo2D.UpdateTexture();
    }


}
