using UnityEngine;

public class CubeConstructor : MonoBehaviour
{
    [SerializeField]
    private Transform ghostCube;
    private const int chunkSize = 16;
    private const int gridSize = 1;
    [SerializeField]
    private LayerMask hitMask;

    public void Update()
    {
        PlaceCube();
        DeleteCube();
    }
    
    private void PlaceCube()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 2f, hitMask))
        {
            Vector3 p = new Vector3(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));
            ghostCube.transform.position = p + hit.normal;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SetBlock(hit.point + hit.normal, BlockType.Bedrock);
            }
        }
    }

    private void DeleteCube()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 3f, hitMask))
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                DeleteBlock(hit.point);
            }
        }
    }

    private void SetBlock(int x, int y, int z, BlockType ctype)
    {
        Vector3 cubePosition = new Vector3(x, y, z);
        Vector3 chunkPosition = WorldHandler.instance.GetChunkPosition(cubePosition);

        if (!WorldHandler.instance.WorldChunk.ContainsKey(chunkPosition)) return;
        if (!WorldHandler.instance.WorldChunk[chunkPosition].Cubes.ContainsKey(cubePosition)) return;

        WorldHandler.instance.WorldChunk[chunkPosition].Cubes[cubePosition].cubeType = ctype;

        WorldHandler.instance.UpdateChunk(chunkPosition);
    }

    private void SetBlock(Vector3 pos, BlockType ctype)
    {
        SetBlock(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z), ctype);
    }

    private void DeleteBlock(int x, int y, int z)
    {
        SetBlock(x, y, z, BlockType.Air);
    }

    private void DeleteBlock(Vector3 pos)
    {
        SetBlock(pos, BlockType.Air);
    }
}
