using System;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.SkidSmokeParticle
{
	public class SkidSmokeCounter : MonoBehaviour
	{
		public static int SkidSmokeParticleCount;

		private void Start()
		{
			Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				CalculateSkidSmokeCount();
			}).AddTo(this);
		}

		private void CalculateSkidSmokeCount()
		{
			ParticleSystem sharedSkidSmoke = PlayerDatas.SharedSkidSmoke;
			SkidSmokeParticleCount = (sharedSkidSmoke ? sharedSkidSmoke.particleCount : 0);
		}
	}
}
