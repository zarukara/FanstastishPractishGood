using System;
using OrderSystem;
using VContainer.Unity;

namespace UISystem
{
    public sealed class OrderPresenter : IStartable, ITickable, IDisposable
    {
        private readonly OrderService _orderService;
        private readonly OrderView _view;

        public OrderPresenter(OrderService orderService, OrderView view)
        {
            _orderService = orderService;
            _view = view;
        }

        public void Start()
        {
            _orderService.CurrentOrderChanged += OnCurrentOrderChanged;
            OnCurrentOrderChanged(_orderService.CurrentOrder);
        }

        public void Tick()
        {
            Order order = _orderService.CurrentOrder;

            if (order != null)
            {
                _view.SetRemainingSeconds(order.RemainingSeconds);
            }
        }

        public void Dispose()
        {
            _orderService.CurrentOrderChanged -= OnCurrentOrderChanged;
        }

        private void OnCurrentOrderChanged(Order order)
        {
            if (order == null)
            {
                _view.Hide();
                return;
            }

            _view.Show(order.Recipe.DisplayName);
            _view.SetRemainingSeconds(order.RemainingSeconds);
        }
    }
}