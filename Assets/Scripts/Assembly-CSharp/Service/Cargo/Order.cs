using System;
using Service.Abstract;
using UnityEngine;

namespace Service.Cargo
{
	[Serializable]
	public class Order : Service.Abstract.Order
	{
		[HideInInspector]
		public int id;

		[HideInInspector]
		public new Location departure;

		[HideInInspector]
		public new Location destination;

		public Order(int id, Location departure, Location destination, uint price, float distance, string orderName, string destinationName)
		{
			this.id = id;
			departure.position -= Vector3.up * 0.7f;
			destination.position -= Vector3.up * 0.7f;
			this.departure = departure;
			this.destination = destination;
			Init(departure.position, destination.position, price, distance, orderName, destinationName);
		}
	}
}
