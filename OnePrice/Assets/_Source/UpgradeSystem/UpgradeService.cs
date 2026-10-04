using CoreSystem;
using SaveSystem;
using UnityEngine;
namespace UpgradeSystem
{
    public sealed class UpgradeService
    {
        private readonly UpgradeCatalog _catalog;
        private readonly SaveService _save;
        private readonly GameStateMachine _state;
        public UpgradeService(UpgradeCatalog catalog, SaveService save, GameStateMachine state)
        { _catalog = catalog; _save = save; _state = state; }
        public int Level(UpgradeConfig config) => Mathf.Clamp(_save.GetLevel(config.Id), 0, config.MaxLevel);
        public int Price(UpgradeConfig config) => Level(config) < config.MaxLevel ? config.Prices[Level(config)] : 0;
        public bool CanBuy(UpgradeConfig config) => _state.Current == GamePhase.Upgrades &&
            Contains(config) && Price(config) > 0 && _save.Wallet >= Price(config);
        public bool TryBuy(UpgradeConfig config) => CanBuy(config) &&
            _save.TryPurchase(config.Id, Price(config), config.MaxLevel);
        public float Multiplier(UpgradeEffect effect)
        {
            float result = 1f;
            foreach (var config in _catalog.Upgrades)
                if (config != null && config.Effect == effect) result += Level(config) * config.EffectPerLevel;
            return result;
        }
        private bool Contains(UpgradeConfig config)
        {
            if (config == null) return false;
            foreach (var item in _catalog.Upgrades) if (item == config) return true;
            return false;
        }
    }
}
