using System;

namespace OrderSystem
{
    public sealed class OrderService
    {
        private readonly OrderFactory _orderFactory;

        public event Action<Order> CurrentOrderChanged;
        public event Action<Order> OrderExpired;

        public Order CurrentOrder { get; private set; }

        public OrderService(OrderFactory orderFactory)
        {
            _orderFactory = orderFactory
                ?? throw new ArgumentNullException(nameof(orderFactory));
        }

        public Order StartNextOrder(float waitMultiplier = 1f)
        {
            if (CurrentOrder != null)
            {
                throw new InvalidOperationException(
                    "Finish the current order before starting another.");
            }

            CurrentOrder = _orderFactory.Create(waitMultiplier);
            CurrentOrderChanged?.Invoke(CurrentOrder);

            return CurrentOrder;
        }

        public void Tick(float deltaSeconds)
        {
            if (deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            }

            if (CurrentOrder == null)
            {
                return;
            }

            CurrentOrder.Tick(deltaSeconds);

            if (!CurrentOrder.IsExpired)
            {
                return;
            }

            Order expiredOrder = CurrentOrder;
            CurrentOrder = null;

            CurrentOrderChanged?.Invoke(null);
            OrderExpired?.Invoke(expiredOrder);
        }

        public bool TryTakeCurrentOrder(out Order order)
        {
            order = CurrentOrder;

            if (order == null || order.IsExpired)
            {
                order = null;
                return false;
            }

            CurrentOrder = null;
            CurrentOrderChanged?.Invoke(null);

            return true;
        }

        public void Clear()
        {
            if (CurrentOrder == null)
            {
                return;
            }

            CurrentOrder = null;
            CurrentOrderChanged?.Invoke(null);
        }
    }
}