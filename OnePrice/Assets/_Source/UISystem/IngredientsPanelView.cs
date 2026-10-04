using System;
using System.Collections.Generic;
using RecipeSystem;
using UnityEngine;

namespace UISystem
{
    public sealed class IngredientsPanelView : MonoBehaviour
    {
        [SerializeField] private RecipeCatalog _recipeCatalog;
        [SerializeField] private RectTransform _container;
        [SerializeField] private IngredientButtonView _buttonPrefab;

        private readonly List<IngredientButtonView> _buttons = new();

        public event Action<IngredientConfig> IngredientSelected;

        public void SetInteractable(bool value)
        {
            foreach (var button in _buttons) button.SetInteractable(value);
        }

        private void Awake()
        {
            if (_recipeCatalog == null || _container == null || _buttonPrefab == null)
            {
                Debug.LogError("IngredientsPanelView: заполни все ссылки в Inspector.", this);
                return;
            }

            var addedIngredients = new HashSet<IngredientConfig>();

            foreach (RecipeConfig recipe in _recipeCatalog.Recipes)
            {
                if (recipe == null)
                    continue;

                foreach (IngredientConfig ingredient in recipe.Ingredients)
                {
                    if (ingredient == null || !addedIngredients.Add(ingredient))
                        continue;

                    IngredientButtonView button = Instantiate(_buttonPrefab, _container);
                    button.Bind(ingredient);
                    button.Clicked += HandleIngredientClicked;
                    _buttons.Add(button);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (IngredientButtonView button in _buttons)
            {
                if (button != null)
                    button.Clicked -= HandleIngredientClicked;
            }
        }

        private void HandleIngredientClicked(IngredientConfig ingredient)
        {
            IngredientSelected?.Invoke(ingredient);
        }
    }
}
