using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class AsyncParallelProcessor
    {
        private SimplePriorityQueue<Action> priorityQueue;
        private int NumberOfThreads;

        public AsyncParallelProcessor(int numberOfThreads)
        {
            priorityQueue = new SimplePriorityQueue<Action>();
            NumberOfThreads = numberOfThreads;           
        }

        public void QueueTask(Action action, float priority)
        {
            priorityQueue.Enqueue(action, priority);
        }

        public void ProcessQueue()
        {
            //Debug.Log($"Parallel Queue size {priorityQueue.Count}");
            if (priorityQueue.Count == 0) return;

            Action action;
            while(priorityQueue.Count > 0)
            {
                if (priorityQueue.TryDequeue(out action))
                {
                    ThreadPool.QueueUserWorkItem(state => action(), null); // Execute the action
                }
            }           
        }

    
    }
}
