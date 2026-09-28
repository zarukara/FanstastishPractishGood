using System.Collections.Generic;
using UnityEngine;

namespace RecipeSystem
{
    [CreateAssetMenu(fileName = "RecipeCatalog", menuName = "Coffee Shop/Recipe Catalog")]
    public sealed class RecipeCatalog : ScriptableObject
    {
        [SerializeField] private RecipeConfig[] _recipes = new RecipeConfig[0];

        public IReadOnlyList<RecipeConfig> Recipes => _recipes;
    }
}
