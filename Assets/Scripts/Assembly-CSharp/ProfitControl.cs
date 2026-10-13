using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.PlayerProperty.Business;

public class ProfitControl : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI TextMoney = default;

	[HideInInspector]
	public int idOfBuilding;

	public AllwaysLookAtCam allwaysLookAtCam;

	private IBusinessProfitControl _businessProfitControl;

	private int _lastProfit = -1;

	[Inject]
	public void Construct(IBusinessProfitControl businessProfitControl)
	{
		_businessProfitControl = businessProfitControl;
	}

	public void StartEarnMoney()
	{
	}

	private void Update()
	{
		if ((bool)allwaysLookAtCam && allwaysLookAtCam.nowLooking)
		{
			int profit = _businessProfitControl.GetProfit(idOfBuilding);
			if (profit != _lastProfit)
			{
				_lastProfit = profit;
				TextMoney.text = "+" + profit;
			}
		}
	}
}
