using System;
using UnityEngine;

public static class Chunk
{
    public static void LoopThroughTheBlock(ChunkData chunkData, Action<int, int, int> actionToPerform)
    {
        for (int index = 0; index < chunkData.blocks.Length; index++)
        {
            var position = GetPositionFromIndex(chunkData, index);
            actionToPerform(position.x, position.y, position.z);
        }
    }

    private static Vector3Int GetPositionFromIndex(ChunkData chunkData, int index) // convert 1d index into 3d index
    {
        int x = index % chunkData.chunkSize;
        int y = (index / chunkData.chunkSize) % chunkData.chunkHeight;
        int z = index / (chunkData.chunkSize * chunkData.chunkHeight);
        return new Vector3Int(x, y, z);
    }
    private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z) // convert 3d index into 1d index
    {
        return x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkHeight * z;
    }

    private static bool inRange(ChunkData chunkData, int axisCoordinate) //check if in the range in the chunk coordinate system
    {
        if (axisCoordinate < 0 || axisCoordinate >= chunkData.chunkSize)
            return false;

        return true;
    }

    private static bool inRangeHeight(ChunkData chunkData, int yCoordinate) //check if in the range in the chunk coordinate system
    {
        if (yCoordinate < 0 || yCoordinate >= chunkData.chunkHeight)
            return false;

        return true;
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates)
    {
        return GetBlockFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        if (inRange(chunkData, x) && inRangeHeight(chunkData, y) && inRange(chunkData, z))
        {
            int index = GetIndexFromPosition(chunkData, x, y, z);  
            return chunkData.blocks[index];
        }

        //get block from neighbour chunk if not in the range of current chunk
        return chunkData.worldReference.GetBlockFromChunkCoordinates(chunkData, chunkData.worldPosition.x + x, chunkData.worldPosition.y + y, chunkData.worldPosition.z + z);
    }

    public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, BlockType block)
    {
        if (inRange(chunkData, localPosition.x) && inRangeHeight(chunkData, localPosition.y) && inRange(chunkData, localPosition.z))
        {
            int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
            chunkData.blocks[index] = block;
        }
        else
        {
            throw new Exception("Need to ask World for appropriate chunk");
        }
    }

    public static Vector3Int GetBlockInChunkCoordinates(ChunkData chunkData, Vector3Int position) //convert world position into chunk coordinates
    {
        return new Vector3Int
        {
            x = position.x - chunkData.worldPosition.x,
            y = position.y - chunkData.worldPosition.y,
            z = position.z - chunkData.worldPosition.z
        };
    }

    public static MeshData GetChunkMeshData(ChunkData chunkData) //loop through each block, use GetMeshData from BlockHeleper script to get data for each block
    {
        MeshData meshData = new MeshData(true);

        LoopThroughTheBlock(chunkData, (x, y, z) => 
            meshData = BlockHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.blocks[GetIndexFromPosition(chunkData, x, y, z)]));

        return meshData;
    }

    public static Vector3Int ChunkPositionFromBlockCoordinates(World world, int x, int y, int z) 
    {
        Vector3Int pos = new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(z / (float)world.chunkSize) * world.chunkSize,
        };
        return pos; //return position of chunk
    }
}