using System;
using System.Collections.Generic;
using EasyHaptic_EvilBurgers;
using UnityEngine;
using UnityEngine.UI;

public class EasyHapticExample : MonoBehaviour
{
	[SerializeField]
	private InputField millisecondsInput = default;

	[SerializeField]
	private InputField amplitudeInput = default;

	[SerializeField]
	private InputField sharpnessInput = default;

	[SerializeField]
	private Text systemInfo = default;

	[SerializeField]
	private Button btnPrefab = default;

	[SerializeField]
	private Gradient buttonsColor = default;

	[SerializeField]
	private GameObject buttonsGroup = default;

	private List<Button> allButtons = new List<Button>();

	private void Start()
	{
		Init();
	}

	public void PlayCustomVibration()
	{
		double result = 0.0;
		float result2 = 0f;
		float result3 = 0f;
		millisecondsInput.text = millisecondsInput.text.Replace('.', ',');
		double.TryParse(millisecondsInput.text, out result);
		float.TryParse(amplitudeInput.text, out result2);
		float.TryParse(sharpnessInput.text, out result3);
		EasyHaptic.PlayCustom(new CustomVibrationData
		{
			amplitude = result2,
			durationInSeconds = result,
			sharpness = result3
		});
	}

	private void Init()
	{
		CreatePredifinedButtons();
		SetButtonsColor();
		ShowSystemInfo();
	}

	private void CreatePredifinedButtons()
	{
		EVibrationType[] array = (EVibrationType[])Enum.GetValues(typeof(EVibrationType));
		for (int i = 0; i < array.Length; i++)
		{
			EVibrationType type = array[i];
			Button button = UnityEngine.Object.Instantiate(btnPrefab, buttonsGroup.transform);
			button.GetComponentInChildren<Text>().text = type.ToString();
			button.onClick.AddListener(() =>
			{
				EasyHaptic.Play(type);
			});
			allButtons.Add(button);
		}
		btnPrefab.gameObject.SetActive(value: false);
	}

	private void SetButtonsColor()
	{
		if (allButtons.Count == 0)
		{
			Debug.LogError("Predifined buttons not created, please check all prefab refs");
			return;
		}
		for (int i = 0; i < allButtons.Count; i++)
		{
			Button button = allButtons[i];
			float time = (float)i / (float)allButtons.Count;
			Color color = buttonsColor.Evaluate(time);
			button.GetComponent<Image>().color = color;
		}
	}

	private void ShowSystemInfo()
	{
		string text = "Device Name - " + SystemInfo.deviceName + " \n ";
		text = text + "Device model - " + SystemInfo.deviceModel + " \n";
		text = text + "OS version - " + SystemInfo.operatingSystem;
		systemInfo.text = text;
	}
}
