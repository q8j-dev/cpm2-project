using CodeStage.AntiCheat.Storage;
using UnityEngine;
using UnityEngine.UI;

public class isBought : MonoBehaviour
{
	public bool bought;

	public int ThisPrice;

	public bool requireTurbo;

	public GameObject lockTexture;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public void updateStat()
	{
		if (base.transform.Find("backgrounds") == null)
		{
			return;
		}
		int num = int.Parse(base.gameObject.name);
		int num2 = PlayerPrefs.GetInt(num + "_" + V.CarId.ToString());
		int num3 = PlayerPrefs.GetInt("isSet16Car" + V.CarId.ToString());
		int num4 = PlayerPrefs.GetInt("isSet17Car" + V.CarId.ToString());
		int num5 = PlayerPrefs.GetInt("isSet28Car" + V.CarId.ToString());
		if (num == 10 && (bool)lockTexture)
		{
			if (ObscuredPrefs.GetInt("w16unlocked") == 0)
			{
				lockTexture.SetActive(value: true);
			}
			else
			{
				lockTexture.SetActive(value: false);
			}
		}
		if (requireTurbo)
		{
			if (num3 == 1 || num4 == 1 || num5 == 1)
			{
				base.gameObject.SetActive(value: true);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
		if (num2 == 1)
		{
			bought = true;
		}
		else
		{
			bought = false;
		}
		if (PlayerPrefs.GetInt("isSet" + num + "Car" + V.CarId.ToString(), 0) == 1)
		{
			base.transform.Find("backgrounds").GetComponent<RawImage>().color = Color.yellow;
		}
		else
		{
			base.transform.Find("backgrounds").GetComponent<RawImage>().color = Color.white;
		}
	}
}
