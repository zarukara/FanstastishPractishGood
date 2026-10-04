using UnityEngine;

namespace SaveSystem
{
    public sealed class PlayerPrefsSaveStore : ISaveStore
    {
        public const string DefaultKey = "OnePrice.CoffeeShop.Save.v1";
        private readonly string _key;
        public PlayerPrefsSaveStore(string key = DefaultKey) => _key = key;
        public string Read() => PlayerPrefs.GetString(_key, string.Empty);
        public void Write(string json) { PlayerPrefs.SetString(_key, json); PlayerPrefs.Save(); }
    }
}
