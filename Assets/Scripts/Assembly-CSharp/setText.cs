using CPM.PlayerData;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;

public class setText : MonoBehaviour
{
	public TextMeshProUGUI[] Label;

	private string white = "[ffffff]";

	private string green = "[99ff00]";

	private string end_color = "[-]";

	private string red = "[ff0000]";

	private string orange = "[F4FA58]";

	private string mass;

	private string Cd = default;

	private string maxRpm;

	private string DrivingWheel = default;

	private string gears;

	private string shiftTime = default;

	public ObscuredFloat hpInput = 0f;

	public ObscuredFloat torqueInput = 0f;

	public ObscuredFloat massInput = 0f;

	private float EngineMassInput;

	private float IsManualInput;

	public TyreType TyreTypeInput;

	private float gearsInput;

	private float CdInput;

	public ObscuredFloat shiftTimeInput = 0f;

	public ObscuredFloat maxRpmInput = 0f;

	public ObscuredFloat maxTorqueRPMInput = 0f;

	public ObscuredFloat maxPowerRpmInptut = 0f;

	public ObscuredFloat price = 0f;

	private float brakeInput;

	private string brake = default;

	private string TDR;

	private Powertrain _powertrain;

	private new Rigidbody rigidbody;

	private Axles axles;

	private PlayerDatas playerDatas;

	private MenuControl menuControl;

	public int itemIndex;

