using System.Collections.Generic;

namespace RecipeSystem
{
    /// <summary>Compares ingredients as a multiset: their order does not matter.</summary>
    public sealed class RecipeMatcher
    {
        public bool Matches(RecipeConfig recipe, IReadOnlyList<IngredientConfig> selected)
        {
            if (recipe == null || selected == null || recipe.Ingredients == null)
            {
                return false;
            }

            IReadOnlyList<IngredientConfig> required = recipe.Ingredients;
            if (required.Count < 2 || required.Count != selected.Count)
            {
                return false;
            }

            bool[] consumed = new bool[selected.Count];

            for (int i = 0; i < required.Count; i++)
            {
                IngredientConfig ingredient = required[i];
                if (ingredient == null)
                {
                    return false;
                }

                bool found = false;
                for (int j = 0; j < selected.Count; j++)
                {
                    if (!consumed[j] && selected[j] == ingredient)
                    {
                        consumed[j] = true;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
