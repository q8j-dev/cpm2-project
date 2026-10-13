using System;
using CPM.Database;
using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class CarSelectorPanel : MonoBehaviour
	{
		[SerializeField]
		private RawImage CarImage = default;

		[SerializeField]
		private TextMeshProUGUI CarClassText = default;

		[SerializeField]
		private Button SelectCarButton = default;

		[SerializeField]
		private Image GroupImage = default;

		[SerializeField]
		private Sprite[] GroupSprites = default;

		private ICarInfoService _carInfoService;

		public event Action SelectClicked;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		private void Construct(ICarInfoService carInfoService)
		{
			_carInfoService = carInfoService;
		}

		private void OnEnable()
		{
			SelectCarButton?.onClick.AddListener(OnSelectClicked);
		}

		private void OnDisable()
		{
			SelectCarButton?.onClick.RemoveListener(OnSelectClicked);
		}

		protected virtual void OnSelectClicked()
		{
			SelectClicked?.Invoke();
		}

		public VehicleClass ParseCarValues()
		{
			int carID = _carInfoService.GetFromSlot(V.SlotID).CarID;
			VehicleEntityProvider component = V.Car.GetComponent<VehicleEntityProvider>();
			SetCarIcon(carID);
			VehicleClass classVehicle = Utils.GetClassVehicle(component.CarEntity.GetValue().Points, carID);
			SetClassText(classVehicle);
			int carClassImage = (int)classVehicle;
			SetCarClassImage(carClassImage);
			return classVehicle;
		}

		public void SetClassText(VehicleClass vehicleClass)
		{
			CarClassText.text = vehicleClass.ToString();
		}

		public void SetCarIcon(int carId)
		{
			Texture texture = Resources.Load<Texture>("UI/CarImages/b" + carId);
			if (CarImage != null)
			{
				CarImage.texture = texture;
				CarImage.color = Color.white;
				CarImage.gameObject.SetActive(value: true);
			}
		}

		public void SetCarClassImage(int vehicleClass)
		{
			if (GroupImage != null && GroupSprites != null && GroupSprites.Length > vehicleClass)
			{
				GroupImage.sprite = GroupSprites[vehicleClass];
			}
		}
	}
}
