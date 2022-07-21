using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class HotbarInventoryData : BaseInventoryData
    {

        public int SelectedIndex { get; private set; }

        public HotbarInventoryData(int size) : base(size)
        {

        }

      

        public int SetSelectedIndex(int index)
        {
            if(index >= 0 && index < Size)
            {
                SelectedIndex = index;                
            }

            return SelectedIndex;
        }

       

       
    }
}
