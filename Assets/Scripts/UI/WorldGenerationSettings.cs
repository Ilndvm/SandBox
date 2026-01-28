using UnityEngine;

[CreateAssetMenu(fileName = "WorldGenerationSettings", menuName = "Data/World Generation Settings")]
public class WorldGenerationSettings : ScriptableObject
{
    public int chunkDrawingRange = 6;
    public Vector2Int mapSeedOffset = Vector2Int.zero;

    public int waterLevel = 4;
    public bool useDomainWarping = true;

    public float noiseZoom = 0.01f;
    public int noiseOctaves = 5;
    public float persistance = 0.5f;
    public float redistributionModifier = 1.2f;
    public int exponent = 1;
}