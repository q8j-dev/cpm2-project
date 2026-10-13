using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;

public class ExteriorTunningUI : MonoBehaviour
{
	public UIInventory _UIInventory;

	private CarEntity _carEntity;

	[Inject]
	public void Construct()
	{
	}

	public void Init(CarEntity carEntity)
	{
		_carEntity = carEntity;
	}

	public void OnItemClick(PartItem part)
	{
	}

	public async UniTask FillElements()
	{
	}

	public void OnEngineClick()
	{
	}
}
