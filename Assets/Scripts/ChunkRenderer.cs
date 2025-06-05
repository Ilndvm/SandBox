using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))] //takes custom mesh
[RequireComponent(typeof(MeshRenderer))] //takes geometry from MeshFilter and renders it at position defined by the GameObject's Transform 
[RequireComponent(typeof(MeshCollider))] //creates collider 
public class ChunkRenderer : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh; //custom mesh
    public bool showGizmo = false; //Gizmo of a full size of the chunk

    public ChunkData ChunkData { get; private set; } //data about each voxel (block) of the chunk

    public bool ModifiedByThePlayer
    {
        get
        {
            return ChunkData.modifiedByPlayer;
        }
        set
        {
            ChunkData.modifiedByPlayer = value;
        }
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;
    }

    public void InitializeChunk(ChunkData data)
    {
        this.ChunkData = data;
    }

    private void RenderMesh(MeshData meshData) //method for rendering the chunk
    {
        mesh.Clear();

        mesh.subMeshCount = 2; //subMeshes are corresponding to a Material - it is beneficial for creating transparent Water material and defaault Ground material
        mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray(); //add water and ground vertices

        mesh.SetTriangles(meshData.triangles.ToArray(), 0); //separately set triangles for ground mesh
        mesh.SetTriangles(meshData.waterMesh.triangles.Select(val => val + meshData.vertices.Count).ToArray(), 1); //separately set triangles for water mesh

        mesh.uv = meshData.uv.Concat(meshData.waterMesh.uv).ToArray(); //add water and ground uv
        mesh.RecalculateNormals(); //for correct lightning

        meshCollider.sharedMesh = null; //clear shared collider
        Mesh collisionMesh = new Mesh(); //create a new mesh with correct vertices and triangles (only ground)
        collisionMesh.vertices = meshData.colliderVertices.ToArray();
        collisionMesh.triangles = meshData.colliderTriangles.ToArray();
        collisionMesh.RecalculateNormals();

        meshCollider.sharedMesh = collisionMesh; //set brand new mesh to be a collider
    }

    public void UpdateChunk()
    {
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
    }

    public void UpdateChunk(MeshData data)
    {
        RenderMesh(data);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() //draw Gizmo of a full size of the chunk (in the unity editor only)
    {
        if (showGizmo)
        {
            if (Application.isPlaying && ChunkData != null)
            {
                if (Selection.activeObject == gameObject)
                    Gizmos.color = new Color(0, 1, 0, 0.4f);
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f);

                Gizmos.DrawCube(transform.position + new Vector3(ChunkData.chunkSize / 2f, ChunkData.chunkHeight / 2f, ChunkData.chunkSize / 2f), new Vector3(ChunkData.chunkSize, ChunkData.chunkHeight, ChunkData.chunkSize));
            }
        }
    }
#endif
}
