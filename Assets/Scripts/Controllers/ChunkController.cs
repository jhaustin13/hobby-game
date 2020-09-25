using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class ChunkController : MonoBehaviour
{
    public ChunkData ChunkData;

    public GameObject voxelPrefab;

    public VoxelController[,,] Voxels;

    public void Initialize(ChunkData chunkData, int x, int y, int z)
    {
        ChunkData = chunkData;
        int resolution = chunkData.Resolution;
        float voxelSize = chunkData.VoxelSize;
        float offset = resolution * voxelSize;

        transform.localPosition = new Vector3(x * offset, y * offset, z * offset);

        Voxels = new VoxelController[resolution, resolution, resolution];

        for (int vY = 0; vY < resolution; ++vY)
        {
            for (int vZ = 0; vZ < resolution; ++vZ)
            {
                for (int vX = 0; vX < resolution; ++vX)
                {
                    Voxels[vX, vY, vZ] = null;
                }
            }
        }


    }

    public Coordinate GetVoxelIndex(VoxelController voxelController)
    {
        Vector3 vPosition = voxelController.transform.localPosition;

        return new Coordinate((int)(vPosition.x / ChunkData.VoxelSize), (int)(vPosition.y / ChunkData.VoxelSize), (int)(vPosition.z / ChunkData.VoxelSize));
    }

    public void RefreshChunkMesh()
    {
        for (int y = 0; y < ChunkData.Resolution; ++y)
        {
            for (int z = 0; z < ChunkData.Resolution; ++z)
            {
                for (int x = 0; x < ChunkData.Resolution; ++x)
                {
                    //Potentially switch this to go through voxel data instead of voxel controllers
                    VoxelData currentVoxel = ChunkData.Voxels[x, y, z];
                    RefreshChunkMeshAtVoxel(currentVoxel);
                }
            }
        }
    }



    public void RefreshChunkMeshAtVoxel(VoxelData voxelData)
    {
        List<VoxelData> voxelDatas = new List<VoxelData>(8);

        WorldData worldData = voxelData.ParentChunk.ParentWorld;

        Coordinate voxelCoordinates = voxelData.ParentChunk.GetVoxelIndex(voxelData);

        //May have to fix this to work with voxel data instead of voxel controller
        int startX = voxelCoordinates.X - 1;
        int startY = voxelCoordinates.Y - 1;
        int startZ = voxelCoordinates.Z - 1;

        //Starting home chunk offset
        int chunkOffsetX = 0;
        int chunkOffsetY = 0;
        int chunkOffsetZ = 0;

        if (startX < 0)
        {
            chunkOffsetX = -1;
            startX = ChunkData.Resolution + startX;
        }

        if (startY < 0)
        {
            chunkOffsetY = -1;
            startY = ChunkData.Resolution + startY;
        }

        if (startZ < 0)
        {
            chunkOffsetZ = -1;
            startZ = ChunkData.Resolution + startZ;
        }

        ChunkData homeChunk = worldData.GetChunkRelativeToChunk(ChunkData, new Coordinate(chunkOffsetX, chunkOffsetY, chunkOffsetZ));

        if (homeChunk == null)
        {
            if (chunkOffsetX < 0) startX = 0;
            if (chunkOffsetY < 0) startY = 0;
            if (chunkOffsetZ < 0) startZ = 0;

            chunkOffsetX = 0;
            chunkOffsetY = 0;
            chunkOffsetZ = 0;

            homeChunk = ChunkData;
        }

        for (int i = 0; i < 8; ++i)
        {
            voxelDatas.Add(null);
        }

        ChunkData startHomeChunk = homeChunk;

        int endX = startX + 3;
        int endY = startY + 3;
        int endZ = startZ + 3;
        for (int y = startY; y < endY; ++y)
        {
            for (int z = startZ; z < endZ; ++z)
            {
                for (int x = startX; x < endX; ++x)
                {
                    homeChunk = startHomeChunk;

                    //These are for the chunk controller offset calculation to get the home chunk that might be manipulated during the loop
                    int cOffsetX = 0;
                    int cOffsetY = 0;
                    int cOffsetZ = 0;

                    int tempX = -1;
                    int tempY = -1;
                    int tempZ = -1;

                    for (int i = 0; i < 8; ++i)
                    {
                        voxelDatas[i] = null;
                    }

                    if (x >= ChunkData.Resolution)
                    {
                        ChunkData xChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 0, 0));
                        if (xChunk == null)
                        {
                            if (x != tempX && tempX > 0) x = tempX;
                            if (y != tempY && tempY > 0) y = tempY;
                            if (z != tempZ && tempZ > 0) z = tempZ;
                            continue;
                        }

                        homeChunk = xChunk;
                        cOffsetX++;

                        tempX = x;
                        x -= ChunkData.Resolution;
                    }

                    if (y >= ChunkData.Resolution)
                    {
                        ChunkData yChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 1, 0));
                        if (yChunk == null)
                        {
                            if (x != tempX && tempX > 0) x = tempX;
                            if (y != tempY && tempY > 0) y = tempY;
                            if (z != tempZ && tempZ > 0) z = tempZ;
                            continue;
                        }

                        homeChunk = yChunk;
                        cOffsetY++;

                        tempY = y;
                        y -= ChunkData.Resolution;
                    }

                    if (z >= ChunkData.Resolution)
                    {
                        ChunkData zChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 0, 1));
                        if (zChunk == null)
                        {
                            if (x != tempX && tempX > 0) x = tempX;
                            if (y != tempY && tempY > 0) y = tempY;
                            if (z != tempZ && tempZ > 0) z = tempZ;
                            continue;
                        }

                        homeChunk = zChunk;
                        cOffsetZ++;

                        tempZ = z;
                        z -= ChunkData.Resolution;
                    }

                    if (z + 1 >= ChunkData.Resolution && y + 1 >= ChunkData.Resolution && x + 1 >= ChunkData.Resolution)
                    {
                        ChunkData zyxPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 1, 1));

                        if (zyxPlusChunk == null)
                        {
                            if (x != tempX && tempX > 0) x = tempX;
                            if (y != tempY && tempY > 0) y = tempY;
                            if (z != tempZ && tempZ > 0) z = tempZ;
                            continue;
                        }

                        if (voxelDatas[5] == null) voxelDatas[5] = zyxPlusChunk.Voxels[x + 1 - ChunkData.Resolution, y + 1 - ChunkData.Resolution, z + 1 - ChunkData.Resolution];
                    }

                    if (x + 1 >= ChunkData.Resolution && z + 1 >= ChunkData.Resolution)
                    {
                        ChunkData xzPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 0, 1));

                        if (xzPlusChunk == null)
                        {
                            if (x != tempX && tempX > 0) x = tempX;
                            if (y != tempY && tempY > 0) y = tempY;
                            if (z != tempZ && tempZ > 0) z = tempZ;
                            continue;
                        }

                        voxelDatas[1] = xzPlusChunk.Voxels[x + 1 - ChunkData.Resolution, y, z + 1 - ChunkData.Resolution];

                        if (voxelDatas[5] == null) voxelDatas[5] = xzPlusChunk.Voxels[x + 1 - ChunkData.Resolution, y + 1, z + 1 - ChunkData.Resolution];
                    }

                    if (y + 1 >= ChunkData.Resolution && z + 1 >= ChunkData.Resolution)
                    {
                        ChunkData yzPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 1, 1));

                        if (yzPlusChunk == null)
                        {
                            if (x != tempX && tempX > 0) x = tempX;
                            if (y != tempY && tempY > 0) y = tempY;
                            if (z != tempZ && tempZ > 0) z = tempZ;
                            continue;
                        }

                        voxelDatas[4] = yzPlusChunk.Voxels[x, y + 1 - ChunkData.Resolution, z + 1 - ChunkData.Resolution];

                        if (voxelDatas[5] == null) voxelDatas[5] = yzPlusChunk.Voxels[x + 1, y + 1 - ChunkData.Resolution, z + 1 - ChunkData.Resolution];
                    }

                    if (x + 1 >= ChunkData.Resolution && y + 1 >= ChunkData.Resolution)
                    {
                        ChunkData xyPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 1, 0));

                        if (xyPlusChunk == null)
                        {
                            if (x != tempX && tempX > 0) x = tempX;
                            if (y != tempY && tempY > 0) y = tempY;
                            if (z != tempZ && tempZ > 0) z = tempZ;
                            continue;
                        }

                        voxelDatas[6] = xyPlusChunk.Voxels[x + 1 - ChunkData.Resolution, y + 1 - ChunkData.Resolution, z];

                        if (voxelDatas[5] == null) voxelDatas[5] = xyPlusChunk.Voxels[x + 1 - ChunkData.Resolution, y + 1 - ChunkData.Resolution, z + 1];
                    }

                    if (z + 1 >= ChunkData.Resolution)
                    {
                        ChunkData zPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 0, 1));

                        if (zPlusChunk == null)
                        {
                            if (x != tempX && tempX > 0) x = tempX;
                            if (y != tempY && tempY > 0) y = tempY;
                            if (z != tempZ && tempZ > 0) z = tempZ;
                            continue;
                        }

                        voxelDatas[0] = zPlusChunk.Voxels[x, y, z + 1 - ChunkData.Resolution];

                        if (voxelDatas[1] == null) voxelDatas[1] = zPlusChunk.Voxels[x + 1, y, z + 1 - ChunkData.Resolution];
                        if (voxelDatas[4] == null) voxelDatas[4] = zPlusChunk.Voxels[x, y + 1, z + 1 - ChunkData.Resolution];
                        if (voxelDatas[5] == null) voxelDatas[5] = zPlusChunk.Voxels[x + 1, y + 1, z + 1 - ChunkData.Resolution];
                    }

                    if (x + 1 >= ChunkData.Resolution)
                    {
                        ChunkData xPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 0, 0));

                        if (xPlusChunk == null)
                        {
                            if (x != tempX && tempX > 0) x = tempX;
                            if (y != tempY && tempY > 0) y = tempY;
                            if (z != tempZ && tempZ > 0) z = tempZ;
                            continue;
                        }

                        voxelDatas[2] = xPlusChunk.Voxels[x + 1 - ChunkData.Resolution, y, z];

                        if (voxelDatas[1] == null) voxelDatas[1] = xPlusChunk.Voxels[x + 1 - ChunkData.Resolution, y, z + 1];
                        if (voxelDatas[5] == null) voxelDatas[5] = xPlusChunk.Voxels[x + 1 - ChunkData.Resolution, y + 1, z + 1];
                        if (voxelDatas[6] == null) voxelDatas[6] = xPlusChunk.Voxels[x + 1 - ChunkData.Resolution, y + 1, z];
                    }

                    if (y + 1 >= ChunkData.Resolution)
                    {
                        ChunkData yPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 1, 0));

                        if (yPlusChunk == null)
                        {
                            if (x != tempX && tempX > 0) x = tempX;
                            if (y != tempY && tempY > 0) y = tempY;
                            if (z != tempZ && tempZ > 0) z = tempZ;
                            continue;
                        }

                        voxelDatas[7] = yPlusChunk.Voxels[x, y + 1 - ChunkData.Resolution, z];

                        if (voxelDatas[4] == null) voxelDatas[4] = yPlusChunk.Voxels[x, y + 1 - ChunkData.Resolution, z + 1];
                        if (voxelDatas[5] == null) voxelDatas[5] = yPlusChunk.Voxels[x + 1, y + 1 - ChunkData.Resolution, z + 1];
                        if (voxelDatas[6] == null) voxelDatas[6] = yPlusChunk.Voxels[x + 1, y + 1 - ChunkData.Resolution, z];
                    }


                    if (voxelDatas[0] == null) voxelDatas[0] = homeChunk.Voxels[x, y, z + 1];
                    if (voxelDatas[1] == null) voxelDatas[1] = homeChunk.Voxels[x + 1, y, z + 1];
                    if (voxelDatas[2] == null) voxelDatas[2] = homeChunk.Voxels[x + 1, y, z];
                    voxelDatas[3] = homeChunk.Voxels[x, y, z];
                    if (voxelDatas[4] == null) voxelDatas[4] = homeChunk.Voxels[x, y + 1, z + 1];
                    if (voxelDatas[5] == null) voxelDatas[5] = homeChunk.Voxels[x + 1, y + 1, z + 1];
                    if (voxelDatas[6] == null) voxelDatas[6] = homeChunk.Voxels[x + 1, y + 1, z];
                    if (voxelDatas[7] == null) voxelDatas[7] = homeChunk.Voxels[x, y + 1, z];

                    MeshSkeleton meshSkeleton = MarchingCubesHelper.GetTriangles(voxelDatas, homeChunk);

                    WorldController worldController = GetComponentInParent<WorldController>();

                    //Offset is calculate by the offset that may have occured at the beginning
                    //plus and offsets that may have happened during the loop
                    ChunkController homeChunkController = worldController.GetChunkRelativeToChunk(this,
                        new Coordinate(chunkOffsetX + cOffsetX, chunkOffsetY + cOffsetY, chunkOffsetZ + cOffsetZ));

                    if (homeChunkController == null)
                    {
                        if (x != tempX && tempX > 0) x = tempX;
                        if (y != tempY && tempY > 0) y = tempY;
                        if (z != tempZ && tempZ > 0) z = tempZ;
                        continue;
                    }

                    //Instead of setting voxels to active/inactive we are removing and instantiating voxels based on voxel data
                    if (meshSkeleton.Vertices.Count > 0)
                    {                        
                        if (homeChunkController.Voxels[x, y, z] == null)
                        {
                            CreateVoxel(homeChunkController, x, y, z);
                        }
                        homeChunkController.Voxels[x, y, z].SetMesh(MeshHelper.ConvertSkeletonToMesh(meshSkeleton), ChunkData.VoxelSize, true);
                    }
                    else
                    {
                        if (homeChunkController.Voxels[x, y, z] != null)
                        {
                            Destroy(homeChunkController.Voxels[x, y, z].gameObject);
                            homeChunkController.Voxels[x, y, z] = null;
                        }                        
                    }


                    if (x != tempX && tempX > 0) x = tempX;
                    if (y != tempY && tempY > 0) y = tempY;
                    if (z != tempZ && tempZ > 0) z = tempZ;
                }
            }
        }
    }

    public List<VoxelData> GetRelatedVoxelsAtVoxel(VoxelData voxelData)
    {
        List<VoxelData> voxelDatas = new List<VoxelData>(8);

        ChunkData homeChunk = voxelData.ParentChunk;
        WorldData worldData = homeChunk.ParentWorld;

        Coordinate voxelCoordinates = homeChunk.GetVoxelIndex(voxelData);

        int x = voxelCoordinates.X;
        int y = voxelCoordinates.Y;
        int z = voxelCoordinates.Z;

        for (int i = 0; i < 8; ++i)
        {
            voxelDatas.Add(null);
        }

        if (z + 1 >= ChunkData.Resolution && y + 1 >= ChunkData.Resolution && x + 1 >= ChunkData.Resolution)
        {
            ChunkData zyxPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 1, 1));

            if (voxelDatas[5] == null) voxelDatas[5] = zyxPlusChunk?.Voxels[x + 1 - ChunkData.Resolution, y + 1 - ChunkData.Resolution, z + 1 - ChunkData.Resolution];
        }

        if (x + 1 >= ChunkData.Resolution && z + 1 >= ChunkData.Resolution)
        {
            ChunkData xzPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 0, 1));

            voxelDatas[1] = xzPlusChunk?.Voxels[x + 1 - ChunkData.Resolution, y, z + 1 - ChunkData.Resolution];

            if (voxelDatas[5] == null && y + 1 < ChunkData.Resolution) voxelDatas[5] = xzPlusChunk?.Voxels[x + 1 - ChunkData.Resolution, y + 1, z + 1 - ChunkData.Resolution];
        }

        if (y + 1 >= ChunkData.Resolution && z + 1 >= ChunkData.Resolution)
        {
            ChunkData yzPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 1, 1));

            voxelDatas[4] = yzPlusChunk?.Voxels[x, y + 1 - ChunkData.Resolution, z + 1 - ChunkData.Resolution];

            if (voxelDatas[5] == null && x + 1 < ChunkData.Resolution) voxelDatas[5] = yzPlusChunk?.Voxels[x + 1, y + 1 - ChunkData.Resolution, z + 1 - ChunkData.Resolution];
        }

        if (x + 1 >= ChunkData.Resolution && y + 1 >= ChunkData.Resolution)
        {
            ChunkData xyPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 1, 0));

            voxelDatas[6] = xyPlusChunk?.Voxels[x + 1 - ChunkData.Resolution, y + 1 - ChunkData.Resolution, z];

            if (voxelDatas[5] == null && z + 1 < ChunkData.Resolution) voxelDatas[5] = xyPlusChunk?.Voxels[x + 1 - ChunkData.Resolution, y + 1 - ChunkData.Resolution, z + 1];
        }

        if (z + 1 >= ChunkData.Resolution)
        {
            ChunkData zPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 0, 1));

            voxelDatas[0] = zPlusChunk?.Voxels[x, y, z + 1 - ChunkData.Resolution];

            if (voxelDatas[1] == null && x + 1 < ChunkData.Resolution) voxelDatas[1] = zPlusChunk?.Voxels[x + 1, y, z + 1 - ChunkData.Resolution];
            if (voxelDatas[4] == null && y + 1 < ChunkData.Resolution) voxelDatas[4] = zPlusChunk?.Voxels[x, y + 1, z + 1 - ChunkData.Resolution];
            if (voxelDatas[5] == null && x + 1 < ChunkData.Resolution && y + 1 < ChunkData.Resolution) voxelDatas[5] = zPlusChunk?.Voxels[x + 1, y + 1, z + 1 - ChunkData.Resolution];
        }

        if (x + 1 >= ChunkData.Resolution)
        {
            ChunkData xPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(1, 0, 0));

            voxelDatas[2] = xPlusChunk?.Voxels[x + 1 - ChunkData.Resolution, y, z];

            if (voxelDatas[1] == null && z + 1 < ChunkData.Resolution) voxelDatas[1] = xPlusChunk?.Voxels[x + 1 - ChunkData.Resolution, y, z + 1];
            if (voxelDatas[5] == null && z + 1 < ChunkData.Resolution && y + 1 < ChunkData.Resolution) voxelDatas[5] = xPlusChunk?.Voxels[x + 1 - ChunkData.Resolution, y + 1, z + 1];
            if (voxelDatas[6] == null && y + 1 < ChunkData.Resolution) voxelDatas[6] = xPlusChunk?.Voxels[x + 1 - ChunkData.Resolution, y + 1, z];
        }

        if (y + 1 >= ChunkData.Resolution)
        {
            ChunkData yPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, new Coordinate(0, 1, 0));

            voxelDatas[7] = yPlusChunk?.Voxels[x, y + 1 - ChunkData.Resolution, z];

            if (voxelDatas[4] == null && z + 1 < ChunkData.Resolution) voxelDatas[4] = yPlusChunk?.Voxels[x, y + 1 - ChunkData.Resolution, z + 1];
            if (voxelDatas[5] == null && z + 1 < ChunkData.Resolution && x + 1 < ChunkData.Resolution) voxelDatas[5] = yPlusChunk?.Voxels[x + 1, y + 1 - ChunkData.Resolution, z + 1];
            if (voxelDatas[6] == null && x + 1 < ChunkData.Resolution) voxelDatas[6] = yPlusChunk?.Voxels[x + 1, y + 1 - ChunkData.Resolution, z];
        }


        if (voxelDatas[0] == null && z + 1 < ChunkData.Resolution) voxelDatas[0] = homeChunk.Voxels[x, y, z + 1];
        if (voxelDatas[1] == null && x + 1 < ChunkData.Resolution && z + 1 < ChunkData.Resolution) voxelDatas[1] = homeChunk.Voxels[x + 1, y, z + 1];
        if (voxelDatas[2] == null && x + 1 < ChunkData.Resolution) voxelDatas[2] = homeChunk.Voxels[x + 1, y, z];
        voxelDatas[3] = homeChunk.Voxels[x, y, z];
        if (voxelDatas[4] == null && y + 1 < ChunkData.Resolution && z + 1 < ChunkData.Resolution) voxelDatas[4] = homeChunk.Voxels[x, y + 1, z + 1];
        if (voxelDatas[5] == null && x + 1 < ChunkData.Resolution && y + 1 < ChunkData.Resolution && z + 1 < ChunkData.Resolution) voxelDatas[5] = homeChunk.Voxels[x + 1, y + 1, z + 1];
        if (voxelDatas[6] == null && x + 1 < ChunkData.Resolution && y + 1 < ChunkData.Resolution) voxelDatas[6] = homeChunk.Voxels[x + 1, y + 1, z];
        if (voxelDatas[7] == null && y + 1 < ChunkData.Resolution) voxelDatas[7] = homeChunk.Voxels[x, y + 1, z];

        return voxelDatas;
    }

    public void SetChunkVoxelTerrain()
    {
        WorldController worldController = GetComponentInParent<WorldController>();

        for (int y = 0; y < ChunkData.Resolution; ++y)
        {
            for (int z = 0; z < ChunkData.Resolution; ++z)
            {
                for (int x = 0; x < ChunkData.Resolution; ++x)
                {
                    //Potentially switch this to go through voxel data instead of voxel controllers
                    VoxelData currentVoxel = ChunkData.Voxels[x, y, z];
                    Vector3 voxelPosition = currentVoxel.Position + ChunkData.Position;
                    float terrainHeight = worldController.FastNoise.GetPerlin(voxelPosition.x, voxelPosition.z) * 10 + 5;

                    if (voxelPosition.y < terrainHeight)
                    {
                        currentVoxel.State = 1;
                    }
                    else
                    {
                        currentVoxel.State = 0;
                    }


                }
            }
        }



    }

    private void CreateVoxel(ChunkController homeChunk, int x, int y, int z)
    {
        GameObject newVoxel = Instantiate(voxelPrefab);
        VoxelController voxelController = newVoxel.GetComponent<VoxelController>();
        newVoxel.transform.parent = homeChunk.transform;
        voxelController.Initialize(homeChunk.ChunkData.Voxels[x, y, z]);

        newVoxel.transform.localPosition = homeChunk.ChunkData.Voxels[x, y, z].Position;


        homeChunk.Voxels[x, y, z] = voxelController;
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
