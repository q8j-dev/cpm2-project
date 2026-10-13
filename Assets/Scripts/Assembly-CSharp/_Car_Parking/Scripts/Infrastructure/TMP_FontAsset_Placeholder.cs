using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Infrastructure
{
	public class TMP_FontAsset_Placeholder : MonoBehaviour
	{
		private const string FallbackPath = "NotoSansSCRegularSDF";

		[SerializeField]
		private TMP_FontAsset DefaultFont;

		[SerializeField]
		private TMP_FontAsset BebasNeue;

		[SerializeField]
		private TMP_FontAsset BebasNeuePro;

		public static TMP_FontAsset FallbackFont;

		private static TMP_FontAsset _defaultFont;

		private static TMP_FontAsset _bebasNeue;

		private static TMP_FontAsset _bebasNeuePro;

		private static bool _loading;

		private void Awake()
		{
			_defaultFont = DefaultFont;
			_bebasNeue = BebasNeue;
			_bebasNeuePro = BebasNeuePro;
			if (NeedsFallbackForLanguage(LocalizationManager.Language))
			{
				EnsureLoaded().Forget();
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void SubscribeToLanguageChange()
		{
			LocalizationManager.LocalizationChanged -= OnLanguageChanged;
			LocalizationManager.LocalizationChanged += OnLanguageChanged;
		}

		private static void OnLanguageChanged()
		{
			if (NeedsFallbackForLanguage(LocalizationManager.Language))
			{
				EnsureLoaded().Forget();
			}
		}

		public static bool NeedsFallbackForLanguage(string language)
		{
			if (!(language == "Chinese") && !(language == "Japanese"))
			{
				return language == "Korean";
			}
			return true;
		}

		public static bool NeedsFallbackForText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			foreach (char c in text)
			{
				if ((c >= '⺀' && c <= '鿿') || (c >= '豈' && c <= '\ufaff') || (c >= '\uff00' && c <= '\uffef'))
				{
					return true;
				}
				if ((c >= '가' && c <= '힣') || (c >= 'ᄀ' && c <= 'ᇿ') || (c >= 'ꥠ' && c <= '\ua97f') || (c >= 'ힰ' && c <= '\ud7ff'))
				{
					return true;
				}
			}
			return false;
		}

		public static void EnsureFor(string text)
		{
			if (!FallbackFont && !_loading && NeedsFallbackForText(text))
			{
				EnsureLoaded().Forget();
			}
		}

		public static async UniTaskVoid EnsureLoaded()
		{
			if (!FallbackFont && !_loading)
			{
				_loading = true;
				TMP_FontAsset tMP_FontAsset = await Addressables.LoadAssetAsync<TMP_FontAsset>("NotoSansSCRegularSDF");
				_loading = false;
				if ((bool)tMP_FontAsset)
				{
					FallbackFont = tMP_FontAsset;
					AddTo(_defaultFont);
					AddTo(_bebasNeue);
					AddTo(_bebasNeuePro);
					RebuildExistingText();
				}
			}
		}

		private static void AddTo(TMP_FontAsset font)
		{
			if ((bool)font && !font.fallbackFontAssetTable.Contains(FallbackFont))
			{
				font.fallbackFontAssetTable.Add(FallbackFont);
			}
		}

		private static void RebuildExistingText()
		{
			TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(isChanged: true, _defaultFont);
			TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(isChanged: true, _bebasNeue);
			TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(isChanged: true, _bebasNeuePro);
			TMP_Text[] array = Object.FindObjectsOfType<TMP_Text>(includeInactive: true);
			for (int i = 0; i < array.Length; i++)
			{
				if ((bool)array[i])
				{
					array[i].SetAllDirty();
				}
			}
		}
	}
}
