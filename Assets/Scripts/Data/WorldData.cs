using Assets.Scripts.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class WorldData
{
    public Vector3 Position { get; }

    public ChunkData[,,] Chunks { get; }

    public FastNoise FastNoise { get; }

    public int SizeX;
    public int SizeY;
    public int SizeZ;


    public WorldData(int sizeX, int sizeY, int sizeZ, int chunkResolution, float voxelSize)
    {
        Chunks = new ChunkData[sizeX, sizeY, sizeZ];
        float offset = chunkResolution * voxelSize;

        SizeX = sizeX;
        SizeY = sizeY;
        SizeZ = sizeZ;

        Position = Vector3.zero;
        FastNoise = new FastNoise();

        for (int y = 0; y < sizeY; ++y)
        {
            for (int z = 0; z < sizeZ; ++z)
            {
                for (int x = 0; x < sizeX; ++x)
                {                   
                    Chunks[x, y, z] = new ChunkData(this,new Vector3(x * offset, y * offset, z * offset), chunkResolution, voxelSize);
                }
            }
        }
        
    }

    public ChunkData GetChunkRelativeToChunk(ChunkData originChunk, Coordinate offset)
    {
        Coordinate chunkIdx = GetChunkIndex(originChunk);

        //TODO check if chunk with offset exists in the world
        if ((chunkIdx.X + offset.X >= 0 && chunkIdx.X + offset.X < SizeX)
            && (chunkIdx.Y + offset.Y >= 0 && chunkIdx.Y + offset.Y < SizeY)
            && (chunkIdx.Z + offset.Z >= 0 && chunkIdx.Z + offset.Z < SizeZ))
        {
            return Chunks[chunkIdx.X + offset.X, chunkIdx.Y + offset.Y, chunkIdx.Z + offset.Z];
        }
        else
        {
            return null;
        }

    }

    public Coordinate GetChunkIndex(ChunkData chunkData)
    {
        Vector3 vPosition = chunkData.Position;
        float offset = chunkData.Resolution * chunkData.VoxelSize;

        return new Coordinate((int)(vPosition.x / offset), (int)(vPosition.y / offset), (int)(vPosition.z / offset));
    }

    
    public bool PlaceItemInWorld(Vector3 position, Quaternion rotation, ChunkController chunkController, WorldItemData worldItem, Mesh mesh)
    {
        bool successfullyPlaced = false;

        //eventually look up world item info but for now we'll include that info on the world item object
        var itemInfo = ResourceCache.Instance.GetItemInfo(worldItem.Id);

        //determine 'home voxel'

        //using bounds and rotation get the range of all potential voxels item can be placed in        
        var rotatedBounds = MeshHelper.GetRotatedBounds(itemInfo.Bounds, rotation);
        rotatedBounds.center = position;
        Bounds voxelBounds = new Bounds();
        VoxelData currentVoxel = null;
        ChunkData currentChunk = null;
        Coordinate chunkOffset = new Coordinate(0,0,0);
        Vector3 voxelResolution = new Vector3(chunkController.ChunkData.VoxelSize, chunkController.ChunkData.VoxelSize, chunkController.ChunkData.VoxelSize);
        
        //Assumption being made is that we are picking to lowest point on the mesh so we shouldn't have to check y lower bound reducing potential cases by 9
        for (int y = Mathf.FloorToInt(rotatedBounds.min.y); y < Mathf.CeilToInt(rotatedBounds.max.y); ++y)
        {
            for (int x = Mathf.FloorToInt(rotatedBounds.min.x); x < Mathf.CeilToInt(rotatedBounds.max.x); ++x)
            {
                for (int z = Mathf.FloorToInt(rotatedBounds.min.z); x < Mathf.CeilToInt(rotatedBounds.max.z); ++z)
                {
                    currentVoxel = null;
                    currentChunk = null;

                    if(x > SizeX && y > SizeY && z > SizeZ)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusXYZ);
                        currentVoxel = currentChunk?.Voxels[x - SizeX, y - SizeY, z - SizeZ];
                    }
                    else if (x < 0 && y > SizeY && z > SizeZ)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusYZNegX);
                        currentVoxel = currentChunk?.Voxels[x + SizeX, y - SizeY, z - SizeZ];
                    }
                    else if (x > SizeX && y > SizeY && z < 0)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusXYNegZ);
                        currentVoxel = currentChunk?.Voxels[x - SizeX, y - SizeY, z + SizeZ];
                    }
                    else if (x < 0 && y > SizeY && z < 0)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusYNegXZ);
                        currentVoxel = currentChunk?.Voxels[x + SizeX, y - SizeY, z + SizeZ];
                    }
                    else if (x > SizeX && y > SizeY)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusXY);
                        currentVoxel = currentChunk?.Voxels[x - SizeX, y - SizeY, z];
                    }
                    else if (x < 0 && y > SizeY)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusYNegX);
                        currentVoxel = currentChunk?.Voxels[x + SizeX, y - SizeY, z];
                    }
                    else if (z < 0 && y > SizeY)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusYNegZ);
                        currentVoxel = currentChunk?.Voxels[x, y - SizeY, z + SizeZ];
                    }
                    else if (x > SizeX && z > SizeZ)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusXZ);
                        currentVoxel = currentChunk?.Voxels[x - SizeX, y, z - SizeZ];
                    }
                    else if (x < 0 && z > SizeZ)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusZNegX);
                        currentVoxel = currentChunk?.Voxels[x + SizeX, y, z - SizeZ];
                    }
                    else if (x > SizeX && z < SizeZ)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusXNegZ);
                        currentVoxel = currentChunk?.Voxels[x - SizeX, y, z + SizeZ];
                    }
                    else if (x < 0 && z < 0)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.NegXZ);
                        currentVoxel = currentChunk?.Voxels[x + SizeX, y, z + SizeZ];
                    }
                    else if (y > SizeY && z > SizeZ)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusYZ);
                        currentVoxel = currentChunk?.Voxels[x, y - SizeY, z - SizeZ];
                    }
                    else if (x > SizeX)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusX);
                        currentVoxel = currentChunk?.Voxels[x - SizeX, y, z];
                    }
                    else if (y > SizeY)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusY);
                        currentVoxel = currentChunk?.Voxels[x , y - SizeY, z ];
                    }
                    else if (z > SizeZ)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.PlusZ);
                        currentVoxel = currentChunk?.Voxels[x, y, z - SizeZ];
                    }
                    else if (x < 0)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.NegX);
                        currentVoxel = currentChunk?.Voxels[x + SizeX, y, z];
                    }
                    else if (z < 0)
                    {
                        currentChunk = GetChunkRelativeToChunk(chunkController.ChunkData, Coordinate.NegZ);
                        currentVoxel = currentChunk?.Voxels[x, y, z + SizeZ];
                    }
                    else
                    {
                        currentChunk = chunkController.ChunkData;
                        currentVoxel = currentChunk?.Voxels[x, y, z];
                    }

                    if (currentChunk != null && currentVoxel != null)
                    {
                        voxelBounds = new Bounds(MeshHelper.GetCentroid(currentChunk.GetRelatedVoxelsAtVoxel(currentVoxel).First.Select(x => x.Position).ToArray()), voxelResolution);

                        foreach(var vertex in mesh.vertices)
                        {
                            if(voxelBounds.Contains(vertex))
                            {
                                break;
                            }
                        }                   
                    }
                }
            }
        }




        //loop through resulting voxels and assign a reference to the placed item if there is some of the object in the voxel 'space'



        return successfullyPlaced;
    }

    
}

