using System;
using Service.Abstract;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Service.Delivery
{
	public class OrderLabel : MonoBehaviour
	{
		[SerializeField]
		private RawImage texture = default;

		[SerializeField]
		private TextMeshProUGUI nameLabel = default;

		[SerializeField]
		private TextMeshProUGUI timeLabel = default;

		[SerializeField]
		private TextMeshProUGUI distanceLabel = default;

		[SerializeField]
		private TextMeshProUGUI locationLabel = default;

		[SerializeField]
		private TextMeshProUGUI priceLabel = default;

		public Order order { get; private set; }

		public void Init(int id, Location departure, Location destination, uint price, float distance, string name)
		{
			nameLabel.text = "Name: " + name;
			locationLabel.text = "Location: " + destination.city;
			priceLabel.text = "";
			distanceLabel.text = $"{distance:0.000}";
			texture.texture = Resources.Load<Texture2D>($"_Services/Delivery/Textures/{id}");
			float a = Vector3.Distance(order.GetDeparture() + V.GlobalOffset, order.GetDestination() + V.GlobalOffset);
			float b = GPSTracker.DistanceBetween(order.GetDeparture() + V.GlobalOffset, order.GetDestination() + V.GlobalOffset);
			float num = 5f * Mathf.Sqrt(Mathf.Max(a, b));
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
