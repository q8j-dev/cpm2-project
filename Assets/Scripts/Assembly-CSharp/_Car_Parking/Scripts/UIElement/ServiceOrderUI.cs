using System;
using CPMEngine;
using Service;
using Service.Abstract;
using Service.Cargo;
using Service.Delivery;
using Service.Taxi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement
{
	public class ServiceOrderUI : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI Name;

		[SerializeField]
		private TextMeshProUGUI Location;

		[SerializeField]
		private TextMeshProUGUI Distance;

		[SerializeField]
		private TextMeshProUGUI Time;

		[SerializeField]
		private TextMeshProUGUI Award;

		[SerializeField]
		private Sprite Normal;

		[SerializeField]
		private Sprite Selected;

		[SerializeField]
		private Sprite ValidIcon;

		[SerializeField]
		private Sprite InvalidIcon;

		[SerializeField]
		private GameObject Line;

		[SerializeField]
		private GameObject ServiceTitlesColumn;

		[SerializeField]
		private GameObject DraggyTitlesColumn;

		[SerializeField]
		private Image FirstKmhValidIcon;

		[SerializeField]
		private Image SecondKmhValidIcon;

		[SerializeField]
		private Image MphValidIcon;

		[SerializeField]
		private Image MileValidIcon;

		private PhoneWindow _phoneWindow;

		[field: SerializeField]
		public Button OrderButton { get; private set; }

		public Service.Abstract.Order Order { get; private set; }

		public int Id { get; private set; }

		public void Init(Location departure, Location destination, uint price, float distance, string destinationName, string orderName, int id, PhoneWindow phoneWindow, int typeId = 0)
		{
			_phoneWindow = phoneWindow;
			Name.text = orderName;
			Location.text = destinationName;
			if (price == 0)
			{
				Line.SetActive(value: true);
				Award.gameObject.SetActive(value: false);
			}
			else
			{
				Line.SetActive(value: false);
				Award.gameObject.SetActive(value: true);
				int num = Mathf.RoundToInt(Utils.GetConvertedMileage(price));
				Award.text = "$" + num;
			}
			switch (phoneWindow.ServiceType)
			{
			case TYPE.TAXI:
				Order = new Service.Taxi.Order(departure.position, destination.position, price, distance, orderName, destinationName);
				break;
			case TYPE.DELIVERY:
				Order = new Service.Delivery.Order(typeId, departure.position, destination.position, price, distance, orderName, destinationName);
				break;
			case TYPE.CARGO:
				Order = new Service.Cargo.Order(typeId, departure, destination, price, distance, orderName, destinationName);
				break;
			}
			Distance.text = $"{Utils.GetConvertedMileage(distance):0.000} {Utils.CurrentMetric}";
			Id = id;
			float a = Vector3.Distance(Order.GetDeparture() + V.GlobalOffset, Order.GetDestination() + V.GlobalOffset);
			float b = GPSTracker.DistanceBetween(Order.GetDeparture() + V.GlobalOffset, Order.GetDestination() + V.GlobalOffset);
			float num2 = 3f * Mathf.Sqrt(Mathf.Max(a, b));
			Order.SetTime(num2);
			TimeSpan timeSpan = TimeSpan.FromSeconds(num2);
			Time.text = timeSpan.ToString("mm\\:ss");
		}

		public void Init(Service.Abstract.Order order)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(order.GetTime());
			Name.text = order.GetName();
			Location.text = order.GetLocation();
			if (order.GetPrice() == 0)
			{
				Line.SetActive(value: true);
				Award.gameObject.SetActive(value: false);
			}
			else
			{
				Line.SetActive(value: false);
				Award.gameObject.SetActive(value: true);
				int num = Mathf.RoundToInt(Utils.GetConvertedMileage(order.GetPrice()));
				Award.text = "$" + num;
			}
			Distance.text = $"{Utils.GetConvertedMileage(order.GetDistance()):0.000} {Utils.CurrentMetric}";
			Time.text = timeSpan.ToString("mm\\:ss");
			OrderButton.interactable = false;
		}

		public void Select(bool isSelected)
		{
			OrderButton.image.sprite = (isSelected ? Selected : Normal);
		}

		private void Start()
		{
			OrderButton.onClick.AddListener(SelectOrder);
		}

		private void SelectOrder()
		{
			Select(isSelected: true);
			GPSTracker.ClearPath(forget: true);
			Order.CorrectDepartureAndDestination();
			GPSTracker.DrawPath(Order.GetDeparture(), Order.GetDestination(), GPSTracker.GPS.SERVICE);
			_phoneWindow.OnOrderSelected(Id);
		}
	}
}
