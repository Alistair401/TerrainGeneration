using UnityEngine;
using System.Collections;

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
        q.transform.Rotate(new Vector3(180, 0, 0));
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
                    triangles[triangleIndex++] = vertexIndex + adjColumns + 1;
                    triangles[triangleIndex++] = vertexIndex + adjColumns;

                    triangles[triangleIndex++] = vertexIndex + adjColumns + 1;
                    triangles[triangleIndex++] = vertexIndex;
                    triangles[triangleIndex++] = vertexIndex + 1;
                }
                vertexIndex++;
            }
        }
        DiamondSquare(vertices);

        // Generate UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < adjRows; v++)
        {
            for (int u = 0; u < adjColumns; u++)
            {
                uvs[u + (v * adjColumns)] = new Vector2((float)u / (adjColumns - 1), (float)v / (adjRows - 1));
            }
        }

        mf.mesh.vertices = vertices;
        mf.mesh.uv = uvs;
        mf.mesh.triangles = triangles;
        mf.mesh.RecalculateNormals();
        mf.mesh.RecalculateBounds();
        mf.mesh.Optimize();
        mr.material = mat;

        go.AddComponent<MeshCollider>();
        return go;

    }

    public float ScaleRange(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        float oldVal = value / (oldMax - oldMin);
        float newVal = oldVal * (newMax - newMin);
        return newVal;
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
        float scale = amplitude * -0.3f;

        // Start with 4 random values at the corners
        plane[0].y = Random.value * scale;
        plane[(int)sideLength - 1].y = Random.value * scale;
        plane[plane.Length - (int)sideLength].y = Random.value * scale;
        plane[plane.Length - 1].y = Random.value * scale;

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
                plane[(minR + (maxR + (dist * (int)sideLength))) / 2].y = avg + ((Random.value - 0.5f) * iterScale);

                // Sample square
                // set the 4 points between the corners

                plane[(minR + maxR) / 2].y = ((a + b) / 2) + ((Random.value - 0.5f) * iterScale);
                plane[(minR + (minR + (dist * (int)sideLength))) / 2].y = ((a + c) / 2) + ((Random.value - 0.5f) * iterScale);
                plane[((minR + (dist * (int)sideLength)) + (maxR + (dist * (int)sideLength))) / 2].y = ((c + d) / 2) + ((Random.value - 0.5f) * iterScale);
                plane[((maxR + (dist * (int)sideLength)) + maxR) / 2].y = ((d + b) / 2) + ((Random.value - 0.5f) * iterScale);

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
