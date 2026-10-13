using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class InputFieldStateController : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField TMPInputField;

		[SerializeField]
		private Image Background;

		[SerializeField]
		private Sprite Normal;

		[SerializeField]
		private Sprite Active;

		[SerializeField]
		private Sprite Error;

		[SerializeField]
		private Color NormalTextColor;

		[SerializeField]
		private Color ErrorTextColor;

		[SerializeField]
		private bool IsEmail;

		[SerializeField]
		private Button ClearButton;

		[SerializeField]
		private bool IsPassword;

		[SerializeField]
		private Toggle PasswordHideToggle;

		[SerializeField]
		private Sprite On;

		[SerializeField]
		private Sprite Off;

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
