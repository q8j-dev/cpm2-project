using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.MenuUI.LocalizationUI
{
	[RequireComponent(typeof(Text))]
	public class LocalizedText : LocalizedTextBase
	{
		[SerializeField]
		private Text _text;

		[SerializeField]
		private bool UpperCase = true;

		[SerializeField]
		private bool UseStyle = default;

		private void Awake()
		{
			if (!_text)
			{
				TryGetComponent<Text>(out _text);
			}
		}

		private void Reset()
		{
			TryGetComponent<Text>(out _text);
		}

		public override void SetText(string localized)
		{
			if (UpperCase)
			{
				localized = localized.ToUpper();
			}
			if (UseStyle)
			{
				localized = LocalizedTextStyles.Apply(localized);
			}
			_text.text = localized;
		}
	}
}
