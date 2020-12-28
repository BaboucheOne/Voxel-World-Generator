using System.Collections.Generic;
using UnityEngine;

public enum Orientation { Up, Down, Left, Right, Forward, Backward }

[System.Serializable]
public class VoxelGenerator
{
    private FastNoise fastNoise = new FastNoise();
    private Vector3 chunkOffest = new Vector3();

    private Vector3 chunkPosition = new Vector3();

    private Dictionary<Vector3, Block> chunkVoxels = new Dictionary<Vector3, Block>();
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<Color> colors = new List<Color>();

    private List<Vector3> colliderVertices = new List<Vector3>();
    private List<int> colliderTriangles = new List<int>();
    private BiomeType biome;

    private const int chunkSize = 16;
    private const float tileUnit = 0.0625f;

    private int faceCount = 0;
    private int colFaceCount = 0;

    public static Dictionary<Vector3, Orientation> OrientationbMask = new Dictionary<Vector3, Orientation>
    {
        { new Vector3(1, 0, 0), Orientation.Left},
        { new Vector3(-1, 0, 0), Orientation.Right},
        { new Vector3(0, 1, 0), Orientation.Up},
        { new Vector3(0, -1, 0), Orientation.Down},
        { new Vector3(0, 0, 1), Orientation.Forward},
        { new Vector3(0, 0, -1), Orientation.Backward},
    };

    public ChunkData GenerateChunk(Vector3 pos)
    {
        chunkPosition = pos;
        InitChunk();
        GenerateVertices();

        return new ChunkData(vertices.ToArray(), colliderVertices.ToArray(), triangles.ToArray(), colliderTriangles.ToArray(), uvs.ToArray(), colors.ToArray(), pos, chunkVoxels, biome);
    }
    
    public ChunkData GenerateChunkByVoxelCube(Vector3 pos, Dictionary<Vector3, Block> voxels)
    {
        chunkPosition = pos;
        chunkVoxels = voxels;

        GenerateVertices();
        
        return new ChunkData(vertices.ToArray(), colliderVertices.ToArray(), triangles.ToArray(), colliderTriangles.ToArray(), uvs.ToArray(), colors.ToArray(), pos, chunkVoxels, biome);
    }

