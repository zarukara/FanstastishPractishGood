using CoreSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UISystem
{
    // Scene references and presentation only. Game rules live in the injected services.
    public sealed class GameView : MonoBehaviour
    {
        public GameObject Gameplay, MainMenu, Pause, Settings, Success, Failure, Upgrades;
        public GameObject Visitor, Cup;
        public TMP_Text Hud, Composition, Preparation, Status, MenuStats, Goal, SuccessStats, FailureStats;
        public TMP_Text Wallet, SettingsVolume, FullscreenLabel, TicketDetails;
        public Button StartDay, MenuUpgrades, MenuSettings, Quit;
        public Button PauseGame, Resume, PauseSettings, PauseMenu;
        public Button RemoveLast, Prepare, Serve, SettingsBack, Fullscreen, VolumeDown, VolumeUp;
        public Button SuccessAgain, SuccessUpgrades, SuccessMenu, FailureAgain, FailureUpgrades, FailureMenu;
        public Button UpgradesPlay, UpgradesMenu;
        public Image BrewProgress;
        public RectTransform UpgradeContainer;
        public UpgradeCardView UpgradePrefab;
        public AudioSource Audio;
        public AudioClip ClickSound, SuccessSound, FailureSound;
        public void Show(GamePhase phase)
        {
            Gameplay.SetActive(phase == GamePhase.Playing || phase == GamePhase.Paused);
            MainMenu.SetActive(phase == GamePhase.MainMenu);
            Pause.SetActive(phase == GamePhase.Paused);
            Settings.SetActive(phase == GamePhase.Settings);
            Success.SetActive(phase == GamePhase.DaySuccess);
            Failure.SetActive(phase == GamePhase.DayFailure);
            Upgrades.SetActive(phase == GamePhase.Upgrades);
        }
        public void PlayClick() { if (Audio && ClickSound) Audio.PlayOneShot(ClickSound); }
        public void PlayResult(bool success)
        {
            var clip = success ? SuccessSound : FailureSound;
            if (Audio && clip) Audio.PlayOneShot(clip);
        }
    }
}
