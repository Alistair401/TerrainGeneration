using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {
    MeshFilter mf;
    int width = 100;
    int height = 100;
    int rows = 6;
    int columns = 6;

	// Use this for initialization
	void Start () {
        mf = GetComponent<MeshFilter>();
        Vector3[] vertices = new Vector3[rows * columns];
        int[] triangles = new int[(columns - 1) * (rows - 1) * 6];
        int triangleIndex = 0;
        int vertexIndex = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                vertices[(row * columns) + column] = new Vector3(column * (width / columns), 0, row * (height / rows));
                if (row < rows - 1 && column < columns - 1)
                {
                    triangles[triangleIndex++] = vertexIndex;
                    triangles[triangleIndex++] = vertexIndex + columns + 1;
                    triangles[triangleIndex++] = vertexIndex + columns;
                    
                    triangles[triangleIndex++] = vertexIndex + columns + 1;
                    triangles[triangleIndex++] = vertexIndex;
                    triangles[triangleIndex++] = vertexIndex + 1;
                }
                vertexIndex++;
            }
        }
        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;

    }
	
	// Update is called once per frame
	void Update () {
	
	}

}
