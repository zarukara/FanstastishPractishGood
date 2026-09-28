using OrderSystem;
using PreparationSystem;
using RecipeSystem;
using UISystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CoreSystem
{
    [DisallowMultipleComponent]
    public sealed class Bootstrapper : LifetimeScope
    {
        [SerializeField] private RecipeCatalog _recipeCatalog;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_recipeCatalog == null)
            {
                throw new System.InvalidOperationException(
                    "Assign RecipeCatalog_Main to Bootstrapper.");
            }

            builder.RegisterInstance(_recipeCatalog);
            builder.RegisterInstance(new System.Random());

            builder.Register<RecipeMatcher>(Lifetime.Scoped);
            builder.Register<OrderFactory>(Lifetime.Scoped);
            builder.Register<OrderService>(Lifetime.Scoped);
            builder.Register<DrinkPreparationService>(Lifetime.Scoped);
            builder.Register<GameStateMachine>(Lifetime.Scoped);

            builder.RegisterComponentInHierarchy<OrderView>();

            builder.RegisterEntryPoint<GameSession>();
            builder.RegisterEntryPoint<OrderPresenter>();
            builder.RegisterComponentInHierarchy<IngredientsPanelView>();
            builder.RegisterEntryPoint<IngredientsPresenter>();
        }
    }
}