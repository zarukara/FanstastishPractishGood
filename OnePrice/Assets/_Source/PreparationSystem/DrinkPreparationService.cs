using System;
using System.Collections.Generic;
using RecipeSystem;

namespace PreparationSystem
{
    public sealed class DrinkPreparationService
    {
        private readonly List<IngredientConfig> _ingredients =
            new List<IngredientConfig>();

        private readonly IReadOnlyList<IngredientConfig> _ingredientsView;

        public event Action Changed;

        public PreparationPhase Phase { get; private set; } =
            PreparationPhase.Selecting;

        public float RemainingSeconds { get; private set; }

        public IReadOnlyList<IngredientConfig> Ingredients => _ingredientsView;

        public DrinkPreparationService()
        {
            _ingredientsView = _ingredients.AsReadOnly();
        }

        public bool TryAddIngredient(IngredientConfig ingredient)
        {
            if (Phase != PreparationPhase.Selecting || ingredient == null)
            {
                return false;
            }

            _ingredients.Add(ingredient);
            Changed?.Invoke();
            return true;
        }

        public bool TryRemoveLastIngredient()
        {
            if (Phase != PreparationPhase.Selecting ||
                _ingredients.Count == 0)
            {
                return false;
            }

            _ingredients.RemoveAt(_ingredients.Count - 1);
            Changed?.Invoke();
            return true;
        }

        public bool TryStart(float durationSeconds)
        {
            if (Phase != PreparationPhase.Selecting ||
                _ingredients.Count == 0 ||
                durationSeconds <= 0f ||
                float.IsNaN(durationSeconds) ||
                float.IsInfinity(durationSeconds))
            {
                return false;
            }

            RemainingSeconds = durationSeconds;
            Phase = PreparationPhase.Preparing;
            Changed?.Invoke();
            return true;
        }

        public void Tick(float deltaSeconds)
        {
            if (deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            }

            if (Phase != PreparationPhase.Preparing)
            {
                return;
            }

            RemainingSeconds = Math.Max(
                0f, RemainingSeconds - deltaSeconds);

            if (RemainingSeconds == 0f)
            {
                Phase = PreparationPhase.Ready;
            }

            Changed?.Invoke();
        }

        public bool TryCollect(out IngredientConfig[] drink)
        {
            if (Phase != PreparationPhase.Ready)
            {
                drink = null;
                return false;
            }

            drink = _ingredients.ToArray();
            Reset();
            return true;
        }

        public void Reset()
        {
            _ingredients.Clear();
            RemainingSeconds = 0f;
            Phase = PreparationPhase.Selecting;
            Changed?.Invoke();
        }
    }
}