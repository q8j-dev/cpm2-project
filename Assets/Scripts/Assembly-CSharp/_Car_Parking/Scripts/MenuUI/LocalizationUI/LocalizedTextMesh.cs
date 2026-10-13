using UnityEngine;

namespace _Car_Parking.Scripts.MenuUI.LocalizationUI
{
	[RequireComponent(typeof(TextMesh))]
	public class LocalizedTextMesh : LocalizedTextBase
	{
		[SerializeField]
		private TextMesh _textMesh;

		private void Awake()
		{
			if (!_textMesh)
			{
				_textMesh = GetComponent<TextMesh>();
			}
		}

		private void Reset()
		{
			_textMesh = GetComponent<TextMesh>();
		}

		public override void SetText(string localized)
		{
			_textMesh.text = localized;
		}
	}
}
