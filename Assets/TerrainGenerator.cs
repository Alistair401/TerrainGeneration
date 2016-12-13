using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {
    public Material mat;
    public int dimensions;
    public float meshWidth;
    public float meshHeight;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 1; i++)
        {
            for (int j = 0; j < 1; j++)
            {
                GameObject q = CreatePlane(meshWidth, meshHeight, dimensions, dimensions, i * dimensions, j*dimensions);
                q.transform.Rotate(new Vector3(180, 0, 0));
                q.transform.Translate(new Vector3(i * meshWidth, 0, j * meshHeight));
            }
        }
    }

    // Update is called once per frame
    void Update () {
	
	}

    public GameObject CreatePlane(float width, float height, int rows, int columns, float offsetX, float offsetY)
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
        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        mf.mesh.RecalculateNormals();
        mf.mesh.RecalculateBounds();
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

    public Vector3[] DiamondSquare(Vector3[] plane)
    {
        float scale = -1000;
        float sideLength = Mathf.Sqrt(plane.Length);
        if (sideLength % 1 != 0)
        {
            Debug.Log("dimensions should be 2,4,8,16...");
            return null;
        }
        float iterations = Mathf.Log(sideLength-1, 2);
        if (iterations % 1 != 0)
        {
            Debug.Log("dimensions should be 2,4,8,16...");
            return null;
        }


        // Start with 4 random values at the corners
        plane[0].y = Random.value * scale;
        plane[(int)sideLength - 1].y = Random.value * scale;
        plane[plane.Length - (int)sideLength].y = Random.value * scale;
        plane[plane.Length - 1].y = Random.value * scale;


        // start with the full matrix
        int minR = 0;
        int maxR = ((int)sideLength - 1);
        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < Mathf.Pow(4,i); j++)
            {
                // Sample diamond
                // get the 4 corners
                float a = plane[minR].y;
                float b = plane[maxR].y;
                float c = plane[maxR*(int)sideLength].y;
                float d = plane[(maxR * (int)sideLength) + maxR].y;
                // get the average
                float avg = (a + b + c + d) / 4;
                // set the center
                plane[((maxR * (int)sideLength) + maxR)/2].y = avg + ((Random.value - 0.5f) * scale);

                // Sample square
                // set the 4 points between the corners
                plane[maxR / 2].y = ((a + b) / 2) + ((Random.value - 0.5f) * (scale * 0.5f));
                plane[(maxR * (int)sideLength) / 2].y = ((a + c) / 2) + ((Random.value - 0.5f) * (scale * 0.5f));
                plane[((maxR * (int)sideLength) + ((maxR * (int)sideLength) + maxR)) / 2].y = ((c + d) / 2) + ((Random.value - 0.5f) * (scale * 0.5f));
                plane[(maxR + ((maxR * (int)sideLength) + maxR)) / 2].y = ((d + b) / 2) + ((Random.value - 0.5f) * (scale * 0.5f));

                // increment minR and maxR
            }
            // reset minR and maxR
        }
        return plane;
    }

}
