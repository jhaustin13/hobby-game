using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WorldItemData
{
    public string Name { get; protected set; }
    public string Id { get; protected set; }
    public Vector3 Position { get; set; }

    public Quaternion Rotation { get; set; }

    public ChunkData ParentChunk { get; set; }

    public List<string> Attributes { get; protected set; }

    public WorldItemData(InventoryItemData inventoryItemData, Vector3 position, Quaternion rotation, ChunkData parentChunk)
    {
        Name = inventoryItemData.Name;
        Id = inventoryItemData.Id;
        Position = position;
        Rotation = rotation;
        ParentChunk = parentChunk;
        

        Attributes = inventoryItemData.Attributes;
        //Probably use a different texture when its in the world

    }
}

