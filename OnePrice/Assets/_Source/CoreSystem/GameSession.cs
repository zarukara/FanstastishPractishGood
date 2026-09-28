using System;
using OrderSystem;
using PreparationSystem;
using UnityEngine;
using VContainer.Unity;

namespace CoreSystem
{
    public sealed class GameSession : IStartable, ITickable, IDisposable
    {
        private readonly GameStateMachine _stateMachine;
        private readonly OrderService _orderService;
        private readonly DrinkPreparationService _preparationService;

        public GameSession(
            GameStateMachine stateMachine,
            OrderService orderService,
            DrinkPreparationService preparationService)
        {
            _stateMachine = stateMachine;
            _orderService = orderService;
            _preparationService = preparationService;
        }

        public void Start()
        {
            _orderService.OrderExpired += OnOrderExpired;
            StartDay();
        }

        public void Tick()
        {
            if (_stateMachine.Current != GamePhase.Playing)
            {
                return;
            }

            float deltaSeconds = Time.deltaTime;

            _orderService.Tick(deltaSeconds);
            _preparationService.Tick(deltaSeconds);

            if (_orderService.CurrentOrder == null)
            {
                StartNextOrder();
            }
        }

        public void Dispose()
        {
            _orderService.OrderExpired -= OnOrderExpired;
        }

        public void StartDay()
        {
            _orderService.Clear();
            _preparationService.Reset();

            _stateMachine.StartDay();
            StartNextOrder();
        }

        private void StartNextOrder()
        {
            Order order = _orderService.StartNextOrder();

            Debug.Log(
                $"New order: {order.Recipe.DisplayName}, " +
                $"{order.RemainingSeconds:0} seconds");
        }

        private void OnOrderExpired(Order order)
        {
            _preparationService.Reset();
            Debug.Log($"Order expired: {order.Recipe.DisplayName}");
        }
    }
}