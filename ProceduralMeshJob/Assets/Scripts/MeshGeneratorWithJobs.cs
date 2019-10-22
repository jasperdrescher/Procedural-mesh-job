using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class MeshGeneratorWithJobs : MonoBehaviour
{
    struct MeshGeneratorJob : IJob
    {
        [ReadOnly]
        public int myXLength;

        [ReadOnly]
        public int myZLength;

        public NativeArray<Vector3> myVertices;
        public NativeArray<int> myTriangles;

        public void Execute()
        {
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
    }

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
    }

    private void GenerateMesh()
    {
        myVertices = new Vector3[(myXLength + 1) * (myZLength + 1)];
        NativeArray<Vector3> vertices = new NativeArray<Vector3>((myXLength + 1) * (myZLength + 1), Allocator.Persistent);

        myTriangles = new int[myXLength * myZLength * 6];
        NativeArray<int> triangles = new NativeArray<int>(myXLength * myZLength * 6, Allocator.Persistent);

        var job = new MeshGeneratorJob()
        {
            myVertices = vertices,
            myTriangles = triangles,
            myXLength = myXLength,
            myZLength = myZLength
        };

        JobHandle jobHandle = job.Schedule();

        jobHandle.Complete();

        job.myVertices.CopyTo(myVertices);
        job.myTriangles.CopyTo(myTriangles);

        vertices.Dispose();
        triangles.Dispose();

        SetMesh();
    }

    private void SetMesh()
    {
        myMesh.vertices = myVertices;
        myMesh.triangles = myTriangles;
        myMesh.RecalculateBounds();
        myMeshCollider.sharedMesh = myMesh;
    }
}
