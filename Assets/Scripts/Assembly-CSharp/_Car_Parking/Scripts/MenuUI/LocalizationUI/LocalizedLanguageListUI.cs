using System.Linq;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.MenuUI.LocalizationUI
{
	public class LocalizedLanguageListUI : MonoBehaviour
	{
		[SerializeField]
		private GameObject LanguageListPrefab = default;

		[SerializeField]
		private GameObject WindowParent = default;

		[SerializeField]
		private Transform ElementsParent = default;

		[SerializeField]
		private Texture[] AllFlags = default;

		private int _yPosIndex;

		private IPrefabFactory _prefabFactory;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory)
		{
			_prefabFactory = prefabFactory;
		}

		private void Start()
		{
			WindowParent.gameObject.SetActive(value: false);
		}

		private void CreateAllLanguageBox()
		{
			_yPosIndex = 0;
			foreach (Transform item in ElementsParent)
			{
				if ((bool)item)
				{
					Object.Destroy(item.gameObject);
				}
			}
			foreach (string item2 in LocalizationManager.LocalizedTexts.Keys.ToList())
			{
				if (!(item2 == "CheckKeys"))
				{
					CreateLanguageBox(item2);
				}
			}
		}

		private void CreateLanguageBox(string language)
		{
			GameObject obj = _prefabFactory.Instantiate(LanguageListPrefab, Vector3.zero, Quaternion.identity);
			obj.GetComponent<UILanguageListElement>();
			_ = AllFlags[GetLanguageIndex(language)];
			obj.transform.parent = ElementsParent;
			obj.transform.localScale = new Vector3(0.87f, 0.8f, 1f);
			obj.transform.localPosition = new Vector3(0f, _yPosIndex * -80 + 700, 15f);
			_yPosIndex++;
		}

		private int GetLanguageIndex(string language)
		{
			return language switch
			{
				"English" => 0, 
				"Russian" => 1, 
				"French" => 2, 
				"Italian" => 3, 
				"Chinese" => 4, 
				"German" => 5, 
				"Japanese" => 6, 
				"Korean" => 7, 
				"Spanish" => 8, 
				"Turkish" => 9, 
				"Kazakh" => 10, 
				_ => 0, 
			};
		}

		public void ShowLanguageList()
		{
			WindowParent.gameObject.SetActive(!WindowParent.gameObject.activeSelf);
			if (WindowParent.gameObject.activeSelf)
			{
				CreateAllLanguageBox();
			}
		}
	}
}