	private PlayerDataStorage _playerDataStorage;

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage)
	{
		_playerDataStorage = playerDataStorage;
	}

	private void Start()
	{
		menuControl = GetComponent<MenuControl>();
	}

	public void initialize()
	{
		if ((bool)V.Car)
		{
			rigidbody = V.Car.GetComponent<Rigidbody>();
			_powertrain = V.Car.GetComponent<Powertrain>();
			axles = V.Car.GetComponent<Axles>();
		}
	}

	public void Label_set()
	{
		initialize();
		string text = "NO";
		if (IsSetItem(16) || IsSetItem(17) || IsSetItem(28))
		{
			text = "YES";
		}
		if ((bool)V.Car)
		{
			mass = rigidbody.mass.ToString();
			gears = _powertrain.NumGears.ToString();
			float maxWhp = ObscuredPrefs.GetFloat("mileage" + V.CarId.ToString(), Random.Range(5, 23));
			string text2 = maxWhp.ToString("F1") + " KM";
			_powertrain.CalcMaxPowerMaxTorque(out var maxHp, out var maxHpRpm, out var maxTq, out var maxTqRpm, out maxWhp);
			maxHpRpm = Utils.RoundTo(maxHpRpm / 100f, 0) * 100f;
			Label[0].text = $"{orange}Power: {end_color}{white}{maxHp:N0}hp ({maxHpRpm}){end_color}{orange}\r\nTorque: {end_color}{white}{maxTq:N0}Nm ({maxTqRpm}){end_color}{orange} \r\nAccel: {end_color}{white}{Cd} {end_color}{orange} \r\nDriving Wheel: {end_color}{white}{DrivingWheel} {end_color}{orange} \r\nBrake: {end_color}{white}{brake} {end_color}{orange} \r\nShift Time: {end_color}{white}{shiftTime} s {end_color}{orange} \r\nTURBO: {end_color}{white}{text} {end_color}{orange} \r\nMileage: {end_color}{white}{text2} {end_color}";
			Label[1].text = Label[0].text;
		}
	}

	public void Label_set2(int i)
	{
	}

	public void UpdateLabel(GameObject parent, float money)
	{
		initialize();
		ObscuredInt obscuredInt = int.Parse(parent.name);
		_powertrain.CalcMaxPowerMaxTorque(out var maxHp, out var maxHpRpm, out var maxTq, out var maxTqRpm, out var maxWhp);
		if ((int)obscuredInt != 0)
		{
			setAllZero();
		}
		switch (obscuredInt)
		{
		case 1:
			hpInput = 150f;
			maxTorqueRPMInput = 4100f;
			torqueInput = 220f;
			maxPowerRpmInptut = 5900f;
			maxRpmInput = 8000f;
			price = 5000f;
			EngineMassInput = 140f;
			break;
		case 2:
			hpInput = 190f;
			maxTorqueRPMInput = 4100f;
			torqueInput = 300f;
			maxPowerRpmInptut = 5900f;
			maxRpmInput = 8000f;
			price = 10000f;
			EngineMassInput = 150f;
			break;
		case 3:
			hpInput = 240f;
			maxTorqueRPMInput = 4500f;
			torqueInput = 310f;
			maxPowerRpmInptut = 6800f;
			maxRpmInput = 8000f;
			price = 15000f;
			EngineMassInput = 170f;
			break;
		case 4:
			hpInput = 280f;
			maxTorqueRPMInput = 4700f;
			torqueInput = 350f;
			maxPowerRpmInptut = 6300f;
			maxRpmInput = 8000f;
			price = 25000f;
			EngineMassInput = 180f;
			break;
		case 5:
			hpInput = 360f;
			maxTorqueRPMInput = 3400f;
			torqueInput = 500f;
			maxPowerRpmInptut = 6300f;
			maxRpmInput = 8000f;
			price = 40000f;
			EngineMassInput = 225f;
			break;
		case 6:
			hpInput = 415f;
			maxTorqueRPMInput = 4000f;
			torqueInput = 430f;
			maxPowerRpmInptut = 7000f;
			maxRpmInput = 8000f;
			price = 50000f;
			EngineMassInput = 220f;
			break;
		case 7:
			hpInput = 500f;
			maxTorqueRPMInput = 5600f;
			torqueInput = 620f;
			maxPowerRpmInptut = 7000f;
			maxRpmInput = 8000f;
			price = 100000f;
			EngineMassInput = 330f;
			break;
		case 8:
			hpInput = 580f;
			maxTorqueRPMInput = 5000f;
			torqueInput = 680f;
			maxPowerRpmInptut = 7000f;
			maxRpmInput = 8000f;
			price = 150000f;
			EngineMassInput = 350f;
			break;
		case 9:
			hpInput = 612f;
			maxTorqueRPMInput = 3500f;
			torqueInput = 1000f;
			maxPowerRpmInptut = 7000f;
			maxRpmInput = 8000f;
			price = 200000f;
			EngineMassInput = 280f;
			break;
		case 10:
			hpInput = 1120f;
			maxTorqueRPMInput = 3500f;
			torqueInput = 1250f;
			maxPowerRpmInptut = 7000f;
			maxRpmInput = 8000f;
			price = 400000f;
			EngineMassInput = 480f;
			break;
		case 11:
			gearsInput = 6f;
			IsManualInput = 1f;
			shiftTimeInput = 0f;
			TDR = "TRWD";
			price = 4000f;
			break;
		case 12:
			gearsInput = 6f;
			IsManualInput = 0f;
			shiftTimeInput = 0f;
			TDR = "TFWD";
			price = 5000f;
			break;
		case 13:
			gearsInput = 7f;
			IsManualInput = 0f;
			shiftTimeInput = 0.15f;
			TDR = "TAWD";
			price = 7000f;
			break;
		case 14:
			gearsInput = 7f;
			IsManualInput = 0f;
			shiftTimeInput = 0.1f;
			TDR = "TAWD";
			price = 10000f;
			break;
		case 15:
			gearsInput = 6f;
			IsManualInput = 1f;
			shiftTimeInput = 0f;
			TDR = "TAWD";
			price = 5000f;
			break;
		case 16:
			if (IsSetItem(16))
			{
				hpInput = 0f;
				torqueInput = 0f;
				break;
			}
			price = 10000f;
			if (IsSetItem(17))
			{
				float num15 = maxHp / 1.2f;
				float num16 = maxTq / 1.3f;
				hpInput = Mathf.Round(num15 * 10f / 100f + num15);
				torqueInput = Mathf.Round(num16 * 20f / 100f + num16);
			}
			else if (IsSetItem(28))
			{
				float num17 = maxHp / 1.3f;
				float num18 = maxTq / 1.4f;
				hpInput = Mathf.Round(num17 * 10f / 100f + num17);
				torqueInput = Mathf.Round(num18 * 20f / 100f + num18);
			}
			else
			{
				hpInput = maxHp * 10f / 100f + maxHp;
				torqueInput = maxTq * 20f / 100f + maxTq;
			}
			break;
		case 17:
			if (IsSetItem(17))
			{
				hpInput = 0f;
				torqueInput = 0f;
				break;
			}
			price = 50000f;
			if (IsSetItem(16))
			{
				float num8 = maxHp / 1.1f;
				float num9 = maxTq / 1.2f;
				hpInput = Mathf.Round(num8 * 20f / 100f + num8);
				torqueInput = Mathf.Round(num9 * 30f / 100f + num9);
			}
			else if (IsSetItem(28))
			{
				float num10 = maxHp / 1.3f;
				float num11 = maxTq / 1.4f;
				hpInput = Mathf.Round(num10 * 20f / 100f + num10);
				torqueInput = Mathf.Round(num11 * 30f / 100f + num11);
			}
			else
			{
				hpInput = maxHp * 20f / 100f + maxHp;
				torqueInput = maxTq * 30f / 100f + maxTq;
			}
			break;
		case 18:
			price = 3000f;
			TyreTypeInput = TyreType.Offroad;
			break;
		case 19:
			price = 3000f;
			TyreTypeInput = TyreType.Touring;
			break;
		case 20:
			TyreTypeInput = TyreType.Touring;
			price = 6000f;
			break;
		case 21:
			TyreTypeInput = TyreType.Slick;
			price = 5000f;
			break;
		case 22:
			TyreTypeInput = TyreType.Slick;
			price = 5000f;
			break;
		case 23:
			TyreTypeInput = TyreType.Slick;
			price = 10000f;
			break;
		case 24:
		{
			if (IsSetItem(24))
			{
				hpInput = 0f;
				torqueInput = 0f;
				break;
			}
			price = 1500f;
			float num19 = maxHp;
			float num20 = maxTq;
			hpInput = Mathf.Round(num19 * 3f / 100f + num19);
			torqueInput = Mathf.Round(num20 * 7f / 100f + num20);
			break;
		}
		case 25:
		{
			if (IsSetItem(25))
			{
				hpInput = 0f;
				torqueInput = 0f;
				break;
			}
			price = 4000f;
			float num12 = 1f;
			if (IsSetItem(16))
			{
				num12 = 1.2f;
			}
			else if (IsSetItem(17))
			{
				num12 = 1.3f;
			}
			else if (IsSetItem(28))
			{
				num12 = 1.4f;
			}
			float num13 = maxHp;
			float num14 = maxTq;
			hpInput = Mathf.Round(num12 * num13 * 5f / 100f + num13);
			torqueInput = Mathf.Round(num12 * num14 * 10f / 100f + num14);
			break;
		}
		case 26:
			price = 5000f;
			brakeInput = 3000f;
			break;
		case 27:
		{
			if (IsSetItem(27))
			{
				hpInput = 0f;
				torqueInput = 0f;
				break;
			}
			price = 5000f;
			float num5 = 1f;
			if (IsSetItem(16))
			{
				num5 = 1.04f;
			}
			else if (IsSetItem(17))
			{
				num5 = 1.1f;
			}
			else if (IsSetItem(28))
			{
				num5 = 1.12f;
			}
			float num6 = maxHp;
			float num7 = maxTq;
			hpInput = Mathf.Round(num5 * num6 * 5f / 100f + num6);
			torqueInput = Mathf.Round(num5 * num7 * 5f / 100f + num7);
			break;
		}
		case 28:
			if (IsSetItem(28))
			{
				hpInput = 0f;
				torqueInput = 0f;
				break;
			}
			price = 200000f;
			if (IsSetItem(16))
			{
				float num = maxHp / 1.1f;
				float num2 = maxTq / 1.2f;
				hpInput = Mathf.Round(num * 30f / 100f + num);
				torqueInput = Mathf.Round(num2 * 40f / 100f + num2);
			}
			else if (IsSetItem(17))
			{
				float num3 = maxHp / 1.2f;
				float num4 = maxTq / 1.3f;
				hpInput = Mathf.Round(num3 * 30f / 100f + num3);
				torqueInput = Mathf.Round(num4 * 40f / 100f + num4);
			}
			else
			{
				hpInput = maxHp * 30f / 100f + maxHp;
				torqueInput = maxTq * 40f / 100f + maxTq;
			}
			break;
		case 29:
			price = 20000f;
			shiftTimeInput = 0.1f;
			break;
		case 30:
			price = 5000f;
			break;
		case 31:
			price = 5000f;
			break;
		}
		if ((int)obscuredInt == 0)
		{
			if ((float)price <= money || PlayerDatas.IsBought(itemIndex))
			{
				if ((float)hpInput != 0f)
				{
					if ((float)hpInput > 2000f)
					{
						hpInput = 2000f;
					}
					maxHp = hpInput;
					ObscuredPrefs.SetFloat("maxPower" + V.CarId.ToString(), maxHp);
				}
				if ((float)torqueInput != 0f)
				{
					if ((float)torqueInput > 3000f)
					{
						torqueInput = 3000f;
					}
					maxTq = torqueInput;
					ObscuredPrefs.SetFloat("maxTorque" + V.CarId.ToString(), maxTq);
				}
				if ((float)maxPowerRpmInptut != 0f)
				{
					maxHpRpm = maxPowerRpmInptut;
					ObscuredPrefs.SetFloat("maxPowerRpm" + V.CarId.ToString(), maxHpRpm);
				}
				if ((float)maxTorqueRPMInput != 0f)
				{
					maxTqRpm = maxTorqueRPMInput;
					ObscuredPrefs.SetFloat("maxTorqueRPM" + V.CarId.ToString(), maxTqRpm);
				}
				if (brakeInput != 0f)
				{
					ObscuredPrefs.SetFloat("brake" + V.CarId.ToString(), brakeInput);
				}
				Mathf.Clamp(maxTqRpm, 500f, maxHpRpm * 0.75f);
				Mathf.Clamp(maxHp, 10f, maxTq);
				Mathf.Clamp(maxTq, maxHp, maxHp * 8f);
				if (itemIndex == 30)
				{
					ObscuredPrefs.SetInt("burble" + V.CarId.ToString(), 1);
				}
				if (itemIndex == 31)
				{
					ObscuredPrefs.SetInt("turboShot" + V.CarId.ToString(), 1);
				}
				SetItem(itemIndex);
				Label_set();
				if (!PlayerDatas.IsBought(itemIndex))
				{
					AdvancedInt advancedInt = new AdvancedInt((int)(float)price);
					CurrencyManagerController.Instance.TryMoneySpend(out var _, advancedInt);
				}
				menuControl.BuyItem(parent);
				setAllZero();
				menuControl.updateAlliTemes();
			}
			else
			{
				menuControl.ShowNotEnoughMoney();
			}
		}
		else
		{
			itemIndex = obscuredInt;
			_powertrain.CalcMaxPowerMaxTorque(out var maxHp2, out maxWhp, out var maxTq2, out var _, out var _);
			string text = chooseColor(hpInput, maxHp2);
			string text2 = chooseColor(torqueInput, maxTq2);
			_ = IsManualInput;
			_ = 1f;
			Label[0].text = white + "HP:" + end_color + orange + maxHp2 + "hp " + end_color + text + white + "\nTorque:" + end_color + orange + maxTq2 + "Nm " + end_color + text2 + white + " \nDriving Wheel: " + end_color + orange + DrivingWheel + end_color;
			Label[1].text = Label[0].text;
		}
	}

	public void update_label(GameObject parent)
	{
		UpdateLabel(parent, (int)CurrencyManagerController.Money);
	}

	public void setAllZero()
	{
		hpInput = 0f;
		torqueInput = 0f;
		massInput = 0f;
		gearsInput = 0f;
		CdInput = 0f;
		shiftTimeInput = 0f;
		maxRpmInput = 0f;
		maxTorqueRPMInput = 0f;
		maxPowerRpmInptut = 0f;
		price = 0f;
		EngineMassInput = 0f;
		brakeInput = 0f;
	}

	private string chooseColor(float input, float org)
	{
		string text = "";
		if (input != 0f)
		{
			float num = input - org;
			string text2 = "";
			text2 = ((num < 0f) ? ("(" + (input - org) + ")" + end_color) : ((!(num > 0f)) ? (end_color ?? "") : ("(+" + (input - org) + ")" + end_color)));
			if (org > input)
			{
				return red + text2;
			}
			return green + text2;
		}
		return orange + end_color;
	}

	public void ShowResult(float car1FinisthTime, float car2FinisthTime, float timeTo100, float timeTo500, float maxSpeed1, bool updateResalt, float timeTo200)
	{
	}

	public void newEngine()
	{
		PlayerPrefs.SetInt("isSet" + 16 + "Car" + V.CarId.ToString(), 0);
		PlayerPrefs.SetInt("isSet" + 17 + "Car" + V.CarId.ToString(), 0);
		PlayerPrefs.SetInt("isSet" + 24 + "Car" + V.CarId.ToString(), 0);
		PlayerPrefs.SetInt("isSet" + 25 + "Car" + V.CarId.ToString(), 0);
		PlayerPrefs.SetInt("isSet" + 27 + "Car" + V.CarId.ToString(), 0);
		PlayerPrefs.SetInt("isSet" + 28 + "Car" + V.CarId.ToString(), 0);
		menuControl.updateAlliTemes();
	}

	private void SetItem(ObscuredInt itemIndex)
	{
		if ((int)itemIndex > 0 && (int)itemIndex < 11)
		{
			newEngine();
			for (int i = 1; i <= 10; i++)
			{
				if ((int)itemIndex == i)
				{
					PlayerPrefs.SetInt("isSet" + i + "Car" + V.CarId.ToString(), 1);
				}
				else
				{
					PlayerPrefs.SetInt("isSet" + i + "Car" + V.CarId.ToString(), 0);
				}
			}
		}
		else if ((int)itemIndex > 9 && (int)itemIndex < 16)
		{
			for (int j = 10; j <= 15; j++)
			{
				if ((int)itemIndex == j)
				{
					PlayerPrefs.SetInt("isSet" + j + "Car" + V.CarId.ToString(), 1);
				}
				else
				{
					PlayerPrefs.SetInt("isSet" + j + "Car" + V.CarId.ToString(), 0);
				}
			}
		}
		else if ((int)itemIndex > 17 && (int)itemIndex < 24)
		{
			for (int k = 18; k <= 23; k++)
			{
				if ((int)itemIndex == k)
				{
					PlayerPrefs.SetInt("isSet" + k + "Car" + V.CarId.ToString(), 1);
				}
				else
				{
					PlayerPrefs.SetInt("isSet" + k + "Car" + V.CarId.ToString(), 0);
				}
			}
		}
		else if ((int)itemIndex == 16 || (int)itemIndex == 17 || (int)itemIndex == 28)
		{
			if ((int)itemIndex == 16)
			{
				PlayerPrefs.SetInt("isSet" + 16 + "Car" + V.CarId.ToString(), 1);
				PlayerPrefs.SetInt("isSet" + 17 + "Car" + V.CarId.ToString(), 0);
				PlayerPrefs.SetInt("isSet" + 28 + "Car" + V.CarId.ToString(), 0);
				ObscuredPrefs.SetInt("hasTurbo" + V.CarId.ToString(), 1);
			}
			else if ((int)itemIndex == 17)
			{
				PlayerPrefs.SetInt("isSet" + 17 + "Car" + V.CarId.ToString(), 1);
				PlayerPrefs.SetInt("isSet" + 16 + "Car" + V.CarId.ToString(), 0);
				PlayerPrefs.SetInt("isSet" + 28 + "Car" + V.CarId.ToString(), 0);
				ObscuredPrefs.SetInt("hasTurbo" + V.CarId.ToString(), 2);
			}
			else if ((int)itemIndex == 28)
			{
				PlayerPrefs.SetInt("isSet" + 17 + "Car" + V.CarId.ToString(), 0);
				PlayerPrefs.SetInt("isSet" + 16 + "Car" + V.CarId.ToString(), 0);
				PlayerPrefs.SetInt("isSet" + 28 + "Car" + V.CarId.ToString(), 1);
				ObscuredPrefs.SetInt("hasTurbo" + V.CarId.ToString(), 3);
			}
			menuControl.updateAlliTemes();
		}
		else
		{
			ObscuredInt obscuredInt = itemIndex;
			PlayerPrefs.SetInt("isSet" + obscuredInt.ToString() + "Car" + V.CarId.ToString(), 1);
		}
	}

	public bool IsSetItem(int itemIndex)
	{
		if (PlayerPrefs.GetInt("isSet" + itemIndex + "Car" + V.CarId.ToString(), 0) == 1)
		{
			return true;
		}
		return false;
	}
}
