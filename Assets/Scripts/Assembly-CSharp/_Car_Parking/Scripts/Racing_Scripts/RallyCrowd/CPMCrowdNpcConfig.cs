using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	[CreateAssetMenu(menuName = "Rally/Rally Crowd NPC Config")]
	public class CPMCrowdNpcConfig : ScriptableObject
	{
		[Serializable]
		public struct ActionClip
		{
			public string StateName;

			public float Duration;
		}

		[SerializeField]
		private Vector2 idleDelayRange = new Vector2(2f, 7f);

		[SerializeField]
		private float crossFadeDuration = 0.2f;

		[SerializeField]
		private string idleStateName = "Idle";

		[SerializeField]
		private ActionClip[] actions;

		[SerializeField]
		[Range(0f, 1f)]
		private float startAsActionChance = 0.4f;

		[SerializeField]
		private AnimatorCullingMode cullingMode = AnimatorCullingMode.CullCompletely;

		public Vector2 IdleDelayRange => idleDelayRange;

		public float CrossFadeDuration => crossFadeDuration;

		public string IdleStateName => idleStateName;

		public ActionClip[] Actions => actions;

		public float StartAsActionChance => startAsActionChance;

		public AnimatorCullingMode CullingMode => cullingMode;
	}
}
