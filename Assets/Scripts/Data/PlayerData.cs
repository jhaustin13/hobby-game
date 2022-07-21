using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerData
{
    public Vector3 Position;

    public InventoryData InventoryData;



    public PlayerData()
    {
        InventoryData = new InventoryData(10,10,4);
    }
}

