using UnityEngine;
using UnityEngine.UI;
using _Car_Parking;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.MenuUI.LanguagesServers;

[RequireComponent(typeof(Image))]
public class PlayersListFlag : MonoBehaviour
{
	[SerializeField]
	private Sprite[] Flags;

	[SerializeField]
	private LanguagesIconsConfig _config;

	private void Start()
	{
		if (!FlagsStorage.IsMultiFree)
		{
			base.gameObject.SetActive(value: false);
		}
		int language = PlayerPrefs.GetInt("languageIndex", (int)Application.systemLanguage);
		Sprite sprite = _config.TryGetIcon((LanguageIconsProvider.CpmLanguages)language);
		if ((bool)sprite)
		{
			GetComponent<Image>().sprite = sprite;
		}
	}
}
