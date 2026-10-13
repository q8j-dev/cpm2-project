using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.MenuUI.LocalizationUI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LocalizedTextMeshPro : LocalizedTextBase
	{
		[SerializeField]
		private TextMeshProUGUI _textMesh;

		[SerializeField]
		private bool UseStyle;

		[SerializeField]
		private bool SetSizeFitterMaxSize;

		[SerializeField]
		private ContentSizeFitter SizeFitter;

		[SerializeField]
		private float MaxSize = 100f;

		private void Awake()
		{
			if (!_textMesh)
			{
				_textMesh = GetComponent<TextMeshProUGUI>();
			}
		}

		private void OnValidate()
		{
			_textMesh = GetComponent<TextMeshProUGUI>();
		}

		public override void SetText(string localized)
		{
			if (UseStyle)
			{
				localized = LocalizedTextStyles.Apply(localized);
			}
			_textMesh.text = localized;
			if (SetSizeFitterMaxSize)
			{
				UIHelper.SizeFitterMaxWidth(SizeFitter, MaxSize).Forget();
			}
		}
	}
}