    private void InitChunk()
    {
        BlockType ctype = BlockType.Air;

        biome = Biome.GetBiomeByBiomeNoise(Biome.BiomeNoise(chunkPosition.x / chunkSize, chunkPosition.z / chunkSize, 0.32f, 0.45f, 2));
        BiomeData biomeData = Biome.GetBiomeData(biome);

        fastNoise.SetFrequency(biomeData.Frequency);
        fastNoise.SetFractalOctaves(biomeData.Octave);
        fastNoise.SetFractalLacunarity(biomeData.Lacunarity);
        fastNoise.SetGradientPerturbAmp(biomeData.Amplitude);
        fastNoise.SetSeed(WorldHandler.instance.GetSeed());

        Vector3 voxelPos = new Vector3();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    ctype = BlockType.Air;
                    
                    SetRelief(ref ctype, x, y, z);

                    voxelPos.Set(chunkPosition.x + x, chunkPosition.y + y, chunkPosition.z + z);
                    chunkVoxels[voxelPos] = new Block(ctype);
                }

            }
        }
    }
    
    private void GenerateVertices()
    {
        Block cube = null;
        Vector3 voxelPosition = new Vector3();
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    voxelPosition.Set(chunkPosition.x + x, chunkPosition.y + y, chunkPosition.z + z);
                    if (chunkVoxels[voxelPosition].cubeType == BlockType.Air) continue;

                    foreach (KeyValuePair<Vector3, Orientation> entry in OrientationbMask)
                    {
                        cube = GetVoxelCube(chunkPosition.x + x + entry.Key.x,
                                            chunkPosition.y + y + entry.Key.y,
                                            chunkPosition.z + z + +entry.Key.z);

                        if (cube == null || cube.cubeType == BlockType.Air)
                        {
                            CreateFace(x, y, z, entry.Value);
                            CreateCollider(x, y, z, entry.Value);
                        }
                    }
                }
            }
        }
    }

    private Block GetVoxelCube(int x, int y, int z)
    {
        return GetVoxelCube(new Vector3(x, y, z));
    }

    private Block GetVoxelCube(float x, float y, float z)
    {
        return GetVoxelCube((int)x, (int)y, (int)z);
    }

    private Block GetVoxelCube(Vector3 pos)
    {
        if (!chunkVoxels.ContainsKey(pos)) return null;

        return chunkVoxels[pos];
    }

    void CreateFace(int x, int y, int z, Orientation o)
    {
        Vector3 voxelWorldPosition = new Vector3(chunkPosition.x + x, chunkPosition.y + y, chunkPosition.z + z);
        Vector2 texturePos = WorldHandler.instance.GetUVtextureOf(chunkVoxels[voxelWorldPosition].cubeType, o);
        BlockType cube = GetVoxelCube(voxelWorldPosition).cubeType;

        switch (o)
        {
            case Orientation.Up:
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x, y, z));
                break;

            case Orientation.Down:
                vertices.Add(new Vector3(x, y - 1, z));
                vertices.Add(new Vector3(x + 1, y - 1, z));
                vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                vertices.Add(new Vector3(x, y - 1, z + 1));
                break;

            case Orientation.Forward:
                vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y - 1, z + 1));
                break;

            case Orientation.Backward:
                vertices.Add(new Vector3(x, y - 1, z));
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y - 1, z));
                break;

            case Orientation.Right:
                vertices.Add(new Vector3(x, y - 1, z + 1));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y - 1, z));
                break;

            case Orientation.Left:
                vertices.Add(new Vector3(x + 1, y - 1, z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                break;
        }


        triangles.Add(faceCount * 4); //1
        triangles.Add(faceCount * 4 + 1); //2
        triangles.Add(faceCount * 4 + 2); //3
        triangles.Add(faceCount * 4); //1
        triangles.Add(faceCount * 4 + 2); //3
        triangles.Add(faceCount * 4 + 3); //4

        uvs.Add(new Vector2(tileUnit * texturePos.x + tileUnit, tileUnit * texturePos.y));
        uvs.Add(new Vector2(tileUnit * texturePos.x + tileUnit, tileUnit * texturePos.y + tileUnit));
        uvs.Add(new Vector2(tileUnit * texturePos.x, tileUnit * texturePos.y + tileUnit));
        uvs.Add(new Vector2(tileUnit * texturePos.x, tileUnit * texturePos.y));
        colors.Add(Color.green);
        colors.Add(Color.green);
        colors.Add(Color.green);
        colors.Add(Color.green);

        faceCount++;
    }

    private void CreateCollider(float x, float y, float z, Orientation o)
    {
        Vector3 voxelWorldPosition = new Vector3(chunkPosition.x + x, chunkPosition.y + y, chunkPosition.z + z);
        BlockType cube = GetVoxelCube(voxelWorldPosition).cubeType;
        if (cube == BlockType.Water) return;

        switch (o)
        {
            case Orientation.Up:
                colliderVertices.Add(new Vector3(x, y, z + 1));
                colliderVertices.Add(new Vector3(x + 1, y, z + 1));
                colliderVertices.Add(new Vector3(x + 1, y, z));
                colliderVertices.Add(new Vector3(x, y, z));
                break;

            case Orientation.Down:
                colliderVertices.Add(new Vector3(x, y - 1, z));
                colliderVertices.Add(new Vector3(x + 1, y - 1, z));
                colliderVertices.Add(new Vector3(x + 1, y - 1, z + 1));
                colliderVertices.Add(new Vector3(x, y - 1, z + 1));
                break;

            case Orientation.Forward:
                colliderVertices.Add(new Vector3(x + 1, y - 1, z + 1));
                colliderVertices.Add(new Vector3(x + 1, y, z + 1));
                colliderVertices.Add(new Vector3(x, y, z + 1));
                colliderVertices.Add(new Vector3(x, y - 1, z + 1));
                break;

            case Orientation.Backward:
                colliderVertices.Add(new Vector3(x, y - 1, z));
                colliderVertices.Add(new Vector3(x, y, z));
                colliderVertices.Add(new Vector3(x + 1, y, z));
                colliderVertices.Add(new Vector3(x + 1, y - 1, z));
                break;

            case Orientation.Right:
                colliderVertices.Add(new Vector3(x, y - 1, z + 1));
                colliderVertices.Add(new Vector3(x, y, z + 1));
                colliderVertices.Add(new Vector3(x, y, z));
                colliderVertices.Add(new Vector3(x, y - 1, z));
                break;

            case Orientation.Left:
                colliderVertices.Add(new Vector3(x + 1, y - 1, z));
                colliderVertices.Add(new Vector3(x + 1, y, z));
                colliderVertices.Add(new Vector3(x + 1, y, z + 1));
                colliderVertices.Add(new Vector3(x + 1, y - 1, z + 1));
                break;
        }

        colliderTriangles.Add(colFaceCount * 4);
        colliderTriangles.Add(colFaceCount * 4 + 1);
        colliderTriangles.Add(colFaceCount * 4 + 2);
        colliderTriangles.Add(colFaceCount * 4);
        colliderTriangles.Add(colFaceCount * 4 + 2);
        colliderTriangles.Add(colFaceCount * 4 + 3);

        colFaceCount++;
    }

    private void SetRelief(ref BlockType cube, float x, float y, float z)
    {
        if (chunkPosition.y < -32) return;

        chunkOffest.Set(chunkPosition.x + x, chunkPosition.y + y, chunkPosition.z + z);

        if(chunkOffest.y == -32)
        {
            cube = BlockType.Bedrock;
            return;
        }

        float sampleStone = fastNoise.GetSimplexFractal(chunkOffest.x, chunkOffest.z);
        sampleStone += fastNoise.GetPerlinFractal(chunkOffest.x, chunkOffest.z);
        sampleStone *= 25;
        float sampleDirt = sampleStone + 1;
        float sampleGrass = sampleStone + 2;

        float nextYChunk = chunkPosition.y + chunkSize;


        if (sampleGrass >= nextYChunk)
        {
            cube = BlockType.Stone;
        }
        else if (sampleStone <= 0)
        {
            if (chunkOffest.y <= 0)
            {
                if (sampleStone <= chunkOffest.y)
                {
                    cube = BlockType.Water;
                } else if(sampleStone - 3 <= chunkOffest.y)
                {
                    cube = BlockType.Sand;
                }
                else
                {
                    cube = BlockType.Stone;
                }
            }
        }
        else if ((chunkPosition.y <= sampleGrass) && (sampleGrass <= nextYChunk))
        {
            if (sampleStone > chunkOffest.y)
            {
                cube = BlockType.Stone;
            }
            else if (sampleDirt > chunkOffest.y)
            {
                cube = BlockType.Dirt;
            }
            else if (sampleGrass > chunkOffest.y)
            {
                cube = BlockType.Grass;
            }
        }
    }
}
