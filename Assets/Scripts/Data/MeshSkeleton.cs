using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class MeshSkeleton
{
    public List<Vector3> Vertices;
    public List<int> Triangles;

    public MeshSkeleton()
    {
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
    }
}

