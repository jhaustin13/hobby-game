using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VoxelData
{
    public int State;

    public Vector3 Position { get; }

    public ChunkData ParentChunk { get; }

    public VoxelData(ChunkData parentChunk, Vector3 position, int state)
    {
        State = state;
        Position = position;
        ParentChunk = parentChunk;
    }
}
