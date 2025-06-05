using UnityEngine;

public static class BlockHelper //helps to fill in the data from ChunkData to the MeshData by creating vertices and triangles depending on block that is analysed
{
    private static Direction[] directions =
    {
        Direction.backwards,
        Direction.down,
        Direction.forward,
        Direction.left,
        Direction.right,
        Direction.up
    };

    public static MeshData GetMeshData (ChunkData chunkData, int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        if (blockType == BlockType.Air || blockType == BlockType.Nothing) //do nothing for Air and Nothing block types
            return meshData;

        foreach (Direction direction in directions)
        {
            var neighbourBlockCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
            var neighbourBlockType = Chunk.GetBlockFromChunkCoordinates(chunkData, neighbourBlockCoordinates);

            //if neighbour is not Nothing and is not Solid 
            if (neighbourBlockType != BlockType.Nothing && BlockDataManager.blockTextureDataDictionary[neighbourBlockType].isSolid == false)
            {
                if (blockType == BlockType.Water) //generate water UVs
                {
                    if (neighbourBlockType == BlockType.Air)
                        meshData.waterMesh = GetFaceDataIn(direction, chunkData, x, y, z, meshData.waterMesh, blockType);
                }
                else //generate ground UVs
                {
                    meshData = GetFaceDataIn(direction, chunkData, x, y, z, meshData, blockType);
                }
            }
        }
        return meshData;
    }

    public static MeshData GetFaceDataIn(Direction direction, ChunkData chunkData, int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        GetFaceVertices(direction, x, y, z, meshData, blockType); //add vertices to the mesh data
        meshData.AddQuadTriangles(BlockDataManager.blockTextureDataDictionary[blockType].generatesCollider); //add triangles to the mesh data
        meshData.uv.AddRange(FaceUVs(direction, blockType)); // add UVs to the mesh data

        return meshData;
    }

    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        var generatesCollider = BlockDataManager.blockTextureDataDictionary[blockType].generatesCollider;
        //order of vertices matters for the normals and how the mesh is rendered
        switch (direction) //calculations of vertices for quads based on the direction of the block
        {
            case Direction.backwards:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;
            case Direction.forward:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.left:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;

            case Direction.right:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                break;
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, BlockType blockType)
    {
        Vector2[] UVs = new Vector2[4];
        var tilePos = TexturePosition(direction, blockType);

        UVs[0] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.tileSizeX - BlockDataManager.textureOffset,
            BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.textureOffset);

        UVs[1] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.tileSizeX - BlockDataManager.textureOffset,
            BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.tileSizeY - BlockDataManager.textureOffset);

        UVs[2] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.textureOffset,
            BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.tileSizeY - BlockDataManager.textureOffset);

        UVs[3] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.textureOffset,
            BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.textureOffset);

        return UVs;
    }

    public static Vector2Int TexturePosition(Direction direction, BlockType blockType)
    {
        return direction switch //based on direction we return different Texture (up, down or side) from BlockDataSO
        {
            Direction.up => BlockDataManager.blockTextureDataDictionary[blockType].up,
            Direction.down => BlockDataManager.blockTextureDataDictionary[blockType].down,
            _ => BlockDataManager.blockTextureDataDictionary[blockType].side
        };
    }
}