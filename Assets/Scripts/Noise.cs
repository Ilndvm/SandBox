using UnityEngine;

public static class Noise
{
    // remap value from one range (initialMin - initialMax) to another (outputMin - outputMax)
    public static float RemapValue(float value, float initialMin, float initialMax, float outputMin, float outputMax)
    { 
        return outputMin + (value - initialMin) * (outputMax - outputMin) / (initialMax - initialMin);
    }

    public static float RemapValue(float value, float outputMin, float outputMax)
    {
        return outputMin + (value - 0) * (outputMax - outputMin) / (1 - 0);
    }

    public static int RemapValueToInt(float value, float outputMin, float outputMax)
    {
        return (int)RemapValue(value, outputMin, outputMax);
    }

    public static float Redistribution(float noise, NoiseSettings settings) // easeIn using Pow function
    {
        return Mathf.Pow(noise * settings.redistributionModifier, settings.exponent);
    }

    public static float OctavePerlin(float x, float z, NoiseSettings settings)
    {
        x *= settings.noiseZoom;
        z *= settings.noiseZoom;
        x += settings.noiseZoom;
        z += settings.noiseZoom;

        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float amplitudeSum = 0; // used for normalizing result to 0.0 - 1.0 range
        for (int i = 0; i < settings.octaves; i++)
        {
            total += Mathf.PerlinNoise((settings.offset.x + settings.worldOffset.x + x) * frequency,
                (settings.offset.y + settings.worldOffset.y + z) * frequency) * amplitude;

            amplitudeSum += amplitude;
            amplitude *= settings.persistance;
            frequency *= 2;
        }
        return total / amplitudeSum;
    }
}