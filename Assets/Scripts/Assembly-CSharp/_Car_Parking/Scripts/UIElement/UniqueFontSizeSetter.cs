using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class UniqueFontSizeSetter : MonoBehaviour
	{
		[SerializeField]
		private RectTransform RectForRebuild;

		[SerializeField]
		private List<TextMeshProUGUI> TextMeshPros;

		private void Start()
		{
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				SetMinSize();
			}).AddTo(this);
		}

		private void SetMinSize()
		{
			float num = 150f;
			foreach (TextMeshProUGUI textMeshPro in TextMeshPros)
			{
				if (textMeshPro.fontSize < num)
				{
					num = textMeshPro.fontSize;
				}
			}
			foreach (TextMeshProUGUI textMeshPro2 in TextMeshPros)
			{
				textMeshPro2.enableAutoSizing = false;
				textMeshPro2.fontSize = num;
			}
			UIHelper.DelayedRebuildLayout(RectForRebuild).Forget();
		}
	}
}
