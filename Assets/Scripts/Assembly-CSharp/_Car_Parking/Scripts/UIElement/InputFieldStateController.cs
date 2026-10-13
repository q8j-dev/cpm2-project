using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class InputFieldStateController : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField TMPInputField = default;

		[SerializeField]
		private Image Background = default;

		[SerializeField]
		private Sprite Normal = default;

		[SerializeField]
		private Sprite Active = default;

		[SerializeField]
		private Sprite Error = default;

		[SerializeField]
		private Color NormalTextColor = default;

		[SerializeField]
		private Color ErrorTextColor = default;

		[SerializeField]
		private bool IsEmail = default;

		[SerializeField]
		private Button ClearButton = default;

		[SerializeField]
		private bool IsPassword = default;

		[SerializeField]
		private Toggle PasswordHideToggle = default;

		[SerializeField]
		private Sprite On = default;

		[SerializeField]
		private Sprite Off = default;

		private bool _isEmpty = true;

		private bool _prevState = true;

		public event Action<bool> OnEmpty;

		private void Start()
		{
			TMPInputField.onSelect.AddListener((string value) =>
			{
				SetActiveState();
			});
			TMPInputField.onDeselect.AddListener((string value) =>
			{
				SetNormal();
			});
			TMPInputField.onValueChanged.AddListener((string value) =>
			{
				CheckText();
			});
			if (IsEmail)
			{
				ClearButton.onClick.AddListener(ClearText);
			}
			if (IsPassword)
			{
				PasswordHideToggle.onValueChanged.AddListener(HidePassword);
			}
		}

		private void SetNormal()
		{
			Background.sprite = Normal;
			TMPInputField.textComponent.color = NormalTextColor;
		}

		private void SetActiveState()
		{
			Background.sprite = Active;
			TMPInputField.textComponent.color = NormalTextColor;
		}

		public void SetError()
		{
			Background.sprite = Error;
			TMPInputField.textComponent.color = ErrorTextColor;
		}

		private void CheckText()
		{
			_isEmpty = string.IsNullOrEmpty(TMPInputField.text);
			if (_isEmpty != _prevState)
			{
				OnEmpty?.Invoke(_isEmpty);
			}
			_prevState = _isEmpty;
			if (IsEmail)
			{
				ClearButton.SetActive(!_isEmpty);
			}
			if (IsPassword)
			{
				PasswordHideToggle.SetActive(!_isEmpty);
			}
		}

		private void ClearText()
		{
			TMPInputField.text = "";
		}

		private void HidePassword(bool isOn)
		{
			PasswordHideToggle.image.sprite = (isOn ? On : Off);
			TMPInputField.contentType = (isOn ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard);
			TMPInputField.ForceLabelUpdate();
		}
	}
}
