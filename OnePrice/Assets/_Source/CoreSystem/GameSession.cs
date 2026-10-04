using System;
using OrderSystem;
using PreparationSystem;
using RecipeSystem;
using SaveSystem;
using UpgradeSystem;
using UnityEngine;
using VContainer.Unity;
namespace CoreSystem
{
    public sealed class GameSession : IStartable, ITickable, IDisposable
    {
        private readonly GameStateMachine _state;
        private readonly OrderService _orders;
        private readonly DrinkPreparationService _preparation;
        private readonly RecipeMatcher _matcher;
        private readonly DayConfig _config;
        private readonly UpgradeService _upgrades;
        private readonly SaveService _save;
        private bool _started;
        public event Action<bool> OrderResolved;
        public float RemainingSeconds { get; private set; }
        public int Revenue { get; private set; }
        public int Reputation { get; private set; }
        public int CompletedOrders { get; private set; }
        public string Status { get; private set; } = "Добро пожаловать в One Price";
        public int RevenueGoal => _config.RevenueGoal;
        public float PreparationDuration => _config.PreparationSeconds / _upgrades.Multiplier(UpgradeEffect.PreparationSpeed);
        public bool CanSelect => _state.Current == GamePhase.Playing && _orders.CurrentOrder != null &&
            _preparation.Phase == PreparationPhase.Selecting;
        public bool CanPrepare => CanSelect && _preparation.Ingredients.Count > 0;
        public bool CanServe => _state.Current == GamePhase.Playing && _orders.CurrentOrder != null &&
            !_orders.CurrentOrder.IsExpired && _preparation.Phase == PreparationPhase.Ready;
        public GameSession(GameStateMachine state, OrderService orders, DrinkPreparationService preparation,
            RecipeMatcher matcher, DayConfig config, UpgradeService upgrades, SaveService save)
        {
            _state = state; _orders = orders; _preparation = preparation; _matcher = matcher;
            _config = config; _upgrades = upgrades; _save = save;
        }
        public void Start()
        {
            if (_started) return;
            _started = true;
            _orders.OrderExpired += OnOrderExpired;
        }
        public void Dispose() { _orders.OrderExpired -= OnOrderExpired; _started = false; }
        public void Tick() => Advance(Time.deltaTime);
        public void Advance(float deltaSeconds)
        {
            if (deltaSeconds < 0 || float.IsNaN(deltaSeconds) || float.IsInfinity(deltaSeconds))
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            if (_state.Current != GamePhase.Playing) return;
            float elapsed = Mathf.Min(deltaSeconds, RemainingSeconds);
            RemainingSeconds = Mathf.Max(0, RemainingSeconds - elapsed);
            _orders.Tick(elapsed); // Expiration clears brewing before preparation can complete.
            if (_state.Current != GamePhase.Playing) return;
            if (RemainingSeconds <= 0) { Finish(false, "Время дня вышло. Попробуйте обслужить гостей быстрее."); return; }
            _preparation.Tick(elapsed);
            if (_orders.CurrentOrder == null) StartNextOrder();
        }
        public void StartDay()
        {
            if (_state.Current == GamePhase.Playing || _state.Current == GamePhase.Paused ||
                _state.Current == GamePhase.Settings) return;
            ClearOrder();
            RemainingSeconds = _config.DurationSeconds;
            Revenue = 0; CompletedOrders = 0; Reputation = _config.StartingReputation;
            Status = "Выберите состав по чеку, приготовьте и выдайте напиток.";
            _state.StartDay();
            StartNextOrder();
        }
        public bool AddIngredient(IngredientConfig ingredient) => CanSelect && _preparation.TryAddIngredient(ingredient);
        public bool RemoveLastIngredient() => CanSelect && _preparation.TryRemoveLastIngredient();
        public bool Prepare()
        {
            if (!CanPrepare || !_preparation.TryStart(PreparationDuration)) return false;
            Status = "Готовим напиток. Состав зафиксирован.";
            return true;
        }
        public bool Serve()
        {
            if (!CanServe || !_orders.TryTakeCurrentOrder(out Order order)) return false;
            if (!_preparation.TryCollect(out IngredientConfig[] drink)) return false;
            bool correct = _matcher.Matches(order.Recipe, drink);
            if (correct)
            {
                int reward = Mathf.RoundToInt(order.Recipe.Reward * _upgrades.Multiplier(UpgradeEffect.OrderReward));
                Revenue += reward; CompletedOrders++;
                _save.RecordOrder(reward, Revenue, CompletedOrders);
                Status = $"Спасибо! +{reward} монет. Заказ выполнен.";
            }
            else
            {
                Reputation = Mathf.Max(0, Reputation - _config.WrongDrinkPenalty);
                Status = $"Неверный состав: −{_config.WrongDrinkPenalty} репутации.";
            }
            OrderResolved?.Invoke(correct);
            if (Reputation <= 0) Finish(false, "Репутация исчерпана. Гости больше не приходят.");
            else if (Revenue >= RevenueGoal) Finish(true, "Цель по выручке достигнута. Отличная смена!");
            // Next order arrives next tick, so repeated clicks cannot consume it.
            return true;
        }
        public void ReturnToMenu() { ClearOrder(); _state.ReturnToMenu(); }
        private void StartNextOrder() => _orders.StartNextOrder(_upgrades.Multiplier(UpgradeEffect.CustomerPatience));
        private void OnOrderExpired(Order order)
        {
            _preparation.Reset();
            Reputation = Mathf.Max(0, Reputation - _config.ExpiredOrderPenalty);
            Status = $"Гость не дождался: −{_config.ExpiredOrderPenalty} репутации.";
            OrderResolved?.Invoke(false);
            if (Reputation <= 0) Finish(false, "Репутация исчерпана. Гости больше не приходят.");
        }
        private void Finish(bool success, string message)
        {
            ClearOrder(); Status = message;
            if (success) _state.Succeed(); else _state.Fail();
        }
        private void ClearOrder() { _preparation.Reset(); _orders.Clear(); }
    }
}
