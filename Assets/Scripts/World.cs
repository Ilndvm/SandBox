using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public int waterThreshold = 50;
    public float noiseScale = 0.03f;
    public GameObject chunkPrefab;

    Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>(); //data about chunks that are going to be generated
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    public void GenerateWorld() //method triggered by button
    {
        chunkDataDictionary.Clear(); //clear current world
        foreach (ChunkRenderer chunk in chunkDictionary.Values)
        {
            Destroy(chunk.gameObject); //destroy all chunks
        }
        chunkDictionary.Clear();

        for (int x = 0; x < mapSizeInChunks; x++) //generate data for each chunk
        {
            for (int z = 0; z < mapSizeInChunks; z++)
            {
                ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                GenerateVoxels(data);
                chunkDataDictionary.Add(data.worldPosition, data);
            }
        }

        foreach (ChunkData data in chunkDataDictionary.Values) //instantiate each chunk
        {
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkDictionary.Add(data.worldPosition, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);
        }
    }

    private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < data.chunkSize; x++) //loop through x coordinates
        {
            for (int z = 0; z < data.chunkSize; z++) //loop through z coordinates
            {
                float noiseValue = Mathf.PerlinNoise((data.worldPosition.x + x) * noiseScale, (data.worldPosition.z + z) * noiseScale); //simple Perlin noise for setting ground level
                int groundPosition = Mathf.RoundToInt(noiseValue * chunkHeight); 

                for (int y = 0; y < chunkHeight; y++) //loop through y coordinates
                {
                    BlockType voxelType = BlockType.Dirt; //if y is smaller than groundPosition -> generate Dirt
                    if (y > groundPosition)
                    {
                        if (y < waterThreshold) //if y is bigger than groundPosition and smaller than waterThreshold -> generate Water
                        {
                            voxelType = BlockType.Water;
                        }
                        else //if y is bigger than groundPosition and bigger than waterThreshold -> generate Air
                        {
                            voxelType = BlockType.Air;
                        }

                    }
                    else if (y == groundPosition) //if y is equal to groundPosition -> generate Grass_Dirt
                    {
                        voxelType = BlockType.Grass_Dirt;
                    }

                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType); //passing data, position and voxelType to the Chunk static class
                }
            }
        }
    }

    public BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoordinates(this, x, y, z);

        ChunkData containerChunk = null;
        chunkDataDictionary.TryGetValue(pos, out containerChunk); //get chunk from chunkDataDictionary

        if (containerChunk == null) //return Nothing if there is no chunk in the position 
            return BlockType.Nothing;

        //return block from neighbour chunk 
        Vector3Int blockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInCHunkCoordinates);
    }
}
