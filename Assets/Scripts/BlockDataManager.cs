using System.Collections.Generic;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    public static float textureOffset = 0.001f; //small offset to paint textures on mesh correctly (due to an error in storing floats)
    public static float tileSizeX, tileSizeY;
    public static Dictionary<BlockType, TextureData> blockTextureDataDictionary = new Dictionary<BlockType, TextureData>();
    public BlockDataSO textureData;

    private void Awake()
    {
        foreach (var item in textureData.textureDataList) //add blocks from BlockData scriptable object to the dictionary
        {
            if (blockTextureDataDictionary.ContainsKey(item.blockType) == false)
            {
                blockTextureDataDictionary.Add(item.blockType, item);
            }
        }
        tileSizeX = textureData.textureSizeX;
        tileSizeY = textureData.textureSizeY;
    }
}
