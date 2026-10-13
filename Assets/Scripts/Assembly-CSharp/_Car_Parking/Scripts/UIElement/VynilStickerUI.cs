using CPM.Scripts.VinylDrawer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class VynilStickerUI : MonoBehaviour
	{
		[SerializeField]
		private Button Button = default;

		[SerializeField]
		private TMP_Text Price = default;

		[SerializeField]
		private Image VynilSprite = default;

		[SerializeField]
		private Image BorderSprite = default;

		[SerializeField]
		private Color SelectColor = default;

		[SerializeField]
		private bool IsVinyl = default;

		private Color _defaultColor;

		private VinylsEditor _vinylsEditor;

		public int Index { get; set; }

		public void Init(VinylsEditor vinylsEditor)
		{
			_vinylsEditor = vinylsEditor;
			_defaultColor = BorderSprite.color;
			Button.onClick.RemoveAllListeners();
			Button.onClick.AddListener(OnClick);
			if (IsVinyl)
			{
				Button.onClick.AddListener(() =>
				{
					_vinylsEditor.SelectVinylUI(this);
				});
			}
			else
			{
				Button.onClick.AddListener(() =>
				{
					_vinylsEditor.ChangePlate(this);
				});
			}
		}

		private void OnClick()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.TuningSelectVinylSwitch, base.gameObject);
		}

		public void SetSprite(Sprite sprite)
		{
			VynilSprite.sprite = sprite;
		}

		public void SetText(string text)
		{
			Price.text = text;
		}

		public void Select(bool isOn)
		{
			BorderSprite.color = (isOn ? SelectColor : _defaultColor);
		}
	}
}
