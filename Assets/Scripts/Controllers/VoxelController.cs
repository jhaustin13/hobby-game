using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelController : MonoBehaviour
{
    private VoxelData voxelData;

    public bool debug;

    private GameObject debugPrimitive;

    public void Initialize(ChunkData chunkData, Vector3 position, int voxelState)
    {
        voxelData = new VoxelData(chunkData, position, voxelState);

        if(debug)
        {
            debugPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            debugPrimitive.transform.parent = transform;
            debugPrimitive.transform.localPosition = Vector3.zero;
            debugPrimitive.transform.localScale = Vector3.one * .1f;

            RefreshState(voxelState);
        }
    }

    public void Initialize(VoxelData voxelData)
    {
        this.voxelData = voxelData;



        if (debug)
        {
            debugPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            debugPrimitive.transform.parent = transform;
            debugPrimitive.transform.localPosition = Vector3.zero;
            debugPrimitive.transform.localScale = Vector3.one * .1f;

           
        }

        RefreshState(voxelData.State);
    }

    public VoxelData GetVoxelData()
    {
        return voxelData;
    }

    public void RefreshState(int voxelState)
    {       

        if(debug)
        {
            switch (voxelState)
            {
                case 0:
                    debugPrimitive.GetComponent<MeshRenderer>().material.color = Color.white;                    
                    break;
                case 1:
                    debugPrimitive.GetComponent<MeshRenderer>().material.color = Color.black;                    
                    break;
            }
        }        

        voxelData.State = voxelState;
    }
    // Start is called before the first frame update

    public void SetMesh(Mesh mesh, float voxelSize, bool offsetPosition = false)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        Vector3[] newVertices = new Vector3[mesh.vertices.Length];

        float halfVoxelSize = voxelSize / 2;

        Vector3 globalMeshOffset = new Vector3(halfVoxelSize, halfVoxelSize, halfVoxelSize);

        if(offsetPosition)
        { 
            for (int i = 0; i < mesh.vertices.Length; ++i)
            {                
                newVertices[i] = mesh.vertices[i] - transform.localPosition - globalMeshOffset;
            }

            mesh.vertices = newVertices;
        }


        Vector2[] uvs = new Vector2[mesh.vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);
        }
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = mesh;
        meshFilter.mesh = mesh;
        meshFilter.sharedMesh = mesh;

        meshRenderer.material.color = Color.white;


    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
