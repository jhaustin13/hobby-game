using Priority_Queue;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class ChunkLoader
    {
        private WorldController worldController;

        private AsyncParallelProcessor chunkSetup;

        private AsyncParallelProcessor chunkRefresher;

        public SimplePriorityQueue<ChunkData> ChunkQueue;

        public List<ChunkController> ChunkCheckQueue;

        private float workInterval;

        private Stopwatch queueTimer;

        public ChunkLoader(WorldController worldController)
        {
            this.worldController = worldController;
            chunkSetup = new AsyncParallelProcessor(1);
            chunkRefresher = new AsyncParallelProcessor(1);
            ChunkQueue = new SimplePriorityQueue<ChunkData>();
            ChunkCheckQueue = new List<ChunkController>();
            ChunkCheckQueue = new List<ChunkController>();
            queueTimer = new Stopwatch();
            workInterval = (1 / 200f) * 1000;
        }

        public void EnqueueChunkSetup(Action action, float priority)
        {
            chunkSetup.QueueTask(action, priority);
        }

        public void EnqueueChunkRefresh(Action action, float priority)
        {
            chunkRefresher.QueueTask(action, priority);
        }

        public void StartProcessor()
        {
            chunkRefresher.ProcessQueue();
            chunkSetup.ProcessQueue();
        }

        public void ProcessChunkQueue()
        {
            queueTimer.Restart();

            //UnityEngine.Debug.Log($"Processing Chunk Queue {ChunkQueue.Count} chunks in queue");
            while (ChunkQueue.Count > 0 && queueTimer.ElapsedMilliseconds < workInterval)
            {
                float priority = ChunkQueue.GetPriority(ChunkQueue.First);

                ChunkData chunk;
                if (ChunkQueue.TryDequeue(out chunk))
                {
                    LoadChunk(chunk, priority);
                }
            }

            queueTimer.Stop();


            //UnityEngine.Debug.Log($"Processing Chunk Check Queue {ChunkCheckQueue.Count} chunks in queue");
            List<ChunkController> chunksToRefresh = new List<ChunkController>();
            foreach (ChunkController chunk in ChunkCheckQueue)
            {
                if (chunk.ChunkData.AllAir)
                {
                    chunksToRefresh.Add(chunk);
                    chunk.State = ChunkController.ChunkState.Ready;
                    continue;
                }
                if (chunk.IsReadyForRefresh())
                {
                    chunksToRefresh.Add(chunk);
                    ThreadPool.QueueUserWorkItem(RefreshChunkMesh, chunk);
                    //chunkRefresher.QueueTask( () => chunk.RefreshChunkMesh(), 3f);

                }
            }

            ChunkCheckQueue.RemoveAll(x => chunksToRefresh.Contains(x));          

        }

        public void LoadChunk(ChunkData chunkData, float priority)
        {
            chunkData.InitializeVoxels();

            GameObject newChunk = worldController.ChunkPool.GetObject();
            ChunkController chunkController = newChunk.GetComponent<ChunkController>();
            chunkController.transform.parent = worldController.transform;
            float chunkLength = chunkData.VoxelSize * chunkData.Resolution;

            int x = (int)(chunkData.Position.x / chunkLength);
            int y = (int)(chunkData.Position.y / chunkLength);
            int z = (int)(chunkData.Position.z / chunkLength);
            chunkController.Initialize(chunkData);

            worldController.Chunks[x, y, z] = chunkController;
            if (priority < 0)
            {
                chunkController.SetChunkVoxelTerrain();
            }
            else
            {
                //UnityEngine.Debug.Log($"Adding chunk named {chunkController.name} currently at {chunkController.State} to queue to be setup");
                ThreadPool.QueueUserWorkItem(SetChunkVoxelTerrain, chunkController);
                //chunkSetup.QueueTask(() => { chunkController.SetChunkVoxelTerrain(); },priority);                
                ChunkCheckQueue.Add(chunkController);
            }
        }

        public void RefreshChunkMesh(object chunk)
        {
            ((ChunkController) chunk).RefreshChunkMesh();
        }

        public void SetChunkVoxelTerrain(object chunk)
        {
            ((ChunkController)chunk).SetChunkVoxelTerrain();
        }
    }
}
