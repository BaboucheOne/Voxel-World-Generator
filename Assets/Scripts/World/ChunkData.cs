using System.Collections.Generic;
using UnityEngine;

public struct ChunkData
{
    public readonly Vector3[] vertices;
    public readonly Vector3[] colVertices;
    public readonly int[] triangles;
    public readonly int[] colTriangles;
    public readonly Vector2[] uvs;
    public readonly Color[] colors;
    public readonly Vector3 Position;
    public readonly Dictionary<Vector3, Block> voxelCubes;
    public readonly BiomeType biome;

    public ChunkData(Vector3[] vertices, Vector3[] colVertices, int[] triangles, int[] colTriangles, Vector2[] uvs, Color[] colors, Vector3 position, Dictionary<Vector3, Block> voxelCubes, BiomeType biome)
    {
        this.vertices = vertices;
        this.colVertices = colVertices;
        this.triangles = triangles;
        this.colTriangles = colTriangles;
        this.uvs = uvs;
        this.colors = colors;
        Position = position;
        this.voxelCubes = voxelCubes;
        this.biome = biome;
    }
}