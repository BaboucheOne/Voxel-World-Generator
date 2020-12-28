using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class WorldHandler : MonoBehaviour
{
    public static WorldHandler instance;
    private int seed;

    public Dictionary<BlockType, BlockProperties> BlocksProterties = new Dictionary<BlockType, BlockProperties>();

    [SerializeField]
    private Transform Player;
    [SerializeField]
    private Vector3 PlayerPosition = new Vector3();
    private Vector3 PreviousPlayerPosition = Vector3.down;
    [SerializeField]
    private GameObject chunkPrefab;

    private Dictionary<BlockType, BlockProperties> blocksProperties = new Dictionary<BlockType, BlockProperties>();

    [SerializeField]
    private int ViewRange = 8;
    private const int ChunkSize = 16;
    private int renderDistance;
    private List<Vector3> activeChunks = new List<Vector3>();
    public Dictionary<Vector3, Chunk> WorldChunk = new Dictionary<Vector3, Chunk>();

    private Queue<ChunkThreadInfo<ChunkData>> chunkDataInfoQueue = new Queue<ChunkThreadInfo<ChunkData>>();
    private Queue<ChunkThreadInfo<ChunkData>> ChunkThreadUpdateDataInfoQueue = new Queue<ChunkThreadInfo<ChunkData>>();

    private void Awake()
    {
        if (instance) return;

        System.Random r = new System.Random();
        seed = r.Next(1000000);
        instance = this;

        renderDistance = ChunkSize * ViewRange;

        LoadBlocksProperties();
    }

    public int GetSeed() => seed;
    public BlockProperties GetBlockProperties(BlockType t) => blocksProperties[t];
    public Vector2 GetUVtextureOf(BlockType t, Orientation o) => blocksProperties[t].GetTexture(o);
    public Vector2[] GetTextures(BlockType t) => blocksProperties[t].TexturesUV;

    public void Update()
    {
        ComputeGridCoordonnate();
        LoadChunks();

        ReadChunkDataThreadInQueue();
        ReadChunksUpdate();

        PreviousPlayerPosition = Player.position;
    }

    #region mesh thread
    private void ReadChunkDataThreadInQueue()
    {
        for (int i = 0; i < chunkDataInfoQueue.Count; i++)
        {
            ChunkThreadInfo<ChunkData> info = chunkDataInfoQueue.Dequeue();
            info.callback(info.parameter);
        }
    }

    private void RequestChunkMeshData(Action<ChunkData> callback, Vector3 pos)
    {
        ThreadStart ts = delegate
        {
            ChunkMeshDataThread(callback, pos);
        };

        new Thread(ts).Start();
    }

    private void ChunkMeshDataThread(Action<ChunkData> callback, Vector3 pos)
    {
        ChunkData data = new VoxelGenerator().GenerateChunk(pos);
        lock(chunkDataInfoQueue)
        {
            chunkDataInfoQueue.Enqueue(new ChunkThreadInfo<ChunkData>(callback, data));
        }
    }
    #endregion mesh thread

    #region update mesh thread
    private void ReadChunksUpdate()
    {
        for (int i = 0; i < ChunkThreadUpdateDataInfoQueue.Count; i++)
        {
            ChunkThreadInfo<ChunkData> info = ChunkThreadUpdateDataInfoQueue.Dequeue();
            info.callback(info.parameter);
        }
    }

    private void RequestForChunkUpdate(Action<ChunkData> callback, Vector3 pos)
    {
        ThreadStart ts = delegate
        {
            ChunkUpdateThread(callback, pos);
        };

        new Thread(ts).Start();
    }

    private void ChunkUpdateThread(Action<ChunkData> callback, Vector3 pos)
    {
        ChunkData data = new VoxelGenerator().GenerateChunkByVoxelCube(pos, WorldChunk[pos].Cubes);

        lock (ChunkThreadUpdateDataInfoQueue)
        {
            ChunkThreadUpdateDataInfoQueue.Enqueue(new ChunkThreadInfo<ChunkData>(callback, data));
        }
    }
    #endregion update mesh tread

    private void CreateChunk(Vector3 Position)
    {
        Vector3 chunkPosition = GetChunkPosition(Position);
        if (!WorldChunk.ContainsKey(chunkPosition))
        {
            GameObject go = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity, transform) as GameObject;
            WorldChunk.Add(chunkPosition, new Chunk(go, chunkPosition));
            RequestChunkMeshData(BuildChunkWithChunkData, chunkPosition);
        } else
        {
            WorldChunk[chunkPosition].MeshObject.SetActive(true);
        }

        activeChunks.Add(Position);
    }

    public void UpdateChunk(Vector3 pos)
    {
        RequestForChunkUpdate(BuildChunkWithChunkData, pos);
    }

    public Vector3 GetChunkPosition(Vector3 v)
    {
        int posX = Mathf.RoundToInt(v.x / ChunkSize) * ChunkSize;
        int posY = Mathf.RoundToInt(v.y / ChunkSize) * ChunkSize;
        int posZ = Mathf.RoundToInt(v.z / ChunkSize) * ChunkSize;
        
        return new Vector3(posX, posY, posZ);
    }

    private void ComputeGridCoordonnate()
    {
        int x = Mathf.RoundToInt(Player.position.x / ChunkSize) * ChunkSize;
        int y = Mathf.RoundToInt(Player.position.y / ChunkSize) * ChunkSize;
        int z = Mathf.RoundToInt(Player.position.z / ChunkSize) * ChunkSize;
        PlayerPosition.Set(x, y, z);
    }

    private void BuildChunkWithChunkData(ChunkData data)
    {
        GameObject go = WorldChunk[data.Position].MeshObject;

        WorldChunk[data.Position].Cubes = data.voxelCubes;
        WorldChunk[data.Position].biome = data.biome;

        Mesh mesh = new Mesh
        {
            name = "Chunk " + data.biome.ToString(),
            vertices = data.vertices,
            triangles = data.triangles,
            uv = data.uvs,
            colors = data.colors
        };

        Mesh colliderMesh = new Mesh
        {
            vertices = data.colVertices,
            triangles = data.colTriangles
        };

        colliderMesh.Optimize();

        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.OptimizeReorderVertexBuffer();

        go.GetComponent<MeshFilter>().mesh = mesh;
        go.GetComponent<MeshCollider>().sharedMesh = colliderMesh;
    }

    private void LoadChunks()
    {
        if (PreviousPlayerPosition != Player.position)
        {
            Vector3 chunkPosition = new Vector3();
            for (int y = -ChunkSize; y < renderDistance; y += ChunkSize)
            {
                for (int x = -renderDistance; x <= renderDistance; x += ChunkSize)
                {
                    for (int z = -renderDistance; z <= renderDistance; z += ChunkSize)
                    {
                        chunkPosition.Set(PlayerPosition.x + x, PlayerPosition.y + y, PlayerPosition.z + z);
                        CreateChunk(chunkPosition);
                    }
                }
            }
        
            UnloadChunks();
        }

    }

    private void UnloadChunks()
    {
        for(int i = activeChunks.Count - 1; i >= 0; i--)
        {
            if(Vector3.Distance(Player.position, activeChunks[i]) >= renderDistance)
            {
                WorldChunk[GetChunkPosition(activeChunks[i])].MeshObject.SetActive(false);
                activeChunks.RemoveAt(i);
            }
        }
    }
    
    private void LoadBlocksProperties()
    {
        string json = System.IO.File.ReadAllText("blockProperties.json");
        BlockPropertiesList propertiesList = JsonUtility.FromJson<BlockPropertiesList>(json);

        foreach(BlockProperties bp in propertiesList.properties)
        {
            blocksProperties.Add(bp.Blocktype, bp);
        }
    }

    internal struct ChunkThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public ChunkThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}
