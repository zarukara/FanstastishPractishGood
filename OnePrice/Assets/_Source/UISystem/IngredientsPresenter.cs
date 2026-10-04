using System;
using CoreSystem;
using RecipeSystem;
using VContainer.Unity;

namespace UISystem
{
    public sealed class IngredientsPresenter : IStartable, IDisposable
    {
        private readonly IngredientsPanelView _view;
        private readonly GameSession _session;

        public IngredientsPresenter(
            IngredientsPanelView view,
            GameSession session)
        {
            _view = view;
            _session = session;
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
            _session.AddIngredient(ingredient);
        }
    }
}
