using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public int SizeX;
    public int SizeY;
    public int SizeZ;

    public GameObject chunkPrefab;

    private int selectedState = 0;
    private int numOfTrees = 100;

    public ChunkController[,,] Chunks;
    public WorldData worldData;

    public FastNoise FastNoise;

    private enum VFace
    {
        None,
        Up,
        Down,
        East,
        West,
        North,
        South
    }

    void Awake()
    {
        FastNoise = new FastNoise();
        worldData = new WorldData(SizeX, SizeY, SizeZ, 24, .5f);
        //FastNoise.SetFrequency(100);

        Chunks = new ChunkController[SizeX, SizeY, SizeZ];

        for (int y = 0; y < SizeY; ++y)
        {
            for (int z = 0; z < SizeZ; ++z)
            {
                for (int x = 0; x < SizeX; ++x)
                {
                    GameObject newChunk = Instantiate(chunkPrefab);
                    ChunkController chunkController = newChunk.GetComponent<ChunkController>();
                    chunkController.transform.parent = transform;
                    chunkController.Initialize(worldData.Chunks[x, y, z], x ,y ,z);                   

                    Chunks[x, y, z] = chunkController;                    
                }
            }
        }

        for (int y = 0; y < SizeY; ++y)
        {
            for (int z = 0; z < SizeZ; ++z)
            {
                for (int x = 0; x < SizeX; ++x)
                {
                    Chunks[x, y, z].SetChunkVoxelTerrain();
                }
            }
        }
        List<ChunkController> chunks = new List<ChunkController>();

        for (int y = 0; y < SizeY; ++y)
        {
            for (int z = 0; z < SizeZ; ++z)
            {
                for (int x = 0; x < SizeX; ++x)
                {
                    Chunks[x, y, z].RefreshChunkMesh();

                    MeshFilter meshFilter = Chunks[x, y, z].GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.mesh.vertexCount > 0)
                    {
                        chunks.Add(Chunks[x, y, z]);
                    }
                }
            }
        }

        

        for(int i = 0; i < numOfTrees; ++i)
        {
            int randomChunk = Mathf.FloorToInt(Random.Range(0, chunks.Count));

            var vertices = chunks[randomChunk].GetComponent<MeshFilter>().mesh.vertices;

            int randomVertex = Mathf.FloorToInt(Random.Range(0, vertices.Length));

            GameObject tree = Instantiate(Resources.Load<GameObject>("Trees/Test Tree"));
            tree.transform.parent = chunks[randomChunk].transform;
            tree.transform.localPosition = vertices[randomVertex];
            tree.transform.rotation = Quaternion.Euler(0,Random.Range(0, 360),0);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public Coordinate GetChunkIndex(ChunkController chunkController)
    {
        Vector3 vPosition = chunkController.transform.localPosition;
        float offset = chunkController.ChunkData.Resolution * chunkController.ChunkData.VoxelSize;

        return new Coordinate((int)(vPosition.x / offset), (int)(vPosition.y / offset), (int)(vPosition.z / offset));
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public ChunkController GetChunkRelativeToChunk(ChunkController originChunk, Coordinate offset)
    {
        Coordinate chunkIdx = GetChunkIndex(originChunk);

        //TODO check if chunk with offset exists in the world
        if ((chunkIdx.X + offset.X >= 0 && chunkIdx.X + offset.X < SizeX)
            && (chunkIdx.Y + offset.Y >= 0 && chunkIdx.Y + offset.Y < SizeY)
            && (chunkIdx.Z + offset.Z >= 0 && chunkIdx.Z + offset.Z < SizeZ))
        {
            return Chunks[chunkIdx.X + offset.X, chunkIdx.Y + offset.Y, chunkIdx.Z + offset.Z];
        }
        else
        {
            return null;
        }

    }



    private VFace GetHitFace(RaycastHit hit)
    {
        Vector3 incomingVec = hit.normal - Vector3.up;

        if (incomingVec == new Vector3(0, -1, -1))
            return VFace.South;

        if (incomingVec == new Vector3(0, -1, 1))
            return VFace.North;

        if (incomingVec == new Vector3(0, 0, 0))
            return VFace.Up;

        if (incomingVec == new Vector3(1, 1, 1))
            return VFace.Down;

        if (incomingVec == new Vector3(-1, -1, 0))
            return VFace.West;

        if (incomingVec == new Vector3(1, -1, 0))
            return VFace.East;

        return VFace.None;
    }
}
