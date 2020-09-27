using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MeshSkeletonTriVoxelMapModel
{
    public MeshSkeleton MeshSkeleton { get; set; }

    public Dictionary<int, VoxelData> TriVoxelMap { get; set; }

    public MeshSkeletonTriVoxelMapModel()
    {
        MeshSkeleton = new MeshSkeleton();
        TriVoxelMap = new Dictionary<int, VoxelData>();
    }

    public MeshSkeletonTriVoxelMapModel(MeshSkeleton meshSkeleton, Dictionary<int, VoxelData> triVoxelMap)
    {
        MeshSkeleton = meshSkeleton;
        TriVoxelMap = triVoxelMap;
    }
}

