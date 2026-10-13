using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class OccupiedCarSlotsUI : MonoBehaviour
	{
		[SerializeField]
		private int GarageIndex;

		[SerializeField]
		private GameObject TextContainer;

		[SerializeField]
		private TMP_Text CarSlotsText;

		private CarSlotsObserver _carSlotsObserver;

		private int _manualOccupied = -1;

		[Inject]
		public void Construct(CarSlotsObserver carSlotsObserver)
		{
			_carSlotsObserver = carSlotsObserver;
		}

		private void Start()
		{
			UpdateState(_carSlotsObserver.Current);
			_carSlotsObserver.OnChanged += UpdateState;
		}

		public void Init(int garageIndex)
		{
			GarageIndex = garageIndex;
			_manualOccupied = -1;
			UpdateState(_carSlotsObserver.Current);
		}

		public void SetOccupiedCount(int occupied)
		{
			_manualOccupied = occupied;
			CarSlotsInfo carSlotsInfo = _carSlotsObserver.Current.CarSlotsInfoArray[GarageIndex];
			CarSlotsText.text = $"{_manualOccupied}/{carSlotsInfo.Purchased}";
		}

		private void UpdateState(GarageSlotsInfo garageSlotsInfo)
		{
			if (garageSlotsInfo.CarSlotsInfoArray != null && GarageIndex < garageSlotsInfo.CarSlotsInfoArray.Length)
			{
				CarSlotsInfo carSlotsInfo = garageSlotsInfo.CarSlotsInfoArray[GarageIndex];
				TextContainer.SetActive(carSlotsInfo.IsBought);
				int num = ((_manualOccupied >= 0) ? _manualOccupied : carSlotsInfo.Occupied);
				CarSlotsText.text = $"{num}/{carSlotsInfo.Purchased}";
			}
		}

		private void OnDestroy()
		{
			if (_carSlotsObserver != null)
			{
				_carSlotsObserver.OnChanged -= UpdateState;
			}
		}
	}
}
