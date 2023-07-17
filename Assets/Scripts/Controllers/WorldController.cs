using Assets.Scripts.Controllers;
using Assets.Scripts.Data;
using Assets.Scripts.Helpers;
using Assets.Scripts.ItemUIControllers;
using Assets.Scripts.ResourceManagement;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class WorldController : MonoBehaviour
{
    public int SizeX;
    public int SizeY;
    public int SizeZ;

    public GameObject chunkPrefab;

    private int selectedState = 0;
    private int numOfTrees = 5;

    public ChunkController[,,] Chunks;
    public WorldData worldData;

    public ChunkLoader ChunkLoader;

    public SimpleObjectPool ChunkPool;

    public FastNoise FastNoise;

    public Vector3 Spawn;

    private enum VFace
    {
        None,
        Up,
        Down,
        East,
        West,
        North,
        South
    }

    void Awake()
    {
        Stopwatch stopwatch = new Stopwatch();
        Stopwatch segStopwatch = new Stopwatch();

        ChunkLoader = new ChunkLoader(this);

        Spawn = Vector3.one * 5;

        ChunkPool = new SimpleObjectPool();
        ChunkPool.prefab = chunkPrefab;

        FastNoise = new FastNoise();
        worldData = new WorldData(SizeX, SizeY, SizeZ, 24, .5f);
        float loadDistance = 5 * 12; //12 is the chunk size, so load distance is radius of 5 chunks or so
        //World Data only should be allocating memory for the chunks closest to the spawn location
        //instead of creating everything here we need to prioritize creating chunks closest to the player
        //only blocking at the beginning to load the chunks underneath the player or the spawn location

        //Okay so next steps
        //Memory allocation and object instantiating need to happen on the made thread
        //So all remaining chunks that need memory to be allocated and initialized should be
        //moved into a priority queue that will execute for some time during the update
        
        //The voxel setting and the mesh tasks should be attempted to be moved off of the main thread
        //and into the async parallel process, the only issue will be making sure that the mesh refresh call
        //only gets queued when the surrounding chunks have been initialized, so either we need a list of every chunk
        //or something that checks chunks that aren't ready and tries to put them on the queue to be processed


        //FastNoise.SetFrequency(100);

        segStopwatch.Start();
        stopwatch.Start();
        Chunks = new ChunkController[SizeX, SizeY, SizeZ];
        int initializedChunks = 0;
        for (int y = 0; y < SizeY; ++y)
        {
            for (int z = 0; z < SizeZ; ++z)
            {
                for (int x = 0; x < SizeX; ++x)
                {
                    float distanceFromSpawn = float.MaxValue;

                    if (worldData.Chunks[x,y,z] != null)
                    {
                        distanceFromSpawn = Vector3.Distance(worldData.Chunks[x, y, z].Position, Spawn);
                    }

                    if (worldData.Chunks[x, y, z] != null && distanceFromSpawn < loadDistance)
                    {
                        //worldData.Chunks[x, y, z].InitializeVoxels();

                        //GameObject newChunk = ChunkPool.GetObject();
                        //ChunkController chunkController = newChunk.GetComponent<ChunkController>();
                        //chunkController.transform.parent = transform;

                        //chunkController.Initialize(worldData.Chunks[x, y, z], x, y, z);

                        //Chunks[x, y, z] = chunkController;
                        //Chunks[x, y, z].SetChunkVoxelTerrain();
                        ChunkLoader.LoadChunk(worldData.Chunks[x, y, z], -1);
                        initializedChunks++;
                    }
                    else 
                    {
                        ChunkLoader.ChunkQueue.Enqueue(worldData.Chunks[x, y, z], distanceFromSpawn);
                    }


                }
            }
        }

        segStopwatch.Stop();
        Debug.Log($"Took {segStopwatch.ElapsedMilliseconds / 1000} seconds to instantiate {SizeX * SizeY * SizeZ} chunk controllers.");

        Debug.Log($"Chunks initialized {initializedChunks}");

        Debug.Log($"Chunks queueed {ChunkLoader.ChunkQueue.Count}");
       // segStopwatch.Restart();


        //for (int y = 0; y < SizeY; ++y)
        //{
        //    for (int z = 0; z < SizeZ; ++z)
        //    {
        //        for (int x = 0; x < SizeX; ++x)
        //        {
                    
        //        }
        //    }
        //}

        //segStopwatch.Stop();
        //Debug.Log($"Took {segStopwatch.ElapsedMilliseconds / 1000} seconds to set {SizeX * SizeY * SizeZ} chunks voxel terrain.");
        segStopwatch.Restart();

        //List<ChunkController> chunks = new List<ChunkController>();

        for (int y = 0; y < SizeY; ++y)
        {
            for (int z = 0; z < SizeZ; ++z)
            {
                for (int x = 0; x < SizeX; ++x)
                {                    
                    if(Chunks[x,y,z] != null)
                    {
                        if (Chunks[x,y,z].ChunkData.Position == new Vector3(24.00f, 0.00f, 60.00f))
                        {
                            //Debug.Log("This chunk breaks for some reason");
                        }

                        if(Chunks[x, y, z].IsReadyForRefresh())
                        {
                            Chunks[x, y, z].RefreshChunkMesh();
                        }
                        else
                        {
                            if (Chunks[x, y, z].State == ChunkController.ChunkState.Setup)
                            {
                                ChunkLoader.ChunkCheckQueue.Add(Chunks[x, y, z]);
                            }
                        }
                    }

                    
                  
                    
                    
                    //MeshFilter meshFilter = Chunks[x, y, z].GetComponent<MeshFilter>();
                    //if (meshFilter != null && meshFilter.mesh.vertexCount > 0)
                    //{
                    //    chunks.Add(Chunks[x, y, z]);
                    //}
                }
            }
        }

        segStopwatch.Stop();
        Debug.Log($"Took {segStopwatch.ElapsedMilliseconds / 1000} seconds to refresh {SizeX * SizeY * SizeZ} chunks mesh.");
        stopwatch.Stop();
        Debug.Log($"Took {stopwatch.ElapsedMilliseconds / 1000} seconds to generate {SizeX * SizeY * SizeZ} chunks.");

        stopwatch.Restart();
        //for (int i = 0; i < 10; ++i)
        //{
        //    //int randomChunk = Mathf.FloorToInt(Random.Range(0, chunks.Count));

        //    var vertices = Chunks[0,0,4].GetComponent<MeshFilter>().mesh.vertices;

        //    int randomVertex = Mathf.FloorToInt(Random.Range(0, vertices.Length));

        //    GameObject tree = Instantiate(Resources.Load<GameObject>("Trees/Test Tree"));
        //    tree.transform.parent = Chunks[0, 0, 4].transform;
        //    tree.transform.localPosition = vertices[randomVertex];
        //    tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        //}
        stopwatch.Stop();
        Debug.Log($"Took {stopwatch.ElapsedMilliseconds / 1000} seconds to generate {numOfTrees} trees.");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Coordinate GetChunkIndex(ChunkController chunkController)
    {
        Vector3 vPosition = chunkController.transform.localPosition;
        float offset = chunkController.ChunkData.Resolution * chunkController.ChunkData.VoxelSize;

        return new Coordinate((int)(vPosition.x / offset), (int)(vPosition.y / offset), (int)(vPosition.z / offset));
    }



    // Update is called once per frame
    void Update()
    {
        if(ChunkLoader.ChunkQueue.Count > 0)
        {
            Debug.Log($"Processing Chunk Queue {ChunkLoader.ChunkQueue.Count} Chunks");
            ChunkLoader.ProcessChunkQueue();
        }
        

        //ChunkLoader.StartProcessor();
    }   

    public ChunkController GetChunkRelativeToChunk(ChunkController originChunk, Coordinate offset)
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

    public WorldItemController PlaceItemInWorld(Vector3 position, Quaternion rotation, ChunkController chunkController, InventoryItemData inventoryItemData)
    {
        var itemInfo = ResourceCache.Instance.GetItemInfo(inventoryItemData.Id);

        // var itemPosition = position + new Vector3(0, itemInfo.Bounds.extents.y * itemInfo.ItemPrefab.transform.localScale.y, 0) ;
        var itemPosition = position;
        WorldItemData newItem = new WorldItemData(inventoryItemData, itemPosition, rotation, chunkController.ChunkData);

        var placeableItem = Instantiate(itemInfo.ItemPrefab,itemPosition, rotation, chunkController.transform);      
        
        var worldItemController = placeableItem.GetComponent<WorldItemController>();
        
        
        if (worldItemController == null)
        {         
            worldItemController = placeableItem.AddComponent<WorldItemController>();
            worldItemController.Initialize(newItem);
        }
        else if (worldItemController != null && worldItemController.GetType() == typeof(InventoryWorldItemController))
        {
            if (newItem.Attributes.Contains(Attributes.Slot9Inventory))
            {
                //worldItemController = placeableItem.AddComponent<InventoryWorldItemController>();
                newItem = new InventoryWorldItemData(inventoryItemData, newItem.Position, newItem.Rotation, newItem.ParentChunk, 9);
                ((InventoryWorldItemController)worldItemController).Initialize((InventoryWorldItemData)newItem);
            }
        }

        if(newItem.Attributes.Contains(Attributes.UIInteractable))
        {
            var itemUIcontroller = placeableItem.GetComponent<ItemUIController>();

            if(itemUIcontroller == null)
            {
                itemUIcontroller = placeableItem.AddComponent<ItemUIController>();
            }

            itemUIcontroller.Initialize();
        }
        

        chunkController.ChunkData.Items.Add(newItem);

        return worldItemController;
    }

    private VFace GetHitFace(RaycastHit hit)
    {
        Vector3 incomingVec = hit.normal - Vector3.up;

        if (incomingVec == new Vector3(0, -1, -1))
            return VFace.South;

        if (incomingVec == new Vector3(0, -1, 1))
            return VFace.North;

        if (incomingVec == new Vector3(0, 0, 0))
            return VFace.Up;

        if (incomingVec == new Vector3(1, 1, 1))
            return VFace.Down;

        if (incomingVec == new Vector3(-1, -1, 0))
            return VFace.West;

        if (incomingVec == new Vector3(1, -1, 0))
            return VFace.East;

        return VFace.None;
    }
}
