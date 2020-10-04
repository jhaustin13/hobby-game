using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class UIManager : Singleton<UIManager>
{
    public GameObject Player;
    public GameObject Hotbar;
    
    void Start()
    {
        //Need to get player data and pass it to the hotbar
        //However it doesn't make sense of the UI Manager to have a reference of the player... or does it
        HotbarController hbController = Hotbar.GetComponent<HotbarController>();
        PlayerController playerController = Player.GetComponent<PlayerController>();

        hbController.Initialize(playerController.PlayerData);
    }


    public void RefreshHotbar()
    {
        HotbarController hotbarController = Hotbar.GetComponent<HotbarController>();

        hotbarController.RefreshUI();
    }
}
