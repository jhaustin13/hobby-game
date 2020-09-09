using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MeshHelper
{
    public static Mesh ConvertSkeletonToMesh(MeshSkeleton meshSkeleton)
    {
        Mesh mesh = new Mesh();

        mesh.vertices = meshSkeleton.Vertices.ToArray();
        mesh.triangles = meshSkeleton.Triangles.ToArray();

        
        Vector2[] uvs = new Vector2[meshSkeleton.Vertices.Count];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(meshSkeleton.Vertices[i].x, meshSkeleton.Vertices[i].z);
        }

        mesh.uv = uvs;

        return mesh;
    }
}

