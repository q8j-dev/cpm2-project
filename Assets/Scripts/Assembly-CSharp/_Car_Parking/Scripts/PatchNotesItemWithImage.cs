using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts
{
	public class PatchNotesItemWithImage : MonoBehaviour
	{
		private const string DefaultCarPreviewPath = "UI/CarImages/DefaultCar";

		[SerializeField]
		private TMP_Text Text = default;

		[SerializeField]
		private Image Image = default;

		[SerializeField]
		private Button Button = default;

		private PatchNotesData.CarItem _carItem;

		public event Action<PatchNotesData.CarItem> OnItemPressed;

		private void Start()
		{
			Button.onClick.AddListener(ItemPressedHandler);
		}

		public void Init(PatchNotesData.CarItem data)
		{
			_carItem = data;
			if ((bool)Text)
			{
				Text.text = data.Text;
			}
			if ((bool)Image)
			{
				LoadSpriteAsync(data.Index, this.GetCancellationTokenOnDestroy()).Forget();
			}
		}

		private async UniTaskVoid LoadSpriteAsync(int carIndex, CancellationToken token)
		{
			Sprite sprite = (await Resources.LoadAsync<Sprite>(PathGetterHelper.GetCarPreviewPath(carIndex)).ToUniTask(null, PlayerLoopTiming.Update, token)) as Sprite;
			if (!sprite)
			{
				Debug.LogError($"[PatchNotesItemWithImage] Car preview sprite not found for index {carIndex}");
				sprite = (await Resources.LoadAsync<Sprite>("UI/CarImages/DefaultCar").ToUniTask(null, PlayerLoopTiming.Update, token)) as Sprite;
			}
			if ((bool)Image && (bool)sprite)
			{
				Image.sprite = sprite;
			}
		}

		private void ItemPressedHandler()
		{
			OnItemPressed?.Invoke(_carItem);
		}

		private void OnDestroy()
		{
			Button.onClick.RemoveListener(ItemPressedHandler);
		}
	}
}
