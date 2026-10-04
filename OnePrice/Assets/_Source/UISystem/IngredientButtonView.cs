using System;
using RecipeSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public sealed class IngredientButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _nameText;

        private IngredientConfig _ingredient;

        public event Action<IngredientConfig> Clicked;

        public void SetInteractable(bool value) => _button.interactable = value;

        public void Bind(IngredientConfig ingredient)
        {
            if (ingredient == null)
                throw new ArgumentNullException(nameof(ingredient));

            _ingredient = ingredient;
            _nameText.text = ingredient.DisplayName;
        }

        private void OnEnable()
        {
            if (_button != null) _button.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            if (_button != null) _button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            if (_ingredient != null)
                Clicked?.Invoke(_ingredient);
        }
    }
}
