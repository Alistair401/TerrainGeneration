using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {
    public Material mat;
    public int vertexRows;
    public int vertexColumns;
    public float meshWidth;
    public float meshHeight;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject q = CreatePlane(meshWidth, meshHeight, vertexRows, vertexColumns, i * vertexRows, j*vertexColumns);
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
                float amplitude = Mathf.PerlinNoise((2f * (column+offsetX)) / width, (2f * (row + offsetY)) / height);
                amplitude = -ScaleRange(amplitude, 0, 1, 0, 100);
                vertices[(row * adjColumns) + column] = new Vector3(column * ((width) / columns), amplitude, row * ((height) / rows));
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
        mr.material = mat;

        go.AddComponent<MeshCollider>();
        return go;

    }

    public static float ScaleRange(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        float oldVal = value / (oldMax - oldMin);
        float newVal = oldVal * (newMax - newMin);
        return newVal;
    }

}
