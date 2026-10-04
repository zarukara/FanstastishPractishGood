using System;
using System.Collections.Generic;
namespace SaveSystem

{
    [Serializable] public sealed class UpgradeLevel { public string id; public int level; }
    
    [Serializable]
    public sealed class SaveData
    {
        public int version = 1;
        public int wallet;
        public int bestRevenue;
        public int bestOrders;
        public List<UpgradeLevel> upgrades = new List<UpgradeLevel>();
        public float volume = .65f;
        public bool fullscreen;
    }
}
