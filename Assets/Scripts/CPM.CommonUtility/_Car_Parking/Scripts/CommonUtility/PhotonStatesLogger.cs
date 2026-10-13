using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.CommonUtility
{
	public class PhotonStatesLogger : IDisposable
	{
		private IDisposable dis;

		public void Start()
		{
			dis = Observable.IntervalFrame(30).Subscribe((long _) =>
			{
				Debug.LogWarning(PhotonNetwork.NetworkClientState);
			});
		}

		public void Dispose()
		{
			dis.Dispose();
		}
	}
}
