using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    public sealed class SaveService
    {
        private readonly ISaveStore _store;
        private readonly SaveData _data;
        
        public event Action Changed;
        
        public int Wallet => _data.wallet;
        public int BestRevenue => _data.bestRevenue;
        public int BestOrders => _data.bestOrders;
        public float Volume => _data.volume;
        public bool Fullscreen => _data.fullscreen;
        
        public SaveService(ISaveStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _data = Decode(store.Read());
        }
        
        public int GetLevel(string id) => _data.upgrades.Find(x => x.id == id)?.level ?? 0;
        public void RecordOrder(int reward, int revenue, int orders)
        {
            if (reward <= 0) throw new ArgumentOutOfRangeException(nameof(reward));
            _data.wallet = (int)Math.Min(int.MaxValue, (long)_data.wallet + reward);
            _data.bestRevenue = Math.Max(_data.bestRevenue, revenue);
            _data.bestOrders = Math.Max(_data.bestOrders, orders);
            Persist();
        }
        
        public bool TryPurchase(string id, int price, int maxLevel)
        {
            if (string.IsNullOrWhiteSpace(id) || price <= 0 || _data.wallet < price ||
                maxLevel <= 0 || GetLevel(id) >= maxLevel) return false;
            UpgradeLevel entry = _data.upgrades.Find(x => x.id == id);
            if (entry == null) { entry = new UpgradeLevel { id = id }; _data.upgrades.Add(entry); }
            _data.wallet -= price;
            entry.level++;
            Persist();
            return true;
        }
        
        public void SetSettings(float volume, bool fullscreen)
        {
            _data.volume = float.IsNaN(volume) ? .65f : Mathf.Clamp01(volume);
            _data.fullscreen = fullscreen;
            Persist();
        }
        
        private void Persist() { _store.Write(JsonUtility.ToJson(_data)); Changed?.Invoke(); }
        
        private static SaveData Decode(string json)
        {
            SaveData data;
            try { data = string.IsNullOrWhiteSpace(json) ? new SaveData() : JsonUtility.FromJson<SaveData>(json); }
            catch (ArgumentException) { data = new SaveData(); }
            if (data == null || data.version != 1) data = new SaveData();
            data.wallet = Math.Max(0, data.wallet);
            data.bestRevenue = Math.Max(0, data.bestRevenue);
            data.bestOrders = Math.Max(0, data.bestOrders);
            data.volume = float.IsNaN(data.volume) ? .65f : Mathf.Clamp01(data.volume);
            data.upgrades ??= new List<UpgradeLevel>();
            var seen = new HashSet<string>();
            data.upgrades.RemoveAll(x => x == null || string.IsNullOrWhiteSpace(x.id) || !seen.Add(x.id));
            foreach (var upgrade in data.upgrades) upgrade.level = Math.Max(0, upgrade.level);
            return data;
        }
    }
}
