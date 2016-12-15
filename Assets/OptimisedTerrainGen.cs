using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class OptimisedTerrainGen : MonoBehaviour
{
    public float Size;
    public int TriColumns;
    public Material MeshMaterial;

	// Use this for initialization
	void Start ()
	{
	    GameObject p = CreatePlane(Size,TriColumns);
	}

    private GameObject CreatePlane(float size, int triColumns)
    {

        Vector3[] vertices = PlaneGen(triColumns, size);
        vertices = DiamondSquareGen(vertices);
        int[] triangles = TriGen(vertices);
        Vector2[] uvs = UVGen(vertices);

        GameObject go = new GameObject();
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
        mf.mesh.uv = uvs;
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
                //TODO
                // Set upper left vertex
                vertices[vertIndex] = new Vector3(x * (size/triColumns),0,y * (size/triColumns));
                // Set upper right vertex
                vertices[vertIndex+1] = new Vector3((x+1) * (size/triColumns),0,(y+1) * (size/triColumns));
                // Set lower left vertex
                vertices[vertIndex+matrixDimensions] = new Vector3((x+1) * (size/triColumns),0,(y+1) * (size/triColumns));
                // Set lower right vertex

            }
        }
        return vertices;
    }

    private int[] TriGen(Vector3[] vertices)
    {
        throw new System.NotImplementedException();
    }

    private Vector2[] UVGen(Vector3[] vertices)
    {
        throw new System.NotImplementedException();
    }

    private Vector3[] DiamondSquareGen(Vector3[] vertices)
    {

    }


    
    
}
