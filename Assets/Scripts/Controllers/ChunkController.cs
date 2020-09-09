using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class ChunkController : MonoBehaviour
{
    private ChunkData chunkData;
    public float VoxelSize;
    public int Resolution;

    public GameObject voxelPrefab;

    public VoxelController[,,] Voxels;

    public void Initialize()
    {
        Voxels = new VoxelController[Resolution, Resolution, Resolution];
        chunkData = new ChunkData(transform.position, Resolution , VoxelSize);

        for(int y = 0; y < Resolution; ++y)
        {
            for(int z = 0; z < Resolution; ++z)
            {
                for(int x = 0; x < Resolution; ++x)
                {
                    GameObject voxel = Instantiate(voxelPrefab);
                    VoxelController voxelController = voxel.GetComponent<VoxelController>();
                    Voxels[x, y, z] = voxelController;
                    voxel.transform.parent = transform;


                    voxel.transform.localPosition = new Vector3(x * VoxelSize, y * VoxelSize, z * VoxelSize);

                    voxel.GetComponent<BoxCollider>().size = new Vector3(VoxelSize, VoxelSize, VoxelSize);

                    voxelController.Initialize(voxel.transform.localPosition, 1);
                    chunkData.SetVoxel(x, y, z, voxelController.GetVoxelData());
                }
            }
        }
    }

    public Coordinate GetVoxelIndex(VoxelController voxelController)
    {
        Vector3 vPosition = voxelController.transform.localPosition;

        return new Coordinate( (int) (vPosition.x / VoxelSize), (int) (vPosition.y / VoxelSize), (int) (vPosition.z / VoxelSize));
    }

    public void RefreshChunkMesh()
    {
        List<VoxelData> voxelDatas = new List<VoxelData>(8);

        for(int i = 0; i < 8; ++i)
        {
            voxelDatas.Add(null);
        }

        for (int y = 0; y < Resolution - 1; ++y)
        {
            for (int z = 0; z < Resolution - 1; ++z)
            {
                for (int x = 0; x < Resolution - 1; ++x)
                {
                    voxelDatas[0] = Voxels[x, y, z + 1].GetVoxelData();
                    voxelDatas[1] = Voxels[x + 1, y, z + 1].GetVoxelData();
                    voxelDatas[2] = Voxels[x + 1, y, z].GetVoxelData();
                    voxelDatas[3] = Voxels[x, y, z].GetVoxelData();
                    voxelDatas[4] = Voxels[x, y + 1, z + 1].GetVoxelData();
                    voxelDatas[5] = Voxels[x + 1, y + 1, z + 1].GetVoxelData();
                    voxelDatas[6] = Voxels[x + 1, y + 1, z].GetVoxelData();
                    voxelDatas[7] = Voxels[x, y + 1, z].GetVoxelData();

                    Voxels[x, y, z].SetMesh(MeshHelper.ConvertSkeletonToMesh(MarchingCubesHelper.GetTriangles(voxelDatas)), VoxelSize, true);                                 
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
