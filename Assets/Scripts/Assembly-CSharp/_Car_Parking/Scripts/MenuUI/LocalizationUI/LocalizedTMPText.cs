using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.MenuUI.LocalizationUI
{
	[RequireComponent(typeof(TMP_Text))]
	public class LocalizedTMPText : LocalizedTextBase
	{
		[SerializeField]
		private TMP_Text _textMesh;

		private void Awake()
		{
			if (!_textMesh)
			{
				_textMesh = GetComponent<TMP_Text>();
			}
		}

		private void Reset()
		{
			_textMesh = GetComponent<TMP_Text>();
		}

		public override void SetText(string localized)
		{
			_textMesh.text = localized;
		}
	}
}
