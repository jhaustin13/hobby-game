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
}

