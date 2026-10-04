using SaveSystem;
using UnityEngine;
namespace SettingsSystem
{
    public sealed class SettingsService
    {
        private readonly SaveService _save;
        public SettingsService(SaveService save) => _save = save;
        public void Apply() { AudioListener.volume = _save.Volume; Screen.fullScreen = _save.Fullscreen; }
        public void SetVolume(float volume)
        {
            _save.SetSettings(volume, _save.Fullscreen);
            AudioListener.volume = _save.Volume;
        }
        public void ToggleFullscreen()
        {
            _save.SetSettings(_save.Volume, !_save.Fullscreen);
            Screen.fullScreen = _save.Fullscreen;
        }
    }
}
