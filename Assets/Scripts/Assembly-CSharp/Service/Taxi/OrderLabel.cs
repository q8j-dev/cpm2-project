using System;
using Service.Abstract;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Service.Taxi
{
	public class OrderLabel : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		[SerializeField]
		private TextMeshProUGUI timeLabel;

		[SerializeField]
		private TextMeshProUGUI distanceLabel;

		[SerializeField]
		private TextMeshProUGUI locationLabel;

		[SerializeField]
		private TextMeshProUGUI priceLabel;

		public Button buttonLabel;

		public Order order { get; private set; }

		public void Init(Vector3 departure, Vector3 destination, uint price, float distance, string destinationName, string name)
		{
			nameLabel.text = "Name: " + name;
			locationLabel.text = "Location: " + destinationName;
			priceLabel.text = price + "/km";
			distanceLabel.text = $"{distance:0.000}";
			order = new Order(departure, destination, price, distance, nameLabel.text, destinationName);
			float a = Vector3.Distance(order.GetDeparture() + V.GlobalOffset, order.GetDestination() + V.GlobalOffset);
			float b = GPSTracker.DistanceBetween(order.GetDeparture() + V.GlobalOffset, order.GetDestination() + V.GlobalOffset);
			float num = 3f * Mathf.Sqrt(Mathf.Max(a, b));
			order.SetTime(num);
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			timeLabel.text = timeSpan.ToString("mm\\:ss");
		}

		public void Init(Service.Abstract.Order order)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(order.GetTime());
			nameLabel.text = "Name: " + order.GetName();
			locationLabel.text = "Location: " + order.GetLocation();
			priceLabel.text = "Price: " + order.GetPrice() + "/km";
			distanceLabel.text = "Distance: " + $"{order.GetDistance():0.000}km";
			timeLabel.text = "Time: " + timeSpan.ToString("mm\\:ss");
		}
	}
}
