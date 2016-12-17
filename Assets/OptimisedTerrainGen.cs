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
    public float Smoothness;

    // Use this for initialization
	void Start ()
	{
	    GameObject p = CreatePlane(Size,TriColumns);
	}

    private GameObject CreatePlane(float size, int triColumns)
    {

        Vector3[] vertices = PlaneGen(triColumns, size);
        int[] triangles = TriGen(vertices);
        vertices = DiamondSquareGen(vertices);
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
        Vector3[] plane = (Vector3[])vertices.Clone();
        int totalVertices = vertices.Length;
        int matrixDimensions = (int) Mathf.Sqrt(totalVertices);
        int triColumns = matrixDimensions / 2;
        // Calculate the number of required iterations
        int iterations = (int)Mathf.Log(triColumns, 2);

        // Create an array to keep track of set and unset vertices
        Boolean[] set = new Boolean[vertices.Length];
        for (int b = 0; b < set.Length; b++)
        {
            set[b] = false;
        }

        // Start with 4 random values at the corners
        plane[0].y = UnityEngine.Random.value * Amplitude; // Top left
        plane[matrixDimensions - 1].y = UnityEngine.Random.value * Amplitude; // Top right
        plane[(matrixDimensions - 1) * matrixDimensions].y = UnityEngine.Random.value * Amplitude; // Bottom left
        plane[(matrixDimensions * matrixDimensions) - 1].y = UnityEngine.Random.value * Amplitude; // Bottom right

        // For each iteration
        for (int i = 0; i < 2; i++)
        {
            // Set the smoothness of the current iteration
            float iterScale = Amplitude / ((i * Smoothness) + 1);

            // Size the sampling matrix
            int sampleColumns = (int)Mathf.Pow(2, i);
            int sampleDimensions = matrixDimensions / sampleColumns;
            int minSampleCorner = 0;
            int row = 0;
            // for each sub iteration (0 to 4^iteration)
            for (int j = 0; j < (int)Mathf.Pow(4, i);j++)
            {
                int maxSampleCorner = (minSampleCorner + sampleDimensions - 1) + ((sampleDimensions - 1) * matrixDimensions);

                // Sample the diamond
                float ul = plane[minSampleCorner].y;
                float ur = plane[minSampleCorner + sampleDimensions - 1].y;
                float bl = plane[minSampleCorner + ((sampleDimensions -1) * matrixDimensions)].y;
                float br = plane[minSampleCorner + sampleDimensions - 1 + ((sampleDimensions - 1) * matrixDimensions)].y;
                float avg = (ul + ur + bl + br) / 4;

                // Set the center 4 vertices (4x duplicates included)
                float randVal = avg + ((UnityEngine.Random.value - 0.5f) * iterScale);
                plane[minSampleCorner + (sampleDimensions / 2) + (((sampleDimensions / 2) - 1) * matrixDimensions) -1].y = randVal; //ul
                plane[minSampleCorner + (sampleDimensions / 2) + (((sampleDimensions / 2)-1) * matrixDimensions)].y = randVal; // ur
                plane[minSampleCorner + (sampleDimensions / 2) + ((sampleDimensions / 2) * matrixDimensions)].y = randVal; //br
                plane[minSampleCorner + (sampleDimensions/2) + ((sampleDimensions/2)*matrixDimensions) - 1].y = randVal; //bl

                // Sample the square
                // Set the 8 midpoint vertices (2x duplicates included)

                
                // Top midpoints
                randVal = UnityEngine.Random.value;
                float val = plane[((2 * minSampleCorner) + sampleDimensions) / 2].y = ((ul + ur) / 2) + ((randVal - 0.5f) * iterScale);
                plane[(((2 * minSampleCorner) + sampleDimensions) / 2) - 1].y = val;

                // Right midpoints
                randVal = UnityEngine.Random.value;
                val = plane[(minSampleCorner + sampleDimensions - 1) + (((sampleDimensions / 2) - 1) * matrixDimensions)].y = ((ur + br) / 2) + ((randVal - 0.5f) * iterScale);
                plane[(minSampleCorner + sampleDimensions - 1) + ((sampleDimensions / 2) * matrixDimensions)].y = val;

                // Bottom midpoints
                randVal = UnityEngine.Random.value;
                val = plane[maxSampleCorner - (sampleDimensions / 2)].y = ((bl + br) / 2) + ((randVal - 0.5f) * iterScale);
                plane[maxSampleCorner - (sampleDimensions / 2) + 1].y = val;

                // Left midpoints
                randVal = UnityEngine.Random.value;
                val = plane[minSampleCorner + (sampleDimensions / 2 - 1) * matrixDimensions].y = ((ul + bl) / 2) + ((randVal - 0.5f) * iterScale);
                plane[minSampleCorner + (sampleDimensions / 2) * matrixDimensions].y = val;


                // Increment the sampling matrix
                int rowMax = (row * matrixDimensions) + ((row + 1) * (matrixDimensions -1));
                if (minSampleCorner + sampleDimensions - 1 == rowMax)
                {
                    row += sampleDimensions;
                    minSampleCorner = (row * matrixDimensions);
                }
                else
                {
                    minSampleCorner += sampleDimensions;
                }
            }
        }
        

        return plane;
    }


    
    
}
