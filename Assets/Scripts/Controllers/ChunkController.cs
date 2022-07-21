using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class ChunkController : MonoBehaviour
{
    public ChunkData ChunkData;

    public GameObject voxelPrefab;

    public VoxelController[,,] Voxels;

    public Dictionary<int, VoxelData> TriVoxelMap;

    public void Initialize(ChunkData chunkData, int x, int y, int z)
    {
        ChunkData = chunkData;
        TriVoxelMap = new Dictionary<int, VoxelData>();
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
        List<VoxelData> voxelDatas = new List<VoxelData>(8);

        List<MeshSkeletonVoxelModel> meshSkeletonsVoxelModels = new List<MeshSkeletonVoxelModel>();

        WorldData worldData = ChunkData.ParentWorld;

        ChunkData homeChunk;

        for (int i = 0; i < 8; ++i)
        {
            voxelDatas.Add(null);
        }

        

        for (int y = 0; y < ChunkData.Resolution; ++y)
        {
            for (int z = 0; z < ChunkData.Resolution; ++z)
            {
                for (int x = 0; x < ChunkData.Resolution; ++x)
                {
                    homeChunk = ChunkData;                  

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
                    
                    //Instead of setting voxels to active/inactive we are removing and instantiating voxels based on voxel data
                    if (meshSkeleton.Vertices.Count > 0)
                    {
                        meshSkeletonsVoxelModels.Add(new MeshSkeletonVoxelModel(meshSkeleton, voxelDatas[3]));
                    }                 

                    if (x != tempX && tempX > 0) x = tempX;
                    if (y != tempY && tempY > 0) y = tempY;
                    if (z != tempZ && tempZ > 0) z = tempZ;
                }
            }
        }

        MeshSkeletonTriVoxelMapModel skeletonMap = CombineMeshSkeletons(meshSkeletonsVoxelModels);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        Mesh mesh = MeshHelper.ConvertSkeletonToMesh(skeletonMap.MeshSkeleton);
       
        //mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;        
        TriVoxelMap = skeletonMap.TriVoxelMap;

    }

    public MeshSkeleton CombineMeshSkeletons(List<MeshSkeleton> meshSkeletons)
    {
        MeshSkeleton result = new MeshSkeleton();
        Dictionary<Vector3, int> vLu = new Dictionary<Vector3, int>();

        foreach(var skeleton in meshSkeletons)
        {
            foreach(var tri in skeleton.Triangles)
            {
                if (vLu.ContainsKey(skeleton.Vertices[tri]))
                {
                    result.Triangles.Add(vLu[skeleton.Vertices[tri]]);
                }
                else
                {
                    result.Vertices.Add(skeleton.Vertices[tri]);
                    vLu.Add(skeleton.Vertices[tri], vLu.Count);
                    result.Triangles.Add(vLu[skeleton.Vertices[tri]]);
                }
            }            
        }

        return result;
    }

    public MeshSkeletonTriVoxelMapModel CombineMeshSkeletons(List<MeshSkeletonVoxelModel> meshSkeletons)
    {
        MeshSkeleton meshSkeleton = new MeshSkeleton();
        Dictionary<Vector3, int> vLu = new Dictionary<Vector3, int>();
        MeshSkeletonTriVoxelMapModel result = new MeshSkeletonTriVoxelMapModel();
        Dictionary<int, VoxelData> triVoxelMap = new Dictionary<int, VoxelData>();

        int triCount = 0;

        foreach (var skeleton in meshSkeletons)
        {
            foreach (var tri in skeleton.MeshSkeleton.Triangles)
            {
                
                if (vLu.ContainsKey(skeleton.MeshSkeleton.Vertices[tri]))
                {
                    meshSkeleton.Triangles.Add(vLu[skeleton.MeshSkeleton.Vertices[tri]]);
                }
                else
                {
                    meshSkeleton.Vertices.Add(skeleton.MeshSkeleton.Vertices[tri]);
                    vLu.Add(skeleton.MeshSkeleton.Vertices[tri], vLu.Count);
                    meshSkeleton.Triangles.Add(vLu[skeleton.MeshSkeleton.Vertices[tri]]);
                }

                if(triCount % 3 == 0)
                {
                    triVoxelMap.Add(triCount / 3, skeleton.VoxelData);
                }
                ++triCount;
            }
        }

        result.MeshSkeleton = meshSkeleton;
        result.TriVoxelMap = triVoxelMap;

        return result;
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

   

}
