using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Drag
{
	public class ConfirmWindow : WindowScriptAsyncResult<EmptyData>, IBackableWindow
	{
		[Serializable]
		[Preserve]
		public class AdditionalData
		{
			[Serializable]
			public class ConfirmTimerData
			{
				public bool IsEnabled;

				public float Time;

				public ConfirmTimerData(bool isEnabled, float time)
				{
					IsEnabled = isEnabled;
					Time = time;
				}
			}

			public bool SetBackground;

			public Color Background;

			public bool HaveCombinedPrice;

			public CombinedPrice CombinedPrice;

			public ConfirmTimerData ConfirmTimer;

			public bool CancelButtonVisible = true;

			public AdditionalData()
			{
			}

			public AdditionalData(CombinedPrice combinedPrice)
			{
				CombinedPrice = combinedPrice;
				HaveCombinedPrice = true;
			}

			public AdditionalData(Color background)
			{
				SetBackground = true;
				Background = background;
			}

			public AdditionalData(bool setBackground, Color background, ConfirmTimerData confirmTimer)
			{
				SetBackground = setBackground;
				Background = background;
				ConfirmTimer = confirmTimer;
			}
		}

		[SerializeField]
		private TMP_Text Title;

		[SerializeField]
		private TMP_Text Content;

		[SerializeField]
		private TMP_Text ConfirmButtonText;

		[SerializeField]
		private Button ConfirmButton;

		[SerializeField]
		private Button CancelButton;

		[SerializeField]
		private Button WideCancelButton;

		[SerializeField]
		private RectTransform LayoutParent;

		[SerializeField]
		private RawImage NotificationImage;

		[SerializeField]
		private Image Background;

		[SerializeField]
		private CombinedPriceView CombinedPriceView;

		private List<ConfirmAdditionalController> _additionalControllers = new List<ConfirmAdditionalController>();

		private UnityAction _confirmCallback;

		private UnityAction _cancelCallback;

		private bool _cancelVisible = true;

		private const int NestedFitterPasses = 2;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		protected override void Awake()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ConfirmationWindowSwitch, base.gameObject);
			base.Awake();
			if (Title == null || Content == null || CancelButton == null || ConfirmButton == null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				throw new Exception("ConfirmWindow: Some fields are not assigned");
			}
			base.GUID = base.gameObject.name;
		}

		public virtual void InitCallback(UnityAction confirmCallback, string title, string content, string confirmButtonText = "")
		{
			_confirmCallback = confirmCallback;
			if (confirmButtonText == "")
			{
				confirmButtonText = new LocalizedString("MarketInventoryPopup.Confirm");
			}
			ConfirmButtonText.text = confirmButtonText;
			Title.text = title;
			Content.text = content;
			RebuildLayout().Forget();
			InitBackButton();
		}

		public void SetAdditionalData(AdditionalData additionalData)
		{
			if (additionalData != null)
			{
				if ((bool)Background && additionalData.SetBackground)
				{
					Background.color = additionalData.Background;
				}
				if (additionalData.ConfirmTimer != null && additionalData.ConfirmTimer.IsEnabled)
				{
					_additionalControllers.Add(new ConfirmAdditionalControllerConfirmTimer(additionalData.ConfirmTimer.Time, ConfirmButtonText, ConfirmButton));
				}
				if (additionalData.HaveCombinedPrice)
				{
					CombinedPriceView.Init(additionalData.CombinedPrice);
					CombinedPriceView.gameObject.SetActive(value: true);
				}
				CancelButton.gameObject.SetActive(additionalData.CancelButtonVisible);
				if ((bool)WideCancelButton)
				{
					WideCancelButton.enabled = additionalData.CancelButtonVisible;
				}
				_cancelVisible = additionalData.CancelButtonVisible;
				if (!_cancelVisible)
				{
					_managerUI.OnBackButtonActivated?.Invoke(obj: false);
				}
			}
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			if (!_cancelVisible)
			{
				ReRegisterBackButton().Forget();
			}
			else
			{
				CancelClickCallback();
			}
		}

		private async UniTaskVoid ReRegisterBackButton()
		{
			await UniTask.Yield();
			if ((bool)this)
			{
				_managerUI.SetCurrentWindow(this);
			}
		}

		private async UniTaskVoid RebuildLayout()
		{
			await UniTask.Yield(this.GetCancellationTokenOnDestroy());
			LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutParent);
			for (int i = 0; i < 2; i++)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutParent);
			}
		}

		public void InitCallback(UnityAction confirmCallback, UnityAction cancelCallback, string title, string content, string confirmButtonText = "", Texture texture = null)
		{
			_confirmCallback = confirmCallback;
			_cancelCallback = cancelCallback;
			if (confirmButtonText == "")
			{
				confirmButtonText = new LocalizedString("MarketInventoryPopup.Confirm");
			}
			if ((bool)texture)
			{
				NotificationImage.gameObject.SetActive(value: true);
				NotificationImage.texture = texture;
			}
			ConfirmButtonText.text = confirmButtonText;
			Title.text = title;
			Content.text = content;
			RebuildLayout().Forget();
			InitBackButton();
		}

		protected override void OnEnable()
		{
			if ((bool)ConfirmButton)
			{
				ConfirmButton.onClick.AddListener(ConfirmClickCallback);
			}
			if ((bool)CancelButton)
			{
				CancelButton.onClick.AddListener(CancelClickCallback);
			}
			if ((bool)WideCancelButton)
			{
				WideCancelButton.onClick.AddListener(CancelClickCallback);
			}
			base.OnEnable();
			if ((bool)CancelButton)
			{
				UISelectionCursor.ShowCursor(CancelButton.gameObject);
			}
		}

		private void OnDisable()
		{
			if ((bool)ConfirmButton)
			{
				ConfirmButton.onClick.RemoveListener(ConfirmClickCallback);
			}
			if ((bool)CancelButton)
			{
				CancelButton.onClick.RemoveListener(CancelClickCallback);
			}
			if ((bool)WideCancelButton)
			{
				WideCancelButton.onClick.RemoveListener(CancelClickCallback);
			}
		}

		private void Update()
		{
			for (int i = 0; i < _additionalControllers.Count; i++)
			{
				_additionalControllers[i].Update();
			}
		}

		private void ConfirmClickCallback()
		{
			TrySetResult(CodeType.Success);
			_confirmCallback?.Invoke();
			Close();
		}

		private void CancelClickCallback()
		{
			TrySetResult(CodeType.Canceled);
			_cancelCallback?.Invoke();
			Close();
		}
	}
}
