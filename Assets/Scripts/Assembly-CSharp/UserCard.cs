using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

public class UserCard : MonoBehaviour
{
	public RawImage carImage;

	[SerializeField]
	private FlagDrawer flagDrawer;

	[SerializeField]
	private TextMeshProUGUI[] horsePowerTexts;

	[SerializeField]
	private TextMeshProUGUI mass;

	[SerializeField]
	private TextMeshProUGUI torque;

	[SerializeField]
	private TextMeshProUGUI[] userNameTexts;

	[SerializeField]
	private TextMeshProUGUI winnerCountText;

	[SerializeField]
	private TextMeshProUGUI loseCountText;

	[SerializeField]
	private TextMeshProUGUI raceCountText;

	[SerializeField]
	private TextMeshProUGUI overallText;

	[SerializeField]
	private TextMeshProUGUI reactionText;

	[SerializeField]
	private TextMeshProUGUI timeText;

	[SerializeField]
	private TextMeshProUGUI speedText;

	[SerializeField]
	private TextMeshProUGUI raceStateText;

	private Powertrain _powertrain;

	private MainCarValues mainCarValues;

	private OneRaceResult _lastRaceResultResult;

	public GameObject raceStatusIcon;

	public bool isLeftCard;

	public void Init(MainCarValues carValues, Powertrain powertrain, bool setFlag)
	{
		if ((bool)raceStatusIcon)
		{
			raceStatusIcon.gameObject.SetActive(value: false);
		}
		if ((bool)raceStateText)
		{
			raceStateText.gameObject.SetActive(value: false);
		}
		_powertrain = powertrain;
		if (setFlag)
		{
			SetImages(carValues.CarID);
			if ((bool)flagDrawer)
			{
				flagDrawer.Show();
				flagDrawer.Set(carValues);
			}
		}
		else if ((bool)flagDrawer)
		{
			flagDrawer.Hide();
		}
		TextMeshProUGUI[] array = userNameTexts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = carValues.NickName;
		}
		CalculateSpec();
	}

	public void Init(MainCarValues carValues, Powertrain powertrain)
	{
		_powertrain = powertrain;
		mainCarValues = carValues;
		SetImages(carValues.CarID);
		TextMeshProUGUI[] array = userNameTexts;
		foreach (TextMeshProUGUI textMeshProUGUI in array)
		{
			if ((bool)textMeshProUGUI)
			{
				textMeshProUGUI.text = carValues.NickName;
			}
		}
		CalculateSpec();
	}

	private void Start()
	{
		CalculateSpec();
	}

	private void CalculateSpec()
	{
		if ((bool)_powertrain)
		{
			_powertrain.CalcMaxPowerMaxTorque(out var maxHp, out var _, out var maxTq, out var _, out var _);
			TextMeshProUGUI[] array = horsePowerTexts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].text = Utils.GetPower(maxHp) ?? "";
			}
			if ((bool)torque)
			{
				torque.text = Utils.GetTorque(maxTq) + " " + Utils.GetTorquePostfix;
			}
			if ((bool)mass)
			{
				mass.text = Utils.GetMass(_powertrain.GetComponent<Rigidbody>().mass) ?? "";
			}
		}
		else
		{
			TextMeshProUGUI[] array = horsePowerTexts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].text = string.Empty;
			}
			if ((bool)torque)
			{
				torque.text = string.Empty;
			}
			if ((bool)mass)
			{
				mass.text = string.Empty;
			}
		}
	}

	private async UniTask SetImages(int id)
	{
		RawImage rawImage = carImage;
		rawImage.texture = (await Resources.LoadAsync<Texture>($"UI/CarImages/b{id}")) as Texture;
	}

	public void FillLastResultData(OneRaceResult oneRaceResult)
	{
		if (oneRaceResult != null)
		{
			_lastRaceResultResult = oneRaceResult;
			overallText.text = $"{oneRaceResult.quaterMilResult + oneRaceResult.RT:F3} s";
			reactionText.text = $"{oneRaceResult.RT:F3} s";
			timeText.text = $"{oneRaceResult.quaterMilResult:F3} s";
			speedText.text = $"{Utils.GetConvertedMileage(oneRaceResult.exitSpeed):F0} {Utils.CurrentSpeedMetric}";
		}
	}

	public void SetCurrentMileageType()
	{
		if (_lastRaceResultResult != null)
		{
			speedText.text = $"{Utils.GetConvertedMileage(_lastRaceResultResult.exitSpeed):F0} {Utils.CurrentSpeedMetric}";
		}
	}

	public void ShowUserStatistics(int win, int lose)
	{
		winnerCountText.text = win.ToString();
		raceCountText.text = (lose + win).ToString();
		loseCountText.text = lose.ToString();
	}

	public void FillResultPanel(Dragy dragy, float time)
	{
		float reactionTime = dragy.reactionTime;
		float num = time;
		if (reactionTime > 0f)
		{
			time = reactionTime + num;
		}
		string text = "";
		Color clear = Color.clear;
		if (dragy.reason == Dragy.LostReason.falseStart || reactionTime < 0f)
		{
			text = "False start";
			clear = Color.red;
			raceStatusIcon.gameObject.SetActive(value: false);
		}
		else if (!dragy.lost)
		{
			text = "Winner";
			clear = Color.green;
			raceStatusIcon.gameObject.SetActive(value: true);
		}
		else
		{
			text = new LocalizedString("UserCard.Lose");
			clear = Color.red;
			raceStatusIcon.gameObject.SetActive(value: false);
		}
		overallText.text = $"{time:F3}";
		reactionText.text = $"{reactionTime:F3}";
		speedText.text = $"{Utils.GetConvertedMileage(dragy.exitSpeed):F0} {Utils.CurrentSpeedMetric}";
		timeText.text = $"{num:F3}";
		raceStateText.gameObject.SetActive(value: true);
		raceStateText.text = text;
		raceStateText.color = clear;
	}
}
