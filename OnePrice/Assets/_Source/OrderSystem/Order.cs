using RecipeSystem;
using UnityEngine;

namespace OrderSystem
{
    public sealed class Order
    {
        public RecipeConfig Recipe { get; }
        public float RemainingSeconds { get; private set; }
        public bool IsExpired => RemainingSeconds <= 0f;

        public Order(RecipeConfig recipe, float waitSeconds)
        {
            Recipe = recipe != null ? recipe : throw new System.ArgumentNullException(nameof(recipe));
            RemainingSeconds = Mathf.Max(0f, waitSeconds);
        }

        public void Tick(float deltaSeconds)
        {
            if (deltaSeconds < 0f)
            {
                throw new System.ArgumentOutOfRangeException(nameof(deltaSeconds));
            }

            RemainingSeconds = Mathf.Max(0f, RemainingSeconds - deltaSeconds);
        }
    }
}
