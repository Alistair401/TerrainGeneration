using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class OptimisedTerrainGen : MonoBehaviour
{
    public float Size;
    public int TriColumns;
    public Material MeshMaterial;
    public float Amplitude;

    // Use this for initialization
	void Start ()
	{
	    GameObject p = CreatePlane(Size,TriColumns);
	}

    private GameObject CreatePlane(float size, int triColumns)
    {

        Vector3[] vertices = PlaneGen(triColumns, size);
        int[] triangles = TriGen(vertices);
        //vertices = DiamondSquareGen(vertices);
        //Vector2[] uvs = UVGen(vertices);

        GameObject go = new GameObject();
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        //mf.mesh.uv = uvs;
        mf.mesh.RecalculateNormals();
        mf.mesh.RecalculateBounds();
        go.AddComponent<MeshCollider>();
        mr.material = MeshMaterial;
        return go;

    }

    private Vector3[] PlaneGen(int triColumns, float size)
    {
        int totalVertices = triColumns * triColumns * 4;
        int matrixDimensions = (int)Mathf.Sqrt(totalVertices);
        Vector3[] vertices = new Vector3[totalVertices];
        int vertIndex = 0;
        for (int y = 0; y < triColumns; y++)
        {
            for (int x = 0; x < triColumns; x++)
            {
                // Set upper left vertex
                vertices[vertIndex] = new Vector3(x * (size/triColumns),0,y * (size/triColumns));
                // Set upper right vertex
                vertices[vertIndex + 1] = new Vector3((x+1) * (size/triColumns),0,y * (size/triColumns));
                // Set lower left vertex
                vertices[vertIndex+matrixDimensions] = new Vector3(x * (size/triColumns),0,(y+1) * (size/triColumns));
                // Set lower right vertex
                vertices[vertIndex+matrixDimensions + 1] = new Vector3((x+1) * (size/triColumns),0,(y+1) * (size/triColumns));
                vertIndex += 2;
            }
            vertIndex += matrixDimensions;
        }
        return vertices;
    }

    private int[] TriGen(Vector3[] vertices)
    {

        int totalVertices = vertices.Length;
        int matrixDimensions = (int) Mathf.Sqrt(totalVertices);
        int triColumns = matrixDimensions / 2;
        // Includes triangles between duplicate points
        int[] triangles = new int[triColumns*triColumns*6];
        int triIndex = 0;
        int vertIndex = 0;
        for (int x = 0; x < triColumns; x++)
        {
            for (int y = 0; y < triColumns; y++)
            {
                triangles[triIndex++] = vertIndex;
                triangles[triIndex++] = vertIndex + matrixDimensions;
                triangles[triIndex++] = vertIndex + matrixDimensions + 1;

                triangles[triIndex++] = vertIndex + matrixDimensions + 1;
                triangles[triIndex++] = vertIndex + 1;
                triangles[triIndex++] = vertIndex;
                vertIndex += 2;
            }
            vertIndex += matrixDimensions;
        }
        return triangles;
    }

    private Vector2[] UVGen(Vector3[] vertices)
    {
        throw new System.NotImplementedException();
    }

    private Vector3[] DiamondSquareGen(Vector3[] vertices)
    {
        int totalVertices = vertices.Length;
        int matrixDimensions = (int) Mathf.Sqrt(totalVertices);
        int triColumns = matrixDimensions / 2;
        // Calculate the number of required iterations
        int iterations = (int)Mathf.Log(triColumns, 2);
        // Start with 4 random values at the corners
        /*vertices[0].y = UnityEngine.Random.value * scale;
        plane[(int)sideLength - 1].y = UnityEngine.Random.value * scale;
        plane[plane.Length - (int)sideLength].y = UnityEngine.Random.value * scale;
        plane[plane.Length - 1].y = UnityEngine.Random.value * scale;*/



        throw new System.NotImplementedException();
    }


    
    
}
