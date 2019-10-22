using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneratorWithPerlin : MonoBehaviour
{
    [SerializeField] private int myXLength;
    [SerializeField] private int myZLength;

    private Mesh myMesh;
    private Vector3[] myVertices;
    private int[] myTriangles;
    private MeshFilter myMeshFilter;
    private MeshCollider myMeshCollider;

    private void Awake()
    {
        myMeshFilter = GetComponent<MeshFilter>();
        myMeshCollider = gameObject.AddComponent<MeshCollider>();
        myMeshCollider.sharedMesh = null;

        myMesh = new Mesh();
        myMeshFilter.mesh = myMesh;
    }

    private void Start()
    {
        GenerateMesh();
        SetMesh();
    }

    private void GenerateMesh()
    {
        myVertices = new Vector3[(myXLength + 1) * (myZLength + 1)];

        for (int zLength = 0, index = 0; zLength <= myZLength; zLength++)
        {
            for (int xLength = 0; xLength <= myZLength; xLength++)
            {
                float y = Mathf.PerlinNoise(xLength * 0.3f, zLength * 0.3f) * 2.0f;
                myVertices[index] = new Vector3(xLength, y, zLength);
                index++;
            }
        }

        int vertex = 0;
        int triangles = 0;
        myTriangles = new int[myXLength * myZLength * 6];

        for (int zLength = 0; zLength < myZLength; zLength++)
        {
            for (int xLength = 0; xLength < myXLength; xLength++)
            {
                myTriangles[triangles + 0] = vertex + 0;
                myTriangles[triangles + 1] = vertex + myXLength + 1;
                myTriangles[triangles + 2] = vertex + 1;
                myTriangles[triangles + 3] = vertex + 1;
                myTriangles[triangles + 4] = vertex + myXLength + 1;
                myTriangles[triangles + 5] = vertex + myXLength + 2;

                vertex++;
                triangles += 6;
            }

            vertex++;
        }
    }

    private void SetMesh()
    {
        myMesh.vertices = myVertices;
        myMesh.triangles = myTriangles;
        myMesh.RecalculateBounds();
        myMeshCollider.sharedMesh = myMesh;
    }
}
