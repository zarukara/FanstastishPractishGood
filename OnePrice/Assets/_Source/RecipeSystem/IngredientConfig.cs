using UnityEngine;

namespace RecipeSystem
{
    [CreateAssetMenu(fileName = "Ingredient", menuName = "Coffee Shop/Ingredient")]
    public sealed class IngredientConfig : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        public string DisplayName => string.IsNullOrWhiteSpace(_displayName) ? name : _displayName;
        public Sprite Icon => _icon;
    }
}
