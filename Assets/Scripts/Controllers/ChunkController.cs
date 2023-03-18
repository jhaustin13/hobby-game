using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class ChunkController : MonoBehaviour
{
    public WorldController WorldController;
    public ChunkData ChunkData;
    public GameObject voxelPrefab;

    public enum ChunkState { Initialized, Setup, Ready, Error };

    public ChunkState State;

    public MeshSkeleton meshSkeleton;

    //public VoxelController[,,] Voxels;

    public Dictionary<int, VoxelData> TriVoxelMap;

    public void Initialize(ChunkData chunkData)
    {
        ChunkData = chunkData;
        TriVoxelMap = new Dictionary<int, VoxelData>();
        WorldController = GetComponentInParent<WorldController>();
        int resolution = chunkData.Resolution;
        float voxelSize = chunkData.VoxelSize;
        float offset = resolution * voxelSize;

        meshSkeleton = null;

        //transform.localPosition = new Vector3(ChunkData.Position.x * offset, ChunkData.Position.y * offset, ChunkData.Position.z * offset);
        transform.localPosition = new Vector3(ChunkData.Position.x, ChunkData.Position.y, ChunkData.Position.z);
        name = $"Chunk ({chunkData.Position.x / offset},{chunkData.Position.y / offset},{chunkData.Position.z / offset})";


        State = ChunkState.Initialized;

        //Debug.Log($"Chunk named {name} is now at state {State}");
        //Voxels = new VoxelController[resolution, resolution, resolution];

        //for (int vY = 0; vY < resolution; ++vY)
        //{
        //    for (int vZ = 0; vZ < resolution; ++vZ)
        //    {
        //        for (int vX = 0; vX < resolution; ++vX)
        //        {
        //            Voxels[vX, vY, vZ] = null;
        //        }
        //    }
        //}


    }

    public Coordinate GetVoxelIndex(VoxelController voxelController)
    {
        Vector3 vPosition = voxelController.transform.localPosition;

        return new Coordinate((int)(vPosition.x / ChunkData.VoxelSize), (int)(vPosition.y / ChunkData.VoxelSize), (int)(vPosition.z / ChunkData.VoxelSize));
    }

    private void Update()
    {
        if (meshSkeleton != null)
        {
            ApplyMeshSkeleton();
        }
    }

    private void ApplyMeshSkeleton()
    {
        if (meshSkeleton != null)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            Mesh mesh = MeshHelper.ConvertSkeletonToMesh(meshSkeleton);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
            State = ChunkState.Ready;
            //Debug.Log($"Chunk named {name} is now at state {State}");
            meshSkeleton = null;

        }
    }

    public void RefreshChunkMesh()
    {
        try
        {

            if (ChunkData.AllAir)
            {
                State = ChunkState.Ready;
                //Debug.Log($"Chunk named {name} is now at state {State}");
                return;
            }

            //Debug.Log($"Chunk name {name} is having its mesh refreshed");
            List<VoxelData> voxelDatas = new List<VoxelData>(8);

            List<MeshSkeletonVoxelModel> meshSkeletonsVoxelModels = new List<MeshSkeletonVoxelModel>();

            WorldData worldData = ChunkData.ParentWorld;

            ChunkData homeChunk;

            //List<ChunkData> adjChunkDatas = new List<ChunkData>()
            //{
            //    ChunkData
            //    ,worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusX)
            //    ,worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusY)
            //    ,worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusZ)
            //    ,worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusXYZ)
            //    ,worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusXY)
            //    ,worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusXZ)
            //    ,worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusYZ)
            //};

            //List<string> names = new List<string>();

            //foreach (ChunkData chunk in adjChunkDatas)
            //{
            //    names.Add($"Chunk ({chunk.Position.x},{chunk.Position.y},{chunk.Position.z})");
            //}

            //foreach (ChunkData chunk in adjChunkDatas)
            //{

            //    if (chunk == null)
            //    {
            //        Console.WriteLine( false);
            //    }

            //    if (chunk.Voxels[0, 0, 0] == null)
            //    {
            //        Console.WriteLine(false);
            //        Debug.unityLogger.Log($"Chunks named {string.Join(" ", names.ToArray())} failed during mesh refresh");
            //    }


            //}

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
                            ChunkData xChunk = worldData.GetChunkRelativeToChunk(homeChunk, Coordinate.PlusX);
                            if (xChunk == null)
                            {
                                if (x != tempX && tempX > 0) x = tempX;
                                if (y != tempY && tempY > 0) y = tempY;
                                if (z != tempZ && tempZ > 0) z = tempZ;
                                continue;
                            }

                            //Debug.Log("Changed home chunk for mesh refresh");
                            homeChunk = xChunk;

                            tempX = x;
                            x -= ChunkData.Resolution;
                        }

                        if (y >= ChunkData.Resolution)
                        {
                            ChunkData yChunk = worldData.GetChunkRelativeToChunk(homeChunk, Coordinate.PlusY);
                            if (yChunk == null)
                            {
                                if (x != tempX && tempX > 0) x = tempX;
                                if (y != tempY && tempY > 0) y = tempY;
                                if (z != tempZ && tempZ > 0) z = tempZ;
                                continue;
                            }

                            //Debug.Log("Changed home chunk for mesh refresh");
                            homeChunk = yChunk;

                            tempY = y;
                            y -= ChunkData.Resolution;
                        }

                        if (z >= ChunkData.Resolution)
                        {
                            ChunkData zChunk = worldData.GetChunkRelativeToChunk(homeChunk, Coordinate.PlusZ);
                            if (zChunk == null)
                            {
                                if (x != tempX && tempX > 0) x = tempX;
                                if (y != tempY && tempY > 0) y = tempY;
                                if (z != tempZ && tempZ > 0) z = tempZ;
                                continue;
                            }

                            //Debug.Log("Changed home chunk for mesh refresh");
                            homeChunk = zChunk;

                            tempZ = z;
                            z -= ChunkData.Resolution;
                        }

                        if (z + 1 >= ChunkData.Resolution && y + 1 >= ChunkData.Resolution && x + 1 >= ChunkData.Resolution)
                        {
                            ChunkData zyxPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, Coordinate.PlusXYZ);

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
                            ChunkData xzPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, Coordinate.PlusXZ);

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
                            ChunkData yzPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, Coordinate.PlusYZ);

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
                            ChunkData xyPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, Coordinate.PlusXY);

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
                            ChunkData zPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, Coordinate.PlusZ);

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
                            ChunkData xPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, Coordinate.PlusX);

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
                            ChunkData yPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, Coordinate.PlusY);

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

            meshSkeleton = skeletonMap.MeshSkeleton;
            //Mesh mesh = MeshHelper.ConvertSkeletonToMesh(skeletonMap.MeshSkeleton);

            //mesh.vertices = skeletonMap.MeshSkeleton.Vertices.ToArray();
            //mesh.triangles = skeletonMap.MeshSkeleton.Triangles.ToArray();

            //Vector2[] uvs = new Vector2[skeletonMap.MeshSkeleton.Vertices.Count];

            //for (int i = 0; i < uvs.Length; i++)
            //{
            //    uvs[i] = new Vector2(skeletonMap.MeshSkeleton.Vertices[i].x, skeletonMap.MeshSkeleton.Vertices[i].z);
            //}

            //mesh.uv = uvs;

            ////mesh.Optimize();
            //mesh.RecalculateNormals();
            //mesh.RecalculateBounds();

            //meshFilter.sharedMesh = mesh;
            //meshCollider.sharedMesh = mesh;        
            TriVoxelMap = skeletonMap.TriVoxelMap;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public bool IsReadyForRefresh()
    {

        List<ChunkData> adjControllers = new List<ChunkData>()
        {
            ChunkData
            ,WorldController.worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusX)
            ,WorldController.worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusY)
            ,WorldController.worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusZ)
            ,WorldController.worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusXYZ)
            ,WorldController.worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusXY)
            ,WorldController.worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusXZ)
            ,WorldController.worldData.GetChunkRelativeToChunk(ChunkData, Coordinate.PlusYZ)
        };

        foreach (ChunkData controller in adjControllers)
        {
            if (controller == null)
            {
                return false;
            }

            if (controller.Voxels[0, 0, 0] == null)
            {
                return false;
            }
        }

        return true;
    }

    public MeshSkeleton CombineMeshSkeletons(List<MeshSkeleton> meshSkeletons)
    {
        MeshSkeleton result = new MeshSkeleton();
        Dictionary<Vector3, int> vLu = new Dictionary<Vector3, int>();

        foreach (var skeleton in meshSkeletons)
        {
            foreach (var tri in skeleton.Triangles)
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

                if (triCount % 3 == 0)
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
        for (int y = 0; y < ChunkData.Resolution; ++y)
        {
            for (int z = 0; z < ChunkData.Resolution; ++z)
            {
                for (int x = 0; x < ChunkData.Resolution; ++x)
                {
                    //Potentially switch this to go through voxel data instead of voxel controllers
                    VoxelData currentVoxel = ChunkData.Voxels[x, y, z];
                    Vector3 voxelPosition = currentVoxel.Position + ChunkData.Position;
                    float terrainHeight = WorldController.FastNoise.GetPerlin(voxelPosition.x, voxelPosition.z) * 10 + 5;

                    if (voxelPosition.y < terrainHeight)
                    {
                        currentVoxel.State = 1;
                        ChunkData.AllAir = false;
                    }
                    else
                    {
                        currentVoxel.State = 0;
                    }


                }
            }
        }

        State = ChunkState.Setup;

        //Debug.Log($"Chunk named {name} is now at state {State}");
    }



}
