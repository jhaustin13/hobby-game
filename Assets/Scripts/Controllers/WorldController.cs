using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public int Size;

    public GameObject chunkPrefab;

    private int selectedState = 0;

    public ChunkController[,,] Chunks;

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
        Chunks = new ChunkController[Size, Size, Size];

        for (int y = 0; y < Size; ++y)
        {
            for (int z = 0; z < Size; ++z)
            {
                for (int x = 0; x < Size; ++x)
                {
                    GameObject newChunk = Instantiate(chunkPrefab);
                    ChunkController chunkController = newChunk.GetComponent<ChunkController>();
                    newChunk.transform.parent = transform;
                    int resolution = chunkController.Resolution;
                    float voxelSize = chunkController.VoxelSize;
                    float offset = resolution * voxelSize;

                    newChunk.transform.localPosition = new Vector3(x * offset, y * offset, z * offset);
                    chunkController.Initialize();

                    Chunks[x, y, z] = chunkController;
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
        float offset = chunkController.Resolution * chunkController.VoxelSize;

        return new Coordinate((int)(vPosition.x / offset), (int)(vPosition.y / offset), (int)(vPosition.z / offset));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        { //Break voxel
            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (hitInfo.collider.gameObject.GetComponent<VoxelController>() != null)
                {
                    GameObject voxel = hitInfo.collider.gameObject;
                    VoxelController voxelController = voxel.GetComponent<VoxelController>();

                    ChunkController chunkController = voxelController.GetComponentInParent<ChunkController>();

                    if (voxelController.GetVoxelData().State == 1)
                    {
                        voxelController.RefreshState(0);
                    }

                    chunkController.RefreshChunkMesh();
                }
            }
        }
        else if (Input.GetMouseButton(1))
        { //Place voxel
            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (hitInfo.collider.gameObject.GetComponent<VoxelController>() != null)
                {
                    GameObject voxel = hitInfo.collider.gameObject;
                    VoxelController voxelController = voxel.GetComponent<VoxelController>();

                    VFace face = GetHitFace(hitInfo);

                    ChunkController chunkController = voxelController.GetComponentInParent<ChunkController>();

                    Coordinate vIndex = chunkController.GetVoxelIndex(voxelController);

                    int Xidx = vIndex.X;
                    int Yidx = vIndex.Y;
                    int Zidx = vIndex.Z;

                    VoxelController adjVoxel = null;
                    ChunkController adjChunk = null;

                    switch (face)
                    {
                        case VFace.Up:
                            if (Yidx + 1 >= chunkController.Resolution)
                            {
                                adjChunk = GetChunkRelativeToChunk(chunkController, new Coordinate(0, 1, 0));

                                adjVoxel = adjChunk?.Voxels[Xidx, Yidx + 1 - chunkController.Resolution, Zidx];
                            }
                            else
                            {
                                adjVoxel = chunkController.Voxels[Xidx, Yidx + 1, Zidx];
                            }

                            break;
                        case VFace.Down:
                            if (Yidx - 1 < 0)
                            {
                                adjChunk = GetChunkRelativeToChunk(chunkController, new Coordinate(0, -1, 0));

                                adjVoxel = adjChunk?.Voxels[Xidx, chunkController.Resolution + Yidx - 1, Zidx];
                            }
                            else
                            {
                                adjVoxel = chunkController.Voxels[Xidx, Yidx - 1, Zidx];
                            }

                            break;
                        case VFace.North:
                            if (Zidx + 1 >= chunkController.Resolution)
                            {
                                adjChunk = GetChunkRelativeToChunk(chunkController, new Coordinate(0, 0, 1));

                                adjVoxel = adjChunk?.Voxels[Xidx, Yidx, Zidx + 1 - chunkController.Resolution];
                            }
                            else
                            {
                                adjVoxel = chunkController.Voxels[Xidx, Yidx, Zidx + 1];
                            }

                            break;
                        case VFace.South:
                            if (Zidx - 1 < 0)
                            {
                                adjChunk = GetChunkRelativeToChunk(chunkController, new Coordinate(0, 0, -1));

                                adjVoxel = adjChunk?.Voxels[Xidx, Yidx, chunkController.Resolution + Zidx - 1];
                            }
                            else
                            {
                                adjVoxel = chunkController.Voxels[Xidx, Yidx, Zidx - 1];
                            }

                            break;
                        case VFace.East:
                            if (Xidx + 1 >= chunkController.Resolution)
                            {
                                adjChunk = GetChunkRelativeToChunk(chunkController, new Coordinate(1, 0, 0));

                                adjVoxel = adjChunk?.Voxels[Xidx + 1 - chunkController.Resolution, Yidx, Zidx];
                            }
                            else
                            {
                                adjVoxel = chunkController.Voxels[Xidx + 1, Yidx, Zidx];
                            }

                            break;
                        case VFace.West:
                            if (Xidx - 1 < 0)
                            {
                                adjChunk = GetChunkRelativeToChunk(chunkController, new Coordinate(-1, 0, 0));

                                adjVoxel = adjChunk?.Voxels[chunkController.Resolution + Xidx - 1, Yidx, Zidx];
                            }
                            else
                            {
                                adjVoxel = chunkController.Voxels[Xidx - 1, Yidx, Zidx];
                            }

                            break;
                        case VFace.None:
                            break;
                    }

                    if (adjVoxel != null && adjVoxel.GetVoxelData().State == 0)
                    {
                        adjVoxel.RefreshState(1);
                    }

                    if (adjChunk != null)
                    {
                        adjChunk.RefreshChunkMesh();
                        //chunkController.RefreshChunkMesh();
                    }
                    else
                    {
                        chunkController.RefreshChunkMesh();
                    }
                }
            }
        }
    }

    public ChunkController GetChunkRelativeToChunk(ChunkController originChunk, Coordinate offset)
    {
        Coordinate chunkIdx = GetChunkIndex(originChunk);

        //TODO check if chunk with offset exists in the world
        if ((chunkIdx.X + offset.X >= 0 && chunkIdx.X + offset.X < Size)
            && (chunkIdx.Y + offset.Y >= 0 && chunkIdx.Y + offset.Y < Size)
            && (chunkIdx.Z + offset.Z >= 0 && chunkIdx.Z + offset.Z < Size))
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
