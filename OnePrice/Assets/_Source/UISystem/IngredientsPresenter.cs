using System;
using PreparationSystem;
using RecipeSystem;
using UnityEngine;
using VContainer.Unity;

namespace UISystem
{
    public sealed class IngredientsPresenter : IStartable, IDisposable
    {
        private readonly IngredientsPanelView _view;
        private readonly DrinkPreparationService _preparationService;

        public IngredientsPresenter(
            IngredientsPanelView view,
            DrinkPreparationService preparationService)
        {
            _view = view;
            _preparationService = preparationService;
        }

        public void Start()
        {
            _view.IngredientSelected += HandleIngredientSelected;
        }

        public void Dispose()
        {
            _view.IngredientSelected -= HandleIngredientSelected;
        }

        private void HandleIngredientSelected(IngredientConfig ingredient)
        {
            if (_preparationService.TryAddIngredient(ingredient))
                Debug.Log($"Добавлен ингредиент: {ingredient.DisplayName}");
        }
    }
}