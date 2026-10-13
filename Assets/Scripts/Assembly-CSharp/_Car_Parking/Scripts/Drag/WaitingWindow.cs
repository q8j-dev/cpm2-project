using System;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.MenuUI.LocalizationUI;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Drag
{
	public class WaitingWindow : WindowScript
	{
		public class AdditionalData
		{
			public bool OverrideSortingLayer = true;

			public bool IsDarkLoadingWindow;
		}

		[SerializeField]
		private Transform WheelImage;

		[SerializeField]
		private float Duration = 1.5f;

		[SerializeField]
		private TextMeshProUGUI Description;

		[SerializeField]
		private LocalizedTextMeshPro Localized;

		[SerializeField]
		private Canvas Canvas;

		[field: SerializeField]
		public Image Background { get; private set; }

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.NoInput;

		protected override void Awake()
		{
			TryInitWindow();
			base.Awake();
			if (!string.IsNullOrEmpty(_managerUI.WaitingWindowText))
			{
				SetDescription(_managerUI.WaitingWindowText);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_managerUI.WaitingWindowText = "";
			if (InputMapSetEvent.CurrentInputGroup == InputGroupsEnum.NoInput)
			{
				InputMapSetEvent.TrySetPreviousGroup();
			}
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			if (string.IsNullOrEmpty(data))
			{
				return;
			}
			try
			{
				AdditionalData additionalData = JsonConvert.DeserializeObject<AdditionalData>(data);
				if (additionalData != null)
				{
					if ((bool)Canvas)
					{
						Canvas.overrideSorting = additionalData.OverrideSortingLayer;
					}
					if ((bool)Background && additionalData.IsDarkLoadingWindow)
					{
						Background.color = new Color(0f, 0f, 0f, 1f);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void SetDescription(string description)
		{
			Description.text = description;
			Localized.enabled = false;
		}

		private void Update()
		{
			if ((bool)WheelImage && !(Duration <= 0f))
			{
				float z = -360f * (Time.unscaledTime / Duration % 1f);
				WheelImage.localEulerAngles = new Vector3(0f, 0f, z);
			}
		}

		private void TryInitWindow()
		{
			if (_managerUI.DarkLoadingWindowInitializer != null)
			{
				_managerUI.DarkLoadingWindowInitializer.Init(this);
				_managerUI.DarkLoadingWindowInitializer = null;
			}
		}
	}
}
