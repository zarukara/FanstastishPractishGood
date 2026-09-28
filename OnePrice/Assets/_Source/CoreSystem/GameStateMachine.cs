using System;

namespace CoreSystem
{
    /// <summary>Owns the current screen/game phase and announces transitions.</summary>
    public sealed class GameStateMachine
    {
        private GamePhase _returnFromSettings;

        public event Action<GamePhase> Changed;

        public GamePhase Current { get; private set; } = GamePhase.MainMenu;

        public void StartDay() => Set(GamePhase.Playing);

        public void Pause()
        {
            if (Current == GamePhase.Playing)
            {
                Set(GamePhase.Paused);
            }
        }

        public void Resume()
        {
            if (Current == GamePhase.Paused)
            {
                Set(GamePhase.Playing);
            }
        }

        public void OpenSettings()
        {
            if (Current == GamePhase.Settings)
            {
                return;
            }

            _returnFromSettings = Current;
            Set(GamePhase.Settings);
        }

        public void CloseSettings()
        {
            if (Current == GamePhase.Settings)
            {
                Set(_returnFromSettings);
            }
        }

        public void Succeed() => Set(GamePhase.DaySuccess);
        public void Fail() => Set(GamePhase.DayFailure);
        public void OpenUpgrades() => Set(GamePhase.Upgrades);
        public void ReturnToMenu() => Set(GamePhase.MainMenu);

        private void Set(GamePhase next)
        {
            if (Current == next)
            {
                return;
            }

            Current = next;
            Changed?.Invoke(next);
        }
    }
}
