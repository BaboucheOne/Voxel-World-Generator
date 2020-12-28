using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Dictionary<Vector3, Block> Cubes;
    public GameObject MeshObject;
    public readonly Vector3 Position;
    public BiomeType biome;

    public Chunk(GameObject go)
    {
        MeshObject = go;
        Position = go.transform.position;
    }

    public Chunk(GameObject go, Vector3 pos)
    {
        MeshObject = go;
        Position = pos;
    }

    public Chunk(GameObject go, Dictionary<Vector3, Block> cubes)
    {
        Cubes = cubes;
        MeshObject = go;
        Position = go.transform.position;
    }
}