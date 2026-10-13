using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Offers
{
	[Serializable]
	[Preserve]
	public class OfferModel
	{
		[NonSerialized]
		public bool Enabled;

		[NonSerialized]
		public bool Inizilizated;

		public int ID;

		public int TimeTillEnd;

		public OfferPayloadModel Payload;

		public OfferSkinModel Skin;

		public int Version;

		private int _timeInit;

		public int SecondsToComplete => _timeInit + TimeTillEnd - (int)Time.time;

		public bool TimeIsOver => SecondsToComplete <= 0;

		public bool IsValid
		{
			get
			{
				if (Enabled)
				{
					return !TimeIsOver;
				}
				return false;
			}
		}

		public bool IsHiddenFromBannerList
		{
			get
			{
				if (Payload != null)
				{
					return Payload.HiddenFromList;
				}
				return false;
			}
		}

		public void Init(int imageID)
		{
			_timeInit = Mathf.RoundToInt(Time.time);
			if (Payload == null)
			{
				Payload = new OfferPayloadModel();
			}
			Payload.Init();
			if (Skin == null)
			{
				Skin = new OfferSkinModel();
			}
			Skin.Init();
		}
	}
}
