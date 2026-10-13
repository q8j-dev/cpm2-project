using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class InputFieldSizeFitter : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField CurrentInputField = default;

		[SerializeField]
		private RectTransform TargetRect = default;

		[SerializeField]
		private RectTransform ParentRect = default;

		public void Init()
		{
			CurrentInputField.onValueChanged.AddListener((string text) =>
			{
				SetRectSize().Forget();
			});
			SetRectSize().Forget();
		}

		private async UniTask SetRectSize()
		{
			await UniTask.Yield();
			ParentRect.sizeDelta = new Vector2(TargetRect.sizeDelta.x + 24f, ParentRect.sizeDelta.y);
		}
	}
}
