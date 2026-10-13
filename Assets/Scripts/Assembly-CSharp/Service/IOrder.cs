using UnityEngine;

namespace Service
{
	public interface IOrder
	{
		void Init(Vector3 departure, Vector3 destination, uint price, float distance, string name, string location);

		Vector3 GetDeparture();

		Vector3 GetDestination();

		float GetStartDistance();

		float GetDistance();

		uint GetPrice();

		string GetName();

		string GetLocation();

		void SetTime(float time);
	}
}
