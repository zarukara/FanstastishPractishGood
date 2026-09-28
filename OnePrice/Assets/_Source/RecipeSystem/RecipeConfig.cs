using System.Collections.Generic;
using UnityEngine;

namespace RecipeSystem
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Coffee Shop/Recipe")]
    public sealed class RecipeConfig : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private IngredientConfig[] _ingredients = new IngredientConfig[0];
        [SerializeField, Min(1)] private int _reward = 15;
        [SerializeField, Min(1f)] private float _waitSeconds = 30f;

        public string DisplayName => string.IsNullOrWhiteSpace(_displayName) ? name : _displayName;
        public Sprite Icon => _icon;
        public IReadOnlyList<IngredientConfig> Ingredients => _ingredients;
        public int Reward => Mathf.Max(1, _reward);
        public float WaitSeconds => Mathf.Max(1f, _waitSeconds);
    }
}
