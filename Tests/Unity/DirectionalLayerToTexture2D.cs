using UnityEngine;

public class DirectionalLayerToTexture2D : MonoBehaviour

{
    public UnityEditorTest_Simple simple;


    public Color mulitplyColor = Color.white;

    public int updateRate = 1;
    int updateCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        this.updateCounter = 0;
    }

    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    //updateCounter++;
    //    if (this.updateCounter++ < this.updateRate) { return; }
    //    else { this.updateCounter = 0; }

    //    if (this.simple == null) { this.SetClear(); return; }
    //    if (this.mulitplyColor == null) { this.SetClear(); return; }




    //    MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
    //    if (meshRenderer == null) { this.SetClear(); return; }

    //    Material meshMat = meshRenderer.material;
    //    if (meshMat == null) { this.SetClear(); return; }


    //    //Texture2D pathingTexture = GameSpace.gs.PathingLayers.MapDirectionalLayers[this.layerName].Visualize();
    //    Texture2D pathingTexture = this.GetVisualizedLayer(simple.pathFinderMap.mapLayer);
    //    pathingTexture.filterMode = FilterMode.Point;
    //    pathingTexture.hideFlags = HideFlags.HideAndDontSave;
    //    pathingTexture.Apply();

    //    Destroy(meshMat.mainTexture);
    //    meshMat.mainTexture = pathingTexture;
    //    meshMat.color = this.mulitplyColor;

    //}

    public void UpdateTexture()
    {
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        if (meshRenderer == null) { this.SetClear(); return; }

        Material meshMat = meshRenderer.material;
        if (meshMat == null) { this.SetClear(); return; }

        this.transform.localScale = new Vector3((this.simple.pathFinderMap.mapLayer.DirectionLayer.GetSideWidth() + 0.5f) * 0.2f, 1.0f, (this.simple.pathFinderMap.mapLayer.DirectionLayer.GetSideLength() + 0.5f) * 0.2f);

        //Texture2D pathingTexture = GameSpace.gs.PathingLayers.MapDirectionalLayers[this.layerName].Visualize();
        Texture2D pathingTexture = this.GetVisualizedLayer(this.simple.pathFinderMap.mapLayer);
        pathingTexture.filterMode = FilterMode.Point;
        pathingTexture.hideFlags = HideFlags.HideAndDontSave;
        pathingTexture.Apply();

        Destroy(meshMat.mainTexture);
        meshMat.mainTexture = pathingTexture;
        meshMat.color = this.mulitplyColor;
    }
    void SetClear()
    {
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        if (meshRenderer == null) { return; }

        Material meshMat = meshRenderer.material;
        if (meshMat == null) { return; }

        meshMat.color = Color.clear;
    }


    Texture2D GetVisualizedLayer(MapDirectionalLayer layer)
    {
        int nodeSize = 7;
        Texture2D tex = new Texture2D(layer.DirectionLayer.directionalNodes.GetLength(0) * nodeSize, layer.DirectionLayer.directionalNodes.GetLength(1) * nodeSize);
        tex.filterMode = FilterMode.Point;

        for (int x = 0; x < layer.DirectionLayer.directionalNodes.GetLength(0); ++x)
        {
            for (int y = 0; y < layer.DirectionLayer.directionalNodes.GetLength(1); ++y)
            {
                Color[,] colors = this.GetNodeTexture(layer.DirectionLayer.directionalNodes[x, y], nodeSize, nodeSize);
                for (int x_p = 0; x_p < colors.GetLength(0); ++x_p)
                {
                    for (int y_p = 0; y_p < colors.GetLength(1); ++y_p)
                    {
                        tex.SetPixel(x * nodeSize + x_p, y * nodeSize + y_p, colors[x_p, y_p]);
                        //tex[x * nodeSize + x_p, y * nodeSize + y_p] = colors[x_p, y_p];
                    }
                }



            }
        }
        tex.Apply();


        return tex;
    }

    Color[,] GetNodeTexture(DirectionalNode node, int width, int height)
    {
        Color[,] colors = new Color[width, height];
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                //if (colors[x, y] == null)
                //{
                colors[x, y] = new Color(1, 1, 1);
                //}
            }
        }

        int centerX = width / 2;
        int centerY = height / 2;
        if (node.directions[0] != 0)
        {
            for (int y = centerY; y < height; ++y)
            {

                colors[centerX, y] = new Color(1, 0, 0);
            }
        }

        if (node.directions[1] != 0)
        {
            for (int y = 0; y <= centerY; ++y)
            {
                colors[centerX, y] = new Color(1, 0, 0);
            }
        }

        if (node.directions[2] != 0)
        {
            for (int x = centerX; x < width; ++x)
            {
                colors[x, centerY] = new Color(0, 1, 0);
            }
        }

        if (node.directions[3] != 0)
        {
            for (int x = 0; x <= centerX; ++x)
            {
                colors[x, centerY] = new Color(0, 1, 0);
            }
        }

        if (node.directions[4] != 0)
        {
            for (int x = 0; x < centerX; ++x)
            {
                for (int y = height - 1; y >= centerY; --y)
                {
                    colors[x, y] = new Color(0, 0, 1);
                }
            }
        }

        if (node.directions[5] != 0)
        {
            for (int x = centerX; x < width; ++x)
            {
                for (int y = centerY; y >= 0; --y)
                {
                    colors[x, y] = new Color(0, 0, 1);
                }
            }
        }


        return colors;
    }
}
