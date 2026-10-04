using System;
using System.Collections.Generic;
using System.Text;
using CoreSystem;
using OrderSystem;
using PreparationSystem;
using SaveSystem;
using SettingsSystem;
using UpgradeSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer.Unity;
namespace UISystem
{
    public sealed class GamePresenter : IStartable, ITickable, IDisposable
    {
        private readonly GameView _view;
        private readonly IngredientsPanelView _ingredients;
        private readonly GameSession _session;
        private readonly GameStateMachine _state;
        private readonly OrderService _orders;
        private readonly DrinkPreparationService _preparation;
        private readonly SaveService _save;
        private readonly SettingsService _settings;
        private readonly UpgradeService _upgrades;
        private readonly UpgradeCatalog _catalog;
        private readonly List<(Button button, UnityAction action)> _bindings = new();
        private readonly List<(UpgradeCardView view, UpgradeConfig config)> _cards = new();
        private readonly StringBuilder _text = new();
        private bool _started;
        public GamePresenter(GameView view, IngredientsPanelView ingredients, GameSession session,
            GameStateMachine state, OrderService orders, DrinkPreparationService preparation,
            SaveService save, SettingsService settings, UpgradeService upgrades, UpgradeCatalog catalog)
        {
            _view = view; _ingredients = ingredients; _session = session; _state = state;
            _orders = orders; _preparation = preparation; _save = save; _settings = settings;
            _upgrades = upgrades; _catalog = catalog;
        }
        public void Start()
        {
            if (_started) return;
            _started = true;
            _settings.Apply();
            Bind(_view.StartDay, _session.StartDay);
            Bind(_view.MenuUpgrades, _state.OpenUpgrades);
            Bind(_view.MenuSettings, _state.OpenSettings);
            Bind(_view.Quit, Application.Quit);
            Bind(_view.PauseGame, _state.Pause);
            Bind(_view.Resume, _state.Resume);
            Bind(_view.PauseSettings, _state.OpenSettings);
            Bind(_view.PauseMenu, _session.ReturnToMenu);
            Bind(_view.RemoveLast, () => _session.RemoveLastIngredient());
            Bind(_view.Prepare, () => _session.Prepare());
            Bind(_view.Serve, () => _session.Serve());
            Bind(_view.SettingsBack, _state.CloseSettings);
            Bind(_view.Fullscreen, _settings.ToggleFullscreen);
            Bind(_view.VolumeDown, () => _settings.SetVolume(_save.Volume - .1f));
            Bind(_view.VolumeUp, () => _settings.SetVolume(_save.Volume + .1f));
            Bind(_view.SuccessAgain, _session.StartDay);
            Bind(_view.FailureAgain, _session.StartDay);
            Bind(_view.SuccessUpgrades, _state.OpenUpgrades);
            Bind(_view.FailureUpgrades, _state.OpenUpgrades);
            Bind(_view.SuccessMenu, _session.ReturnToMenu);
            Bind(_view.FailureMenu, _session.ReturnToMenu);
            Bind(_view.UpgradesPlay, _session.StartDay);
            Bind(_view.UpgradesMenu, _session.ReturnToMenu);
            foreach (var config in _catalog.Upgrades)
            {
                var card = UnityEngine.Object.Instantiate(_view.UpgradePrefab, _view.UpgradeContainer);
                card.Title.text = config.DisplayName;
                card.Description.text = config.Description;
                Bind(card.Buy, () => _upgrades.TryBuy(config));
                _cards.Add((card, config));
            }
            _state.Changed += OnPhaseChanged;
            _session.OrderResolved += _view.PlayResult;
            OnPhaseChanged(_state.Current);
        }
        public void Tick()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (_state.Current == GamePhase.Playing) _state.Pause();
                else if (_state.Current == GamePhase.Paused) _state.Resume();
                else if (_state.Current == GamePhase.Settings) _state.CloseSettings();
            }
            Refresh();
        }
        private void Bind(Button button, Action action)
        {
            UnityAction callback = () => { action(); _view.PlayClick(); Refresh(); };
            button.onClick.AddListener(callback);
            _bindings.Add((button, callback));
        }
        private void OnPhaseChanged(GamePhase phase) { _view.Show(phase); Refresh(); }
        private void Refresh()
        {
            int seconds = Mathf.CeilToInt(_session.RemainingSeconds);
            _view.Hud.text = $"СМЕНА  {seconds / 60:00}:{seconds % 60:00}    ВЫРУЧКА  {_session.Revenue}/{_session.RevenueGoal}    КОШЕЛЁК  {_save.Wallet}    РЕПУТАЦИЯ  {_session.Reputation}    ЗАКАЗЫ  {_session.CompletedOrders}";
            _view.Status.text = _session.Status;
            _view.Goal.text = $"Соберите {_session.RevenueGoal} монет за смену.\nСоставьте напиток по чеку → приготовьте → выдайте.\nОшибка и опоздание снижают репутацию.";
            _view.MenuStats.text = $"В кошельке: {_save.Wallet}     •     Рекорд выручки: {_save.BestRevenue}     •     Заказов: {_save.BestOrders}";
            string result = $"{_session.Status}\n\nВыручка  {_session.Revenue}     /     Заказов  {_session.CompletedOrders}\nКошелёк  {_save.Wallet}     /     Рекорд выручки  {_save.BestRevenue}\n\nЗаработанные монеты уже сохранены.";
            _view.SuccessStats.text = result;
            _view.FailureStats.text = result;
            _view.Wallet.text = $"КОШЕЛЁК  {_save.Wallet} монет   •   Покупки сохраняются между запусками";
            _view.SettingsVolume.text = $"Общая громкость: {Mathf.RoundToInt(_save.Volume * 100)}%";
            _view.FullscreenLabel.text = _save.Fullscreen ? "Полный экран: ВКЛ" : "Полный экран: ВЫКЛ";
            _view.VolumeDown.interactable = _save.Volume > 0;
            _view.VolumeUp.interactable = _save.Volume < 1;
            _ingredients.SetInteractable(_session.CanSelect);
            _view.RemoveLast.interactable = _session.CanSelect && _preparation.Ingredients.Count > 0;
            _view.Prepare.interactable = _session.CanPrepare;
            _view.Serve.interactable = _session.CanServe;
            _text.Clear();
            foreach (var ingredient in _preparation.Ingredients)
            {
                if (_text.Length > 0) _text.Append(" + ");
                _text.Append(ingredient.DisplayName);
            }
            _view.Composition.text = _text.Length == 0 ? "Состав: пока пусто" : "Состав: " + _text;
            bool brewing = _preparation.Phase == PreparationPhase.Preparing;
            bool ready = _preparation.Phase == PreparationPhase.Ready;
            _view.Preparation.text = brewing ? $"Готовим… {_preparation.RemainingSeconds:0.0} с" :
                ready ? "Напиток готов. Выдайте гостю!" : "1  Состав   →   2  Приготовить   →   3  Выдать";
            _view.BrewProgress.fillAmount = ready ? 1 : brewing ? 1 - _preparation.RemainingSeconds / _session.PreparationDuration : 0;
            _view.Visitor.SetActive(_orders.CurrentOrder != null && _state.Current != GamePhase.MainMenu);
            _view.Cup.SetActive(_preparation.Ingredients.Count > 0);
            if (_orders.CurrentOrder != null)
            {
                _text.Clear();
                foreach (var ingredient in _orders.CurrentOrder.Recipe.Ingredients)
                    _text.Append("• ").Append(ingredient.DisplayName).Append('\n');
                int reward = Mathf.RoundToInt(_orders.CurrentOrder.Recipe.Reward * _upgrades.Multiplier(UpgradeEffect.OrderReward));
                _view.TicketDetails.text = _text + $"\nНаграда: {reward} монет";
            }
            foreach (var (card, config) in _cards)
            {
                int level = _upgrades.Level(config);
                card.Level.text = $"Уровень {level} / {config.MaxLevel}    •    +{level * config.EffectPerLevel * 100:0}%";
                card.Price.text = level >= config.MaxLevel ? "Максимум" : $"Купить • {_upgrades.Price(config)}";
                card.Buy.interactable = _upgrades.CanBuy(config);
            }
        }
        public void Dispose()
        {
            _state.Changed -= OnPhaseChanged;
            _session.OrderResolved -= _view.PlayResult;
            foreach (var (button, action) in _bindings) if (button) button.onClick.RemoveListener(action);
            _bindings.Clear();
            foreach (var (view, _) in _cards) if (view) UnityEngine.Object.Destroy(view.gameObject);
            _cards.Clear();
            _started = false;
        }
    }
}
