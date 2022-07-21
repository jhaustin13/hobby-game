using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ChunkData
{
    public WorldData ParentWorld { get; }
    public Vector3 Position { get; }
    public int Resolution { get; }
    public VoxelData[,,] Voxels { get; }
    public float VoxelSize { get; }
    public List<WorldItemData> Items { get; set; }
    public ChunkData(WorldData parent, Vector3 position, int resolution, float voxelSize)
    {
        this.Position = position;
        this.Resolution = resolution;
        this.VoxelSize = voxelSize;
        this.ParentWorld = parent;
        Items = new List<WorldItemData>();

        Voxels = new VoxelData[resolution, resolution, resolution];

        for (int y = 0; y < Resolution; ++y)
        {
            for (int z = 0; z < Resolution; ++z)
            {
                for (int x = 0; x < Resolution; ++x)
                {
                    Voxels[x, y, z] = new VoxelData(this, new Vector3(x * VoxelSize, y * VoxelSize, z * VoxelSize), 0);
                }
            }
        }
    }

    public void SetVoxel(int x, int y, int z, VoxelData voxelData)
    {
        Voxels[x, y, z] = voxelData;
    }

    public Coordinate GetVoxelIndex(VoxelData voxelData)
    {
        Vector3 vPosition = voxelData.Position;

        return new Coordinate((int)(vPosition.x / VoxelSize), (int)(vPosition.y / VoxelSize), (int)(vPosition.z / VoxelSize));
    }

    public Pair<List<VoxelData>, List<Coordinate>> GetRelatedVoxelsAtVoxel(VoxelData voxelData)
    {
        List<VoxelData> voxelDatas = new List<VoxelData>(8);
        List<Coordinate> offsets = new List<Coordinate>();
        Pair<List<VoxelData>, List<Coordinate>> result = new Pair<List<VoxelData>, List<Coordinate>>();

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

        if (z + 1 >= Resolution && y + 1 >= Resolution && x + 1 >= Resolution)
        {
            Coordinate offset = new Coordinate(1, 1, 1);
            ChunkData zyxPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, offset);

            offsets.Add(offset);

            if (voxelDatas[5] == null) voxelDatas[5] = zyxPlusChunk?.Voxels[x + 1 - Resolution, y + 1 - Resolution, z + 1 - Resolution];
        }

        if (x + 1 >= Resolution && z + 1 >= Resolution)
        {
            Coordinate offset = new Coordinate(1, 0, 1);
            ChunkData xzPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, offset);

            offsets.Add(offset);
            if (y == 0) offsets.Add(new Coordinate(1, -1, 1));

            voxelDatas[1] = xzPlusChunk?.Voxels[x + 1 - Resolution, y, z + 1 - Resolution];

            if (voxelDatas[5] == null && y + 1 < Resolution) voxelDatas[5] = xzPlusChunk?.Voxels[x + 1 - Resolution, y + 1, z + 1 - Resolution];
        }

        if (y + 1 >= Resolution && z + 1 >= Resolution)
        {
            Coordinate offset = new Coordinate(0, 1, 1);
            ChunkData yzPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, offset);

            offsets.Add(offset);
            if (x == 0) offsets.Add(new Coordinate(-1, 1, 1));

            voxelDatas[4] = yzPlusChunk?.Voxels[x, y + 1 - Resolution, z + 1 - Resolution];

            if (voxelDatas[5] == null && x + 1 < Resolution) voxelDatas[5] = yzPlusChunk?.Voxels[x + 1, y + 1 - Resolution, z + 1 - Resolution];
        }

        if (x + 1 >= Resolution && y + 1 >= Resolution)
        {
            Coordinate offset = new Coordinate(1, 1, 0);
            ChunkData xyPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, offset);

            offsets.Add(offset);
            if (z == 0) offsets.Add(new Coordinate(1, 1, -1));

            voxelDatas[6] = xyPlusChunk?.Voxels[x + 1 - Resolution, y + 1 - Resolution, z];

            if (voxelDatas[5] == null && z + 1 < Resolution) voxelDatas[5] = xyPlusChunk?.Voxels[x + 1 - Resolution, y + 1 - Resolution, z + 1];
        }

        if (z + 1 >= Resolution)
        {
            Coordinate offset = new Coordinate(0, 0, 1);
            ChunkData zPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, offset);

            offsets.Add(offset);
            if (y == 0) offsets.Add(new Coordinate(0, -1, 1));
            if (x == 0) offsets.Add(new Coordinate(-1, 0, 1));
            if (x == 0 && y == 0) offsets.Add(new Coordinate(-1, -1, 1));

            voxelDatas[0] = zPlusChunk?.Voxels[x, y, z + 1 - Resolution];

            if (voxelDatas[1] == null && x + 1 < Resolution) voxelDatas[1] = zPlusChunk?.Voxels[x + 1, y, z + 1 - Resolution];
            if (voxelDatas[4] == null && y + 1 < Resolution) voxelDatas[4] = zPlusChunk?.Voxels[x, y + 1, z + 1 - Resolution];
            if (voxelDatas[5] == null && x + 1 < Resolution && y + 1 < Resolution) voxelDatas[5] = zPlusChunk?.Voxels[x + 1, y + 1, z + 1 - Resolution];
        }

        if (x + 1 >= Resolution)
        {
            Coordinate offset = new Coordinate(1, 0, 0);
            ChunkData xPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, offset);

            offsets.Add(offset);
            if (z == 0) offsets.Add(new Coordinate(1, 0, -1));
            if (y == 0) offsets.Add(new Coordinate(1, -1, 0));
            if (y == 0 && z == 0) offsets.Add(new Coordinate(1, -1, -1));

            voxelDatas[2] = xPlusChunk?.Voxels[x + 1 - Resolution, y, z];

            if (voxelDatas[1] == null && z + 1 < Resolution) voxelDatas[1] = xPlusChunk?.Voxels[x + 1 - Resolution, y, z + 1];
            if (voxelDatas[5] == null && z + 1 < Resolution && y + 1 < Resolution) voxelDatas[5] = xPlusChunk?.Voxels[x + 1 - Resolution, y + 1, z + 1];
            if (voxelDatas[6] == null && y + 1 < Resolution) voxelDatas[6] = xPlusChunk?.Voxels[x + 1 - Resolution, y + 1, z];
        }

        if (y + 1 >= Resolution)
        {
            Coordinate offset = new Coordinate(0, 1, 0);
            ChunkData yPlusChunk = worldData.GetChunkRelativeToChunk(homeChunk, offset);

            offsets.Add(offset);
            if (z == 0) offsets.Add(new Coordinate(0, 1, -1));
            if (x == 0) offsets.Add(new Coordinate(-1, 1, 0));
            if (x == 0 && z == 0) offsets.Add(new Coordinate(-1, 1, -1));

            voxelDatas[7] = yPlusChunk?.Voxels[x, y + 1 - Resolution, z];

            if (voxelDatas[4] == null && z + 1 < Resolution) voxelDatas[4] = yPlusChunk?.Voxels[x, y + 1 - Resolution, z + 1];
            if (voxelDatas[5] == null && z + 1 < Resolution && x + 1 < Resolution) voxelDatas[5] = yPlusChunk?.Voxels[x + 1, y + 1 - Resolution, z + 1];
            if (voxelDatas[6] == null && x + 1 < Resolution) voxelDatas[6] = yPlusChunk?.Voxels[x + 1, y + 1 - Resolution, z];
        }

        if (x == 0) offsets.Add(new Coordinate(-1, 0, 0));
        if (y == 0) offsets.Add(new Coordinate(0, -1, 0));
        if (z == 0) offsets.Add(new Coordinate(0, 0, -1));

        if (x == 0 && y == 0 && z == 0) offsets.Add(new Coordinate(-1, -1, -1));
        if (x == 0 && y == 0) offsets.Add(new Coordinate(-1, -1, 0));
        if (x == 0 && z == 0) offsets.Add(new Coordinate(-1, 0, -1));
        if (y == 0 && z == 0) offsets.Add(new Coordinate(0, -1, -1));


        if (voxelDatas[0] == null && z + 1 < Resolution) voxelDatas[0] = homeChunk.Voxels[x, y, z + 1];
        if (voxelDatas[1] == null && x + 1 < Resolution && z + 1 < Resolution) voxelDatas[1] = homeChunk.Voxels[x + 1, y, z + 1];
        if (voxelDatas[2] == null && x + 1 < Resolution) voxelDatas[2] = homeChunk.Voxels[x + 1, y, z];
        voxelDatas[3] = homeChunk.Voxels[x, y, z];
        if (voxelDatas[4] == null && y + 1 < Resolution && z + 1 < Resolution) voxelDatas[4] = homeChunk.Voxels[x, y + 1, z + 1];
        if (voxelDatas[5] == null && x + 1 < Resolution && y + 1 < Resolution && z + 1 < Resolution) voxelDatas[5] = homeChunk.Voxels[x + 1, y + 1, z + 1];
        if (voxelDatas[6] == null && x + 1 < Resolution && y + 1 < Resolution) voxelDatas[6] = homeChunk.Voxels[x + 1, y + 1, z];
        if (voxelDatas[7] == null && y + 1 < Resolution) voxelDatas[7] = homeChunk.Voxels[x, y + 1, z];

        result.First = voxelDatas;
        result.Second = offsets;

        return result;
    }
}
