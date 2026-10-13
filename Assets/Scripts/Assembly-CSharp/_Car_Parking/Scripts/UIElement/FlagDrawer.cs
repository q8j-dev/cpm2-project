using Flags;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	public class FlagDrawer : MonoBehaviour
	{
		[SerializeField]
		private RawImage RawImage;

		private FlagsDataSO _flagsDataSo;

		[Inject]
		public void Construct(FlagsDataSO flagsDataSo)
		{
			_flagsDataSo = flagsDataSo;
		}

		public void Set(MainCarValues carValues)
		{
			Set((int)((!(carValues != null)) ? SystemLanguage.Arabic : carValues.language));
		}

		public void Set(int index)
		{
			if (_flagsDataSo != null)
			{
				index = Mathf.Clamp(index, 0, _flagsDataSo.languageTextures.Length - 1);
				if (RawImage != null && _flagsDataSo.languageTextures.Length != 0)
				{
					RawImage.texture = _flagsDataSo.languageTextures[index];
				}
			}
		}

		public void Show()
		{
			base.gameObject.SetActive(value: true);
		}

		public void Hide()
		{
			base.gameObject.SetActive(value: false);
		}

		private void SetRefs()
		{
			RawImage = GetComponent<RawImage>();
		}
	}
}
