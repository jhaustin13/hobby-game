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
        worldData = new WorldData(SizeX, SizeY, SizeZ, 10, 1);
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

        for (int y = 0; y < SizeY; ++y)
        {
            for (int z = 0; z < SizeZ; ++z)
            {
                for (int x = 0; x < SizeX; ++x)
                {
                    Chunks[x, y, z].RefreshChunkMesh();
                }
            }
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
        if (Input.GetMouseButtonUp(0))
        { //Break voxel
            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (hitInfo.collider.gameObject.GetComponent<VoxelController>() != null)
                {
                    GameObject voxel = hitInfo.collider.gameObject;
                    VoxelController voxelController = voxel.GetComponent<VoxelController>();
                    VoxelData voxelData = voxelController.GetVoxelData();
                    ChunkController chunkController = voxelController.GetComponentInParent<ChunkController>();

                    List<VoxelData> voxels = chunkController.GetRelatedVoxelsAtVoxel(voxelController.GetVoxelData());

                    foreach (var v in voxels)
                    {
                        v.State = 0;
                    }


                    chunkController.RefreshChunkMeshAtVoxel(voxelController.GetVoxelData());
                }
            }
        }
        else if (Input.GetMouseButtonUp(1))
        { //Place voxel
            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (hitInfo.collider.gameObject.GetComponent<VoxelController>() != null)
                {
                    GameObject voxel = hitInfo.collider.gameObject;
                    VoxelController voxelController = voxel.GetComponent<VoxelController>();

                    ChunkController chunkController = voxelController.GetComponentInParent<ChunkController>();

                    var voxels = chunkController.GetRelatedVoxelsAtVoxel(voxelController.GetVoxelData());

                    foreach (var v in voxels)
                    {
                        v.State = 1;
                    }

                    chunkController.RefreshChunkMeshAtVoxel(voxelController.GetVoxelData());
                }
            }
        }
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
