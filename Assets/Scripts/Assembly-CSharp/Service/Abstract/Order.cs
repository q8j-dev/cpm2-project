using System;
using UnityEngine;

namespace Service.Abstract
{
	[Serializable]
	public abstract class Order : IOrder
	{
		[SerializeField]
		protected Texture texture;

		private bool _isCorrected;

		[field: SerializeField]
		protected Vector3 departure { get; private set; }

		protected Vector3 destination { get; private set; }

		protected uint price { get; private set; }

		protected float distance { get; private set; }

		protected float time { get; private set; }

		protected string name { get; private set; }

		protected string location { get; private set; }

		protected float startDistance { get; private set; }

		public uint Price
		{
			get
			{
				return price;
			}
			set
			{
				price = value;
			}
		}

		public virtual void Init(Vector3 departure, Vector3 destination, uint price, float distance, string name = null, string location = null)
		{
			this.departure = departure;
			this.destination = destination;
			this.price = price;
			this.distance = distance;
			this.name = name;
			this.location = location;
			startDistance = 0f;
			time = time;
		}

		public virtual void SetTime(float time)
		{
			this.time = time;
		}

		public virtual Vector3 GetDeparture()
		{
			return departure;
		}

		public virtual Vector3 GetDestination()
		{
			return destination;
		}

		public virtual float GetStartDistance()
		{
			return startDistance;
		}

		public virtual float GetDistance()
		{
			return distance;
		}

		public virtual uint GetPrice()
		{
			return price;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual string GetLocation()
		{
			return location;
		}

		public virtual float GetTime()
		{
			return time;
		}

		public void CorrectDepartureAndDestination()
		{
			if (!_isCorrected && (bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null && (bool)V.CoreGameManager.FreeDriveProvider.MapCamera)
			{
				MapCamera mapCamera = V.CoreGameManager.FreeDriveProvider.MapCamera;
				departure += V.GlobalOffset;
				destination += V.GlobalOffset;
				departure = mapCamera.GetCorrectedPosition(departure) - V.GlobalOffset;
				destination = mapCamera.GetCorrectedPosition(destination) - V.GlobalOffset;
				_isCorrected = true;
			}
		}
	}
}
