using System.Collections.Generic;
using UnityEngine;
namespace UpgradeSystem
{
    public enum UpgradeEffect { PreparationSpeed, OrderReward, CustomerPatience }
    [CreateAssetMenu(fileName = "Upgrade", menuName = "Coffee Shop/Upgrade")]
    public sealed class UpgradeConfig : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea] private string _description;
        [SerializeField] private UpgradeEffect _effect;
        [SerializeField] private int[] _prices = { 45, 90, 150 };
        [SerializeField, Min(.01f)] private float _effectPerLevel = .25f;
        public string Id => _id;
        public string DisplayName => _displayName;
        public string Description => _description;
        public UpgradeEffect Effect => _effect;
        public IReadOnlyList<int> Prices => _prices;
        public int MaxLevel => _prices?.Length ?? 0;
        public float EffectPerLevel => Mathf.Max(.01f, _effectPerLevel);
    }
}
