using System.Collections.Generic;
using UnityEngine;

public enum BiomeType { Sea = 0, Land = 1, Forest = 2, Desert = 3, Thundra = 4, Jungle = 5 }

public struct BiomeData
{
    public readonly int Octave;
    public readonly float Amplitude;
    public readonly float Lacunarity;
    public readonly float Frequency;

    public BiomeData(int octave, float amplitude, float lacunarity, float frequency)
    {
        Octave = octave;
        Amplitude = amplitude;
        Lacunarity = lacunarity;
        Frequency = frequency;
    }

    public override string ToString()
    {
        return string.Format("{0} {1} {2} {3}", Octave, Amplitude, Lacunarity, Frequency);
    }
}

public static class Biome
{
    private static Dictionary<BiomeType, BiomeData> biomesData = new Dictionary<BiomeType, BiomeData>()
    {
        { BiomeType.Sea, new BiomeData(0, 0f, 0f, 0f) },
        { BiomeType.Land, new BiomeData(4, 0.1f, 0f, 0.007f) },
        { BiomeType.Forest, new BiomeData(3, 0.2f, 2f, 0.01f) },
        { BiomeType.Desert, new BiomeData(3, 0.2f, 2f, 0.01f) },
        { BiomeType.Thundra, new BiomeData(3, 0.2f, 2f, 0.01f) },
        { BiomeType.Jungle, new BiomeData(3, 0.2f, 2f, 0.01f) }
    };

    public static BiomeData GetBiomeData(BiomeType biome)
    {
        return biomesData[biome];
    }

    public static BiomeData GetBiomeData(float x, float y)
    {
        return GetBiomeData(GetBiomeByBiomeNoise(BiomeNoise(x, y, 0.32f, 0.45f, 2)));
    }

    public static float BiomeNoise(float x, float y, float freq, float exp, int oct)
    {
        float nx = x * freq * oct;
        float nz = y * freq * oct;
        float e = 1f * Mathf.PerlinNoise(1f * nx, 1f * nz) +
                  0.5f * Mathf.PerlinNoise(2 * nx, 2 * nz) +
                  0.25f * Mathf.PerlinNoise(4 * nx, 4 * nz);


        return Mathf.Pow(e, exp);
    }

    public static BiomeType GetBiomeByBiomeNoise(float sample)
    {
        if (sample < 0.1f) return BiomeType.Sea;
        else if (sample < 0.3f) return BiomeType.Land;
        else if (sample < 0.5f) return BiomeType.Forest;
        else if (sample < 0.7f) return BiomeType.Desert;
        else if (sample < 0.9f) return BiomeType.Jungle;

        return BiomeType.Thundra;
    }
}
