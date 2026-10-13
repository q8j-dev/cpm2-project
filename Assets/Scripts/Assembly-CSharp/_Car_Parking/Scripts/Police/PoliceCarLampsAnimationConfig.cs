using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Police
{
	[CreateAssetMenu(menuName = "Configs/Police/LampsAnimationConfig")]
	public class PoliceCarLampsAnimationConfig : ScriptableObject
	{
		[Serializable]
		public class AnimationOneLightData
		{
			public int Index;

			public float DelayToEnable;

			public float TimeEnable = 0.1f;

			public float Time;

			public float TimeDisable = 0.1f;
		}

		public float DelayToNextCycle;

		public AnimationOneLightData[] AnimationOneLightDatas;
	}
}
