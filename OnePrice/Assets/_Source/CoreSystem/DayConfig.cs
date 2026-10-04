using UnityEngine;

namespace CoreSystem
{
    [CreateAssetMenu(fileName = "DayConfig", menuName = "Coffee Shop/Day")]
    public sealed class DayConfig : ScriptableObject
    {
        [SerializeField, Min(10)] private float _durationSeconds = 150f;
        [SerializeField, Min(1)] private int _revenueGoal = 120;
        [SerializeField, Min(1)] private int _startingReputation = 100;
        [SerializeField, Min(1)] private int _wrongDrinkPenalty = 25;
        [SerializeField, Min(1)] private int _expiredOrderPenalty = 20;
        [SerializeField, Min(.1f)] private float _preparationSeconds = 3.5f;
        
        public float DurationSeconds => Mathf.Max(10, _durationSeconds);
        public int RevenueGoal => Mathf.Max(1, _revenueGoal);
        public int StartingReputation => Mathf.Max(1, _startingReputation);
        public int WrongDrinkPenalty => Mathf.Max(1, _wrongDrinkPenalty);
        public int ExpiredOrderPenalty => Mathf.Max(1, _expiredOrderPenalty);
        public float PreparationSeconds => Mathf.Max(.1f, _preparationSeconds);
    }
}
