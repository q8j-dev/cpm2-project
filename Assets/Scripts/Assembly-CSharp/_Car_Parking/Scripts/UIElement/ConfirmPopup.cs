using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.UIElement
{
	public class ConfirmPopup : MonoBehaviour
	{
		[SerializeField]
		private Button ConfirmButton;

		[SerializeField]
		private Button CancelButton;

		[SerializeField]
		private RectTransform Root;

		public event Action<ConfirmPopup> Confirmed;

		public event Action<ConfirmPopup> Canceled;

		protected void RebuildLayout()
		{
			RebuildLayoutAsync().Forget();
		}

		protected virtual void OnEnable()
		{
			ConfirmButton.onClick.AddListener(OnConfirmClick);
			CancelButton.onClick.AddListener(OnCancelClick);
			InitNavigation();
		}

		private void InitNavigation()
		{
			UiInputSetup.DisableUnnecessaryNavigation(base.gameObject);
			if (ConfirmButton.TryGetComponent<Selectable>(out var component))
			{
				Navigation navigation = component.navigation;
				navigation.mode = Navigation.Mode.Automatic;
				component.navigation = navigation;
			}
			if (CancelButton.TryGetComponent<Selectable>(out var component2))
			{
				Navigation navigation2 = component2.navigation;
				navigation2.mode = Navigation.Mode.Automatic;
				component2.navigation = navigation2;
			}
		}

		public void Back()
		{
			OnCancelClick();
		}

		protected virtual void OnDisable()
		{
			ConfirmButton.onClick.RemoveListener(OnConfirmClick);
			CancelButton.onClick.RemoveListener(OnCancelClick);
		}

		private void OnConfirmClick()
		{
			Confirmed?.Invoke(this);
		}

		private void OnCancelClick()
		{
			Canceled?.Invoke(this);
		}

		private async UniTaskVoid RebuildLayoutAsync()
		{
			await UniTask.Yield();
			if ((bool)Root)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(Root);
			}
		}
	}
}
