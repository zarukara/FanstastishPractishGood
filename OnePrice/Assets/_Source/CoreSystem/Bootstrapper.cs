using OrderSystem;
using PreparationSystem;
using RecipeSystem;
using SaveSystem;
using SettingsSystem;
using UISystem;
using UpgradeSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace CoreSystem
{
    [DisallowMultipleComponent]
    public sealed class Bootstrapper : LifetimeScope
    {
        [SerializeField] private RecipeCatalog _recipeCatalog;
        [SerializeField] private DayConfig _dayConfig;
        [SerializeField] private UpgradeCatalog _upgradeCatalog;
        protected override void Configure(IContainerBuilder builder)
        {
            if (_recipeCatalog == null || _dayConfig == null || _upgradeCatalog == null)
                throw new System.InvalidOperationException("Open Assets/_Presentation/Scenes/CoffeeShopGame.unity; Bootstrapper needs all three configs.");
            builder.RegisterInstance(_recipeCatalog);
            builder.RegisterInstance(_dayConfig);
            builder.RegisterInstance(_upgradeCatalog);
            builder.RegisterInstance(new System.Random());
            builder.RegisterInstance<ISaveStore>(new PlayerPrefsSaveStore());
            builder.Register<SaveService>(Lifetime.Scoped);
            builder.Register<SettingsService>(Lifetime.Scoped);
            builder.Register<UpgradeService>(Lifetime.Scoped);
            builder.Register<RecipeMatcher>(Lifetime.Scoped);
            builder.Register<OrderFactory>(Lifetime.Scoped);
            builder.Register<OrderService>(Lifetime.Scoped);
            builder.Register<DrinkPreparationService>(Lifetime.Scoped);
            builder.Register<GameStateMachine>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<OrderView>();
            builder.RegisterComponentInHierarchy<IngredientsPanelView>();
            builder.RegisterComponentInHierarchy<GameView>();
            builder.RegisterEntryPoint<GameSession>().AsSelf();
            builder.RegisterEntryPoint<OrderPresenter>();
            builder.RegisterEntryPoint<IngredientsPresenter>();
            builder.RegisterEntryPoint<GamePresenter>();
        }
    }
}
