using System;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public NoiseSettings biomeNoiseSettings;

    public DomainWarping domainWarping;

    public BlockLayerHandler startLayerHandler;
    public List<BlockLayerHandler> additionalLayerHandlers;

    public bool useDomainWarp = true;

    public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int mapSeedOffset)
    {
        biomeNoiseSettings.worldOffset = mapSeedOffset;
        int groundPosition = GetSurfaceHeightNoise(data.worldPosition.x + x, data.worldPosition.z + z, data.chunkHeight);

        for (int y = 0; y < data.chunkHeight; y++) //loop through y coordinates
        {
            startLayerHandler.Handle(data, x, y, z, groundPosition, mapSeedOffset);
        }
        foreach (BlockLayerHandler handler in additionalLayerHandlers)
        {
            handler.Handle(data, x, data.worldPosition.y, z, groundPosition, mapSeedOffset);
        }

        return data;
    }

    private int GetSurfaceHeightNoise(int x, int z, int chunkHeight)
    {
        float terrainHeight = 0;
        if (useDomainWarp)
        { 
            terrainHeight = domainWarping.GenerateDomainNoise(x, z, biomeNoiseSettings);
        }
        else 
        {
            terrainHeight = Noise.OctavePerlin(x, z, biomeNoiseSettings);
        }


        terrainHeight = Noise.Redistribution(terrainHeight, biomeNoiseSettings);
        int surfaceHeight = Noise.RemapValueToInt(terrainHeight, 0, chunkHeight);
        return surfaceHeight;
    }
}