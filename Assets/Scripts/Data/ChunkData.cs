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

    public ChunkData(WorldData parent, Vector3 position, int resolution, float voxelSize)
    {
        this.Position = position;
        this.Resolution = resolution;
        this.VoxelSize = voxelSize;
        this.ParentWorld = parent;

        Voxels = new VoxelData[resolution, resolution, resolution];

        for(int y = 0; y < Resolution; ++y)
        {
            for(int z = 0; z < Resolution; ++z)
            {
                for(int x= 0; x < Resolution; ++x)
                {
                    Voxels[x, y, z] = new VoxelData(this, new Vector3(x * VoxelSize, y * VoxelSize, z * VoxelSize), 0);
                }
            }
        }
    }

    public void SetVoxel(int x , int y, int z, VoxelData voxelData)
    {
        Voxels[x, y, z] = voxelData;
    }

    public Coordinate GetVoxelIndex(VoxelData voxelData)
    {
        Vector3 vPosition = voxelData.Position;

        return new Coordinate((int)(vPosition.x / VoxelSize), (int)(vPosition.y / VoxelSize), (int)(vPosition.z / VoxelSize));
    }
}
