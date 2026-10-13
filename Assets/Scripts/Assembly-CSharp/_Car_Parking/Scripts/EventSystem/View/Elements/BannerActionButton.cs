using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.EventSystem.View.Elements
{
	[RequireComponent(typeof(Button))]
	public class BannerActionButton : MonoBehaviour
	{
		[SerializeField]
		private BannerButtonKind Kind = default;

		private Button _button;

		public BannerButtonKind ButtonKind => Kind;

		public event Action<BannerActionButton> Clicked;

		private void Awake()
		{
			_button = GetComponent<Button>();
		}

		private void OnEnable()
		{
			if (!_button)
			{
				_button = GetComponent<Button>();
			}
			_button.onClick.AddListener(ClickCallback);
		}

		private void OnDisable()
		{
			if ((bool)_button)
			{
				_button.onClick.RemoveListener(ClickCallback);
			}
		}

		private void ClickCallback()
		{
			Clicked?.Invoke(this);
		}
	}
}
