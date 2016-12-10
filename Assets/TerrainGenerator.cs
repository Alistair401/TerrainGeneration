using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {
    MeshFilter mf;
    float width = 100f;
    float height = 100f;
    int rows = 50;
    int columns = 50;

	// Use this for initialization
	void Start () {
        int adjRows = rows + 1;
        int adjColumns = columns + 1;
        mf = GetComponent<MeshFilter>();
        Vector3[] vertices = new Vector3[adjRows * adjColumns];
        int[] triangles = new int[(adjRows - 1) * (adjColumns - 1) * 6];
        int triangleIndex = 0;
        int vertexIndex = 0;
        for (int row = 0; row < adjRows; row++)
        {
            for (int column = 0; column < adjColumns; column++)
            {
                float amplitude = Mathf.PerlinNoise(10 * column / width,10 * row / height);
                amplitude = -ScaleRange(amplitude, 0, 1, 0, 20);
                vertices[(row * adjColumns) + column] = new Vector3(column * (width / columns), amplitude, row * (height / rows));
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
        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        mf.mesh.RecalculateNormals();
        mf.mesh.RecalculateBounds();
        gameObject.AddComponent<MeshCollider>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public static float ScaleRange(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        float oldVal = value / (oldMax - oldMin);
        float newVal = oldVal * (newMax - newMin);
        return newVal;
    }

}
