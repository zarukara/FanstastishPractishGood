using TMPro;
using UnityEngine;

namespace UISystem
{
    public sealed class OrderView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _orderNameText;
        [SerializeField] private TMP_Text _orderTimerText;

        private int _lastDisplayedSeconds = -1;

        public void Show(string orderName)
        {
            gameObject.SetActive(true);
            _orderNameText.text = $"Заказ: {orderName}";
            _lastDisplayedSeconds = -1;
        }

        public void SetRemainingSeconds(float remainingSeconds)
        {
            int seconds = Mathf.CeilToInt(Mathf.Max(0f, remainingSeconds));

            if (_lastDisplayedSeconds == seconds)
            {
                return;
            }

            _lastDisplayedSeconds = seconds;
            _orderTimerText.text = $"{seconds} сек";
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}