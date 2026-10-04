using System.Collections.Generic;
using UnityEngine;
namespace UpgradeSystem
{
    [CreateAssetMenu(fileName = "UpgradeCatalog", menuName = "Coffee Shop/Upgrade Catalog")]
    public sealed class UpgradeCatalog : ScriptableObject
    {
        [SerializeField] private UpgradeConfig[] _upgrades = new UpgradeConfig[0];
        public IReadOnlyList<UpgradeConfig> Upgrades => _upgrades;
    }
}
