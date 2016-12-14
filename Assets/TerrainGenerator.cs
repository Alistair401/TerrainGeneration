using UnityEngine;
using System.Collections;
using System;

public class TerrainGenerator : MonoBehaviour {
    public Material mat;
    public int dimensions;
    public float meshWidth;
    public float meshHeight;
    public float amplitude;
    public float smoothness;


    // Use this for initialization
    void Start() {
        GameObject q = CreatePlane(meshWidth, meshHeight, dimensions, dimensions);
    }

    // Update is called once per frame
    void Update() {

    }

    public GameObject CreatePlane(float width, float height, int rows, int columns)
    {
        GameObject go = new GameObject();
        go.AddComponent<MeshFilter>();
        MeshFilter mf = go.GetComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();


        int adjRows = rows + 1;
        int adjColumns = columns + 1;
        Vector3[] vertices = new Vector3[adjRows * adjColumns];
        int[] triangles = new int[(adjRows - 1) * (adjColumns - 1) * 6];
        int triangleIndex = 0;
        int vertexIndex = 0;
        for (int row = 0; row < adjRows; row++)
        {
            for (int column = 0; column < adjColumns; column++)
            {
                vertices[(row * adjColumns) + column] = new Vector3(column * ((width) / columns), 0, row * ((height) / rows));
                if (row < adjRows - 1 && column < adjColumns - 1)
                {
                    triangles[triangleIndex++] = vertexIndex;
                    triangles[triangleIndex++] = vertexIndex + adjColumns;
                    triangles[triangleIndex++] = vertexIndex + adjColumns + 1;


                    triangles[triangleIndex++] = vertexIndex;
                    triangles[triangleIndex++] = vertexIndex + adjColumns + 1;
                    triangles[triangleIndex++] = vertexIndex + 1;
                }
                vertexIndex++;
            }
        }
        DiamondSquare(vertices);

        Vector2[] uvs = GenerateUV(vertices);

        Texture2D tex = GenerateTex(vertices, 1);

        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        mf.mesh.uv = uvs;
        mf.mesh.RecalculateNormals();
        mf.mesh.RecalculateBounds();
        mat.mainTexture = tex;
        mr.material = mat;
        go.AddComponent<MeshCollider>();
        return go;

    }

    private Texture2D GenerateTex(Vector3[] vertices,int resolution)
    {
        int sideLength = (int)Mathf.Sqrt(vertices.Length);
        Texture2D t = new Texture2D((sideLength - 1) * resolution, (sideLength - 1) * resolution);
        for (int y = 0; y < sideLength-1; y++)
        {
            for (int x = 0; x < sideLength-1; x++)
            {
                Color c = new Color(UnityEngine.Random.Range(0f,1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
                t.SetPixel(x, y, c);
            }
        }
        t.filterMode = FilterMode.Point;
        t.Apply();
        return t;
    }

    public float ScaleRange(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        float oldVal = value / (oldMax - oldMin);
        float newVal = oldVal * (newMax - newMin);
        return newVal;
    }

    public Vector2[] GenerateUV(Vector3[] plane)
    {
        Vector2[] uvs = new Vector2[plane.Length];
        int sideLength = (int)Mathf.Sqrt(plane.Length);
        for (int v = 0; v < sideLength; v++)
        {
            for (int u = 0; u < sideLength; u++)
            {
                uvs[u + (v * sideLength)] = new Vector2((float)u / (sideLength - 1), (float)v / (sideLength - 1));
            }

        }
        return uvs;
    }

    public void DiamondSquare(Vector3[] plane)
    {
        float sideLength = Mathf.Sqrt(plane.Length);
        if (sideLength % 1 != 0)
        {
            Debug.Log("dimensions should be 2,4,8,16...");
            return;
        }
        float iterations = Mathf.Log(sideLength - 1, 2);
        if (iterations % 1 != 0)
        {
            Debug.Log("dimensions should be 2,4,8,16...");
            return;
        }

        // Set the scale of the randomly generated offsets
        float scale = amplitude;

        // Start with 4 random values at the corners
        plane[0].y = UnityEngine.Random.value * scale;
        plane[(int)sideLength - 1].y = UnityEngine.Random.value * scale;
        plane[plane.Length - (int)sideLength].y = UnityEngine.Random.value * scale;
        plane[plane.Length - 1].y = UnityEngine.Random.value * scale;

        for (int i = 0; i < iterations; i++)
        {
            // size the sampled matrix
            int samplingColumns = (int)Mathf.Pow(2, i);
            int minR = 0;
            int maxR = (((int)sideLength - 1) / samplingColumns);
            // distance between sampling corners
            int dist = maxR;
            float iterScale = scale / ((i * smoothness) + 1);
            for (int j = 0; j < Mathf.Pow(4, i); j++)
            {
                // Sample diamond
                // get the 4 corners
                float a = plane[minR].y;
                float b = plane[maxR].y;
                float c = plane[minR + (dist * (int)sideLength)].y;
                float d = plane[maxR + (dist * (int)sideLength)].y;
                // get the average
                float avg = (a + b + c + d) / 4;
                // set the center
                plane[(minR + (maxR + (dist * (int)sideLength))) / 2].y = avg + ((UnityEngine.Random.value - 0.5f) * iterScale);

                // Sample square
                // set the 4 points between the corners

                plane[(minR + maxR) / 2].y = ((a + b) / 2) + ((UnityEngine.Random.value - 0.5f) * iterScale);
                plane[(minR + (minR + (dist * (int)sideLength))) / 2].y = ((a + c) / 2) + ((UnityEngine.Random.value - 0.5f) * iterScale);
                plane[((minR + (dist * (int)sideLength)) + (maxR + (dist * (int)sideLength))) / 2].y = ((c + d) / 2) + ((UnityEngine.Random.value - 0.5f) * iterScale);
                plane[((maxR + (dist * (int)sideLength)) + maxR) / 2].y = ((d + b) / 2) + ((UnityEngine.Random.value - 0.5f) * iterScale);

                // increment minR and maxR
                if (maxR % sideLength == sideLength - 1)
                {
                    minR = (((j + 1) / (int)Mathf.Pow(2, i))) * (dist * (int)sideLength);
                    maxR = minR + dist;
                }
                else
                {
                    minR = maxR;
                    maxR += dist;
                }
            }
        }
    }
    


}
