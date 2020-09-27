using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MeshSkeletonVoxelModel
{
    public MeshSkeleton MeshSkeleton { get; set; }

    public VoxelData VoxelData { get; set; }

    public MeshSkeletonVoxelModel(MeshSkeleton meshSkeleton, VoxelData voxelData)
    {
        MeshSkeleton = meshSkeleton;
        VoxelData = voxelData;
    }
}

