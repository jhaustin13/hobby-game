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

                    voxelController.Initialize(voxel.transform.localPosition, 0);
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

    public void RefreshChunkMesh(VoxelController voxelController)
    {
        List<VoxelController> voxelControllers = new List<VoxelController>(8);        

        WorldController worldController = GetComponentInParent<WorldController>();

        Coordinate voxelCoordinates = GetVoxelIndex(voxelController);


        //Also need to fix if a target voxel is on an edge, currently it doesn't update voxels on the negative edge because of code below
        //Will need more elegant solution
        int startX = voxelCoordinates.X - 1;
        int startY = voxelCoordinates.Y - 1;
        int startZ = voxelCoordinates.Z - 1;

        int chunkOffsetX = 0;
        int chunkOffsetY = 0;
        int chunkOffsetZ = 0;

        if(startX < 0)
        {
            chunkOffsetX = -1;
            startX = Resolution + startX;
        }

        if(startY < 0)
        {
            chunkOffsetY = -1;
            startY = Resolution + startY;
        }

        if(startZ < 0)
        {
            chunkOffsetZ = -1;
            startZ = Resolution + startZ;
        }

        ChunkController homeChunk = worldController.GetChunkRelativeToChunk(this, new Coordinate(chunkOffsetX, chunkOffsetY, chunkOffsetZ));

        if(homeChunk == null)
        {
            if (chunkOffsetX < 0) startX = 0;
            if (chunkOffsetY < 0) startY = 0;
            if (chunkOffsetZ < 0) startZ = 0;

            homeChunk = this;
        }

        for (int i = 0; i < 8; ++i)
        {
            voxelControllers.Add(null);
        }

       
        
        

        ChunkController startHomeChunk = homeChunk;
        int endY = startY + 2;
        for (int y = startY; y < endY; ++y)
        {
            int endZ = startZ + 2;
            for (int z = startZ; z < endZ; ++z)
            {
                int endX = startX + 2;
                homeChunk = startHomeChunk;
                for (int x = startX; x < endX; ++x)
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        voxelControllers[i] = null;
                    }

                    if (x == Resolution)
                    {
                        ChunkController xChunk = worldController.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 0, 0));
                        if (xChunk == null) continue;

                        homeChunk = xChunk;

                        x -= Resolution;
                        endX -= Resolution;
                    }

                    if(y == Resolution)
                    {
                        ChunkController yChunk = worldController.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 1, 0));
                        if (yChunk == null) continue;

                        homeChunk = yChunk;

                        y -= Resolution;
                        endY -= Resolution;
                    }

                    if(z == Resolution)
                    {
                        ChunkController zChunk = worldController.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 0, 1));
                        if (zChunk == null) continue;

                        homeChunk = zChunk;

                        z -= Resolution;
                        endZ -= Resolution;
                    }

                    

                    if (z + 1 >= Resolution && y + 1 >= Resolution && x + 1 >= Resolution)
                    {
                        ChunkController zyxPlusChunk = worldController.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 1, 1));

                        if (zyxPlusChunk == null) continue;

                        if (voxelControllers[5] == null) voxelControllers[5] = zyxPlusChunk.Voxels[x + 1 - Resolution, y + 1 - Resolution, z + 1 - Resolution];
                    }
                    
                    if (x + 1 >= Resolution && z + 1 >= Resolution)
                    {
                        ChunkController xzPlusChunk = worldController.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 0, 1));

                        if (xzPlusChunk == null) continue;

                        voxelControllers[1] = xzPlusChunk.Voxels[x + 1 - Resolution, y, z + 1 - Resolution];

                        if (voxelControllers[5] == null) voxelControllers[5] = xzPlusChunk.Voxels[x + 1 - Resolution, y + 1, z + 1 - Resolution];
                    }
                    
                    if (y + 1 >= Resolution && z + 1 >= Resolution)
                    {
                        ChunkController yzPlusChunk = worldController.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 1, 1));

                        if (yzPlusChunk == null) continue;

                        voxelControllers[4] = yzPlusChunk.Voxels[x, y + 1 - Resolution, z + 1 - Resolution];

                        if (voxelControllers[5] == null) voxelControllers[5] = yzPlusChunk.Voxels[x + 1, y + 1 - Resolution, z + 1 - Resolution];
                    }
                    
                    if (x + 1 >= Resolution && y + 1 >= Resolution)
                    {
                        ChunkController xyPlusChunk = worldController.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 1, 0));

                        if (xyPlusChunk == null) continue;

                        voxelControllers[6] = xyPlusChunk.Voxels[x + 1 - Resolution, y + 1 - Resolution, z];

                        if (voxelControllers[5] == null) voxelControllers[5] = xyPlusChunk.Voxels[x + 1 - Resolution, y + 1 - Resolution, z + 1];
                    }
                    
                    if (z + 1 >= Resolution)
                    {
                        ChunkController zPlusChunk = worldController.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 0, 1));

                        if (zPlusChunk == null) continue;

                        voxelControllers[0] = zPlusChunk.Voxels[x, y, z + 1 - Resolution];

                        if (voxelControllers[1] == null) voxelControllers[1] = zPlusChunk.Voxels[x + 1, y, z + 1 - Resolution];
                        if (voxelControllers[4] == null) voxelControllers[4] = zPlusChunk.Voxels[x, y + 1, z + 1 - Resolution];
                        if (voxelControllers[5] == null) voxelControllers[5] = zPlusChunk.Voxels[x + 1, y + 1, z + 1 - Resolution];
                    }
                    
                    if (x + 1 >= Resolution)
                    {
                        ChunkController xPlusChunk = worldController.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 0, 0));

                        if (xPlusChunk == null) continue;

                        voxelControllers[2] = xPlusChunk.Voxels[x + 1 - Resolution, y, z];

                        if (voxelControllers[1] == null) voxelControllers[1] = xPlusChunk.Voxels[x + 1 - Resolution, y, z + 1];
                        if (voxelControllers[5] == null) voxelControllers[5] = xPlusChunk.Voxels[x + 1 - Resolution, y + 1, z + 1];
                        if (voxelControllers[6] == null) voxelControllers[6] = xPlusChunk.Voxels[x + 1 - Resolution, y + 1, z];                        
                    }
                    
                    if (y + 1 >= Resolution)
                    {
                        ChunkController yPlusChunk = worldController.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 1, 0));

                        if (yPlusChunk == null) continue;

                        voxelControllers[7] = yPlusChunk.Voxels[x, y + 1 - Resolution, z];

                        if (voxelControllers[4] == null) voxelControllers[4] = yPlusChunk.Voxels[x, y + 1 - Resolution, z + 1];
                        if (voxelControllers[5] == null) voxelControllers[5] = yPlusChunk.Voxels[x + 1, y + 1 - Resolution, z + 1];
                        if (voxelControllers[6] == null) voxelControllers[6] = yPlusChunk.Voxels[x + 1, y + 1 - Resolution, z];
                    }
                    
                    
                    if (voxelControllers[0] == null) voxelControllers[0] = homeChunk.Voxels[x, y, z + 1];
                    if (voxelControllers[1] == null) voxelControllers[1] = homeChunk.Voxels[x + 1, y, z + 1];
                    if (voxelControllers[2] == null) voxelControllers[2] = homeChunk.Voxels[x + 1, y, z];
                    voxelControllers[3] = homeChunk.Voxels[x, y, z];
                    if (voxelControllers[4] == null) voxelControllers[4] = homeChunk.Voxels[x, y + 1, z + 1];
                    if (voxelControllers[5] == null) voxelControllers[5] = homeChunk.Voxels[x + 1, y + 1, z + 1];
                    if (voxelControllers[6] == null) voxelControllers[6] = homeChunk.Voxels[x + 1, y + 1, z];
                    if (voxelControllers[7] == null) voxelControllers[7] = homeChunk.Voxels[x, y + 1, z];


                    homeChunk.Voxels[x, y, z].SetMesh(MeshHelper.ConvertSkeletonToMesh(MarchingCubesHelper.GetTriangles(voxelControllers)), VoxelSize, true);                                 
                }
            }
        }
    }

    public void SetChunkVoxelTerrain()
    {
        WorldController worldController = GetComponentInParent<WorldController>();

        for (int y = 0; y < Resolution; ++y)
        {
            for (int z = 0; z < Resolution; ++z)
            {
                for (int x = 0; x < Resolution; ++x)
                {
                    VoxelController currentVoxel = Voxels[x, y, z];
                    Vector3 voxelPosition = currentVoxel.transform.position;
                    float terrainHeight = worldController.FastNoise.GetPerlin(voxelPosition.x, voxelPosition.z) * 10 + 5;

                    if (voxelPosition.y < terrainHeight)
                    {
                        currentVoxel.RefreshState(1);
                    }
                    else
                    {
                        currentVoxel.RefreshState(0);
                    }

                    RefreshChunkMesh(currentVoxel);
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
