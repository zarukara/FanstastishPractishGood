using System;
using System.Collections.Generic;
using RecipeSystem;

namespace OrderSystem
{
    public sealed class OrderFactory
    {
        private readonly List<RecipeConfig> _availableRecipes = new List<RecipeConfig>();
        private readonly Random _random;

        public OrderFactory(RecipeCatalog catalog, Random random = null)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            foreach (RecipeConfig recipe in catalog.Recipes)
            {
                if (IsValid(recipe))
                {
                    _availableRecipes.Add(recipe);
                }
            }

            if (_availableRecipes.Count == 0)
            {
                throw new ArgumentException("The catalog must contain at least one valid recipe.", nameof(catalog));
            }

            _random = random ?? new Random();
        }

        public Order Create(float waitMultiplier = 1f)
        {
            RecipeConfig recipe = _availableRecipes[_random.Next(_availableRecipes.Count)];
            return new Order(recipe, recipe.WaitSeconds * Math.Max(0.1f, waitMultiplier));
        }

        private static bool IsValid(RecipeConfig recipe)
        {
            if (recipe == null || recipe.Ingredients == null || recipe.Ingredients.Count < 2)
            {
                return false;
            }

            foreach (IngredientConfig ingredient in recipe.Ingredients)
            {
                if (ingredient == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
