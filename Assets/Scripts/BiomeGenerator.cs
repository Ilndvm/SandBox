using System;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public int waterThreshold = 50;
    public NoiseSettings biomeNoiseSettings;

    public DomainWarping domainWarping;

    public BlockLayerHandler startLayerHandler;
    public List<BlockLayerHandler> additionalLayerHandlers;

    public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int mapSeedOffset)
    {
        biomeNoiseSettings.worldOffset = mapSeedOffset;
        int groundPosition = GetSurfaceHeightNoise(data.worldPosition.x + x, data.worldPosition.z + z, data.chunkHeight);

        for (int y = 0; y < data.chunkHeight; y++) //loop through y coordinates
        {
            startLayerHandler.Handle(data, x, y, z, groundPosition, mapSeedOffset);

            //BlockType voxelType = BlockType.Dirt; //if y is smaller than groundPosition -> generate Dirt
            //if (y > groundPosition)
            //{
            //    if (y < waterThreshold) //if y is bigger than groundPosition and smaller than waterThreshold -> generate Water
            //    {
            //        voxelType = BlockType.Water;
            //    }
            //    else //if y is bigger than groundPosition and bigger than waterThreshold -> generate Air
            //    {
            //        voxelType = BlockType.Air;
            //    }

            //}
            //else if (y == groundPosition && y < waterThreshold) //if y is equal to groundPosition and below water level -> generate Sand
            //{
            //    voxelType = BlockType.Sand;
            //}
            //else if (y == groundPosition) //if y is equal to groundPosition -> generate Grass_Dirt
            //{
            //    voxelType = BlockType.Grass_Dirt;
            //}

            //Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType); //passing data, position and voxelType to the Chunk static class
        }
        foreach (BlockLayerHandler handler in additionalLayerHandlers)
        {
            handler.Handle(data, x, data.worldPosition.y, z, groundPosition, mapSeedOffset);
        }

        return data;
    }

    private int GetSurfaceHeightNoise(int x, int z, int chunkHeight)
    {
        float terrainHeight = domainWarping.GenerateDomainNoise(x, z, biomeNoiseSettings);

        //terrainHeight = Noise.OctavePerlin(x, z, biomeNoiseSettings);

        terrainHeight = Noise.Redistribution(terrainHeight, biomeNoiseSettings);
        int surfaceHeight = Noise.RemapValueToInt(terrainHeight, 0, chunkHeight);
        return surfaceHeight;
    }
}