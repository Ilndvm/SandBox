using System;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public BiomeGenerator biomeGenerator;
    public ChunkData GenerateChunkData(ChunkData data, Vector2Int mapSeedOffset)
    {
        for (int x = 0; x < data.chunkSize; x++) //loop through x coordinates
        {
            for (int z = 0; z < data.chunkSize; z++) //loop through z coordinates
            {
                data = biomeGenerator.ProcessChunkColumn(data, x, z, mapSeedOffset);
            }
        }
        return data;
    }
}