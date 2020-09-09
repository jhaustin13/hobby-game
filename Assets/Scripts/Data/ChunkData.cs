using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ChunkData
{
    [NonSerialized]
    private Vector3 position;
    private int resolution;   
    private VoxelData[,,] voxels;
    private float voxelSize;    

    public ChunkData(Vector3 position, int resolution, float voxelSize)
    {
        this.position = position;
        this.resolution = resolution;
        this.voxelSize = voxelSize;

        voxels = new VoxelData[resolution, resolution, resolution];
    }

    public void SetVoxel(int x , int y, int z, VoxelData voxelData)
    {
        voxels[x, y, z] = voxelData;
    }
}
