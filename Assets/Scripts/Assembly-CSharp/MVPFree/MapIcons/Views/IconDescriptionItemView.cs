using System;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.MenuUI.LocalizationUI;

namespace MVPFree.MapIcons.Views
{
	public class IconDescriptionItemView : MonoBehaviour
	{
		private static readonly Color DefaultColor = new Color(0f, 0f, 0f, 0.64f);

		[SerializeField]
		private Button MainButton;

		[SerializeField]
		private Button VisibleButton;

		[SerializeField]
		private Image[] BGImages;

		public Button MainButtonRef => MainButton;

		[field: SerializeField]
		public Image VisibleImage { get; private set; }

		[field: SerializeField]
		public Image Image { get; private set; }

		[field: SerializeField]
		public LocalizedTMPText LocalizedTMPText { get; private set; }

		public event Action<IconDescriptionItemView> MainButtonClicked;

		public event Action<IconDescriptionItemView> VisibleButtonClicked;

		public void SetDefaultBGColor()
		{
			SetBGColor(DefaultColor);
		}

		public void SetBGColor(Color color)
		{
			for (int i = 0; i < BGImages.Length; i++)
			{
				if ((bool)BGImages[i])
				{
					BGImages[i].color = color;
				}
			}
		}

		private void OnEnable()
		{
			MainButton.onClick.AddListener(MainButtonClickedCallback);
			VisibleButton.onClick.AddListener(VisibleButtonClickedCallback);
		}

		private void OnDisable()
		{
			MainButton.onClick.RemoveListener(MainButtonClickedCallback);
			VisibleButton.onClick.RemoveListener(VisibleButtonClickedCallback);
		}

		private void MainButtonClickedCallback()
		{
			MainButtonClicked?.Invoke(this);
		}

		private void VisibleButtonClickedCallback()
		{
			VisibleButtonClicked?.Invoke(this);
		}

		public void TriggerMainButton()
		{
			MainButtonClicked?.Invoke(this);
		}

		public void TriggerVisibleButton()
		{
			VisibleButtonClicked?.Invoke(this);
		}
	}
}
