using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;

public class DiscordControl : MonoBehaviour
{
	public void Start()
	{
	}

	public void OnDiscordPressed()
	{
		PlayerPrefs.SetInt("discord", 1);
		ObscuredInt value = 5000;
		CurrencyManagerController.Instance.AddMoney(new AdvancedInt(value));
		base.gameObject.SetActive(value: false);
		Application.OpenURL("https://discord.com/invite/DaXMq99");
	}
}
