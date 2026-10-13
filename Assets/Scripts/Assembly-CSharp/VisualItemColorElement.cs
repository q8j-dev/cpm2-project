using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.DataTypes;

public class VisualItemColorElement : VisualItemElement
{
	public TextMeshProUGUI colorMatName;

	public CarMatType carMatType;

	private IPaintDataService _paintDataService;

	[Inject]
	public void Construct(IPaintDataService paintDataService)
	{
		_paintDataService = paintDataService;
	}

	private void Start()
	{
		Button.onValueChanged.RemoveListener(ChangeColor);
		Button.onValueChanged.AddListener(ChangeColor);
	}

	private void OnEnable()
	{
		_paintDataService.OnBought += OnPaintBought;
	}

	private void OnDisable()
	{
		_paintDataService.OnBought -= OnPaintBought;
	}

	private void OnPaintBought(CarMatType carMatType)
	{
		if (this.carMatType == carMatType)
		{
			UpdateView();
		}
	}

	public override void ChangeColor(bool isOn)
	{
		if (!isOn)
		{
			elementImage.color = Color.white;
		}
	}

	public void UpdateView()
	{
		if (_paintDataService.CheckPaintBought(carMatType))
		{
			elementPrice.text = "0";
			IsBought = true;
		}
	}
}
