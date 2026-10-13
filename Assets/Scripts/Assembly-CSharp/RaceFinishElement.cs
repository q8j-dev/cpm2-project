using System;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

public class RaceFinishElement : MonoBehaviour
{
	public TextMeshProUGUI overAllText;

	public TextMeshProUGUI RTText;

	public TextMeshProUGUI cleanText;

	public TextMeshProUGUI speedText;

	public TextMeshProUGUI ft60Text;

	public TextMeshProUGUI ft330Text;

	public TextMeshProUGUI et1_8Text;

	public RawImage iconCar;

	public TextMeshProUGUI nameText;

	public FlagDrawer flagDrawer;

	public Color highLightedFinishLabelColor;

	public Color shamedFinishLabelColor;

	public Color breakOutColor;

	private DragRacingUI _dragRacingUI;

	private IPlayerStorageService _playerStorageService;

	[Inject]
	public void Construct(OptionalDependency<DragRacingUI> dragRacingUI, IPlayerStorageService playerStorageService)
	{
		_dragRacingUI = dragRacingUI.Value;
		_playerStorageService = playerStorageService;
	}

	public async UniTask ParseDatas(GameObject dragyCar, bool quaterMile)
	{
		if (!dragyCar)
		{
			overAllText.text = "-";
			RTText.text = "-";
			cleanText.text = "-";
			speedText.text = "-";
			ft60Text.text = "-";
			ft330Text.text = "-";
			et1_8Text.text = "-";
			iconCar.SetActive(active: false);
			nameText.text = "-";
			if ((bool)flagDrawer)
			{
				flagDrawer.Hide();
			}
		}
		else
		{
			if (!dragyCar.TryGetComponent<Dragy>(out var component))
			{
				return;
			}
			float num = component.GetResult(quaterMile);
			float result = component.GetResult(quaterMile);
			if (num == 0f)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			base.gameObject.SetActive(value: true);
			if (component.reactionTime > 0f)
			{
				num = component.reactionTime + result;
			}
			overAllText.text = $"{num:F3}";
			overAllText.color = highLightedFinishLabelColor;
			RTText.text = $"{component.reactionTime:F3}";
			RTText.color = highLightedFinishLabelColor;
			cleanText.text = $"{result:F3}";
			cleanText.color = highLightedFinishLabelColor;
			if (result < (float)_dragRacingUI.controller.timeClass && _dragRacingUI.controller.timeClass != 8)
			{
				cleanText.color = breakOutColor;
			}
			speedText.text = $"{Utils.GetConvertedMileage(component.exitSpeed):F3}";
			speedText.color = highLightedFinishLabelColor;
			ft60Text.text = $"{component.ft60:F3}";
			ft60Text.color = highLightedFinishLabelColor;
			ft330Text.text = $"{component.ft330:F3}";
			ft330Text.color = highLightedFinishLabelColor;
			et1_8Text.text = $"{component.et1_8:F3}";
			if (_dragRacingUI.controller.runDistance == DragRacingController.RunDistance.OneMile)
			{
				et1_8Text.text = $"{component.quarterMileResult:F3}";
				et1_8Text.color = highLightedFinishLabelColor;
			}
			else
			{
				et1_8Text.text = $"{component.et1_8:F3}";
				et1_8Text.color = highLightedFinishLabelColor;
			}
			if (!component.TryGetComponent<VehicleData>(out var component2))
			{
				return;
			}
			iconCar.SetActive(active: true);
			if ((bool)flagDrawer)
			{
				flagDrawer.Show();
			}
			Texture texture = Resources.Load<Texture>($"UI/CarImages/b{component2.CarId}");
			iconCar.texture = texture;
			if ((bool)component2.CarValues)
			{
				nameText.text = component2.CarValues.NickName;
				if ((bool)flagDrawer)
				{
					flagDrawer.Set(component2.CarValues);
				}
			}
		}
	}

	public async UniTask ParseDatas(Dragy dragy, Dragy opponentDragy, bool quaterMile)
	{
		float num = dragy.GetResult(quaterMile);
		float result = opponentDragy.GetResult(quaterMile);
		float result2 = dragy.GetResult(quaterMile);
		VehicleData component = dragy.GetComponent<VehicleData>();
		if (num == 0f)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		if (dragy.reactionTime > 0f)
		{
			num = dragy.reactionTime + result2;
		}
		overAllText.text = $"{num:F3}";
		overAllText.color = ((num < result) ? highLightedFinishLabelColor : shamedFinishLabelColor);
		RTText.text = $"{dragy.reactionTime:F3}";
		if (dragy.reactionTime < 0f)
		{
			RTText.color = Color.red;
		}
		else
		{
			RTText.color = ((dragy.reactionTime < opponentDragy.reactionTime) ? highLightedFinishLabelColor : shamedFinishLabelColor);
		}
		cleanText.text = $"{result2:F3}";
		cleanText.color = ((result2 < opponentDragy.GetResult(quaterMile)) ? highLightedFinishLabelColor : shamedFinishLabelColor);
		if (result2 < (float)_dragRacingUI.controller.timeClass && _dragRacingUI.controller.timeClass != 8)
		{
			cleanText.color = breakOutColor;
		}
		speedText.text = $"{Utils.GetConvertedMileage(dragy.exitSpeed):F0}";
		speedText.color = ((dragy.exitSpeed > opponentDragy.exitSpeed) ? highLightedFinishLabelColor : shamedFinishLabelColor);
		ft60Text.text = $"{dragy.ft60:F3}";
		ft60Text.color = ((dragy.ft60 < opponentDragy.ft60) ? highLightedFinishLabelColor : shamedFinishLabelColor);
		ft330Text.text = $"{dragy.ft330:F3}";
		ft330Text.color = ((dragy.ft330 < opponentDragy.ft330) ? highLightedFinishLabelColor : shamedFinishLabelColor);
		et1_8Text.text = $"{dragy.et1_8:F3}";
		if (_dragRacingUI.controller.runDistance == DragRacingController.RunDistance.OneMile)
		{
			et1_8Text.text = $"{dragy.quarterMileResult:F3}";
			et1_8Text.color = ((dragy.quarterMileResult < opponentDragy.quarterMileResult) ? highLightedFinishLabelColor : shamedFinishLabelColor);
		}
		else
		{
			et1_8Text.text = $"{dragy.et1_8:F3}";
			et1_8Text.color = ((dragy.et1_8 < opponentDragy.et1_8) ? highLightedFinishLabelColor : shamedFinishLabelColor);
		}
		if (!component || !component.CarValues)
		{
			return;
		}
		Texture texture = Resources.Load<Texture>($"UI/CarImages/b{component.CarId}");
		iconCar.texture = texture;
		iconCar.SetActive(active: true);
		nameText.text = ((dragy.gameObject == V.LastUsedCar) ? ((string)_playerStorageService.Name) : component.CarValues.NickName);
		try
		{
			if ((bool)flagDrawer)
			{
				flagDrawer.Show();
				flagDrawer.Set(component.CarValues);
			}
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}
}
