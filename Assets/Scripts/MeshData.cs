using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();

    public List<Vector3> colliderVertices = new List<Vector3>(); //separate Lists for collider voxels e.g. to prevent water colliding
    public List<int> colliderTriangles = new List<int>();

    public MeshData waterMesh; //separate data for Water Mesh
    private bool isMainMesh = true;

    public MeshData(bool isMainMesh)
    {
        if (isMainMesh) //if it is a mainMesh we create a Water subMesh
        { 
            waterMesh = new MeshData(false);
        }
    }

    public void AddVertex(Vector3 vertex, bool vertexGeneratesCollider) //add vertex to the vertices List
    { 
        vertices.Add(vertex);
        if (vertexGeneratesCollider) //add vertex to the colliderVertices List if it should have a collider
        {
            colliderVertices.Add(vertex);
        }
    }

    public void AddQuadTriangles(bool quadGeneratesCollider) //create a quad with two triangles
    {
        triangles.Add(vertices.Count - 4); //first triangle
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4); //second triangle
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);

        if (quadGeneratesCollider) //if the quad should have collider create two colliderTriangles
        {
            colliderTriangles.Add(vertices.Count - 4); //first colliderTriangles
            colliderTriangles.Add(vertices.Count - 3);
            colliderTriangles.Add(vertices.Count - 2);

            colliderTriangles.Add(vertices.Count - 4); //second colliderTriangles
            colliderTriangles.Add(vertices.Count - 2);
            colliderTriangles.Add(vertices.Count - 1);
        }
    }
}
