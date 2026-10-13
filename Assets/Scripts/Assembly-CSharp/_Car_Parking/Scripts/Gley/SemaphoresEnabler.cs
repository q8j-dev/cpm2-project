using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Gley
{
	public class SemaphoresEnabler : MonoBehaviour
	{
		[Serializable]
		public class SemaphoreGroup
		{
			public int SceneIndex;

			public GameObject SemaphoreParent;
		}

		private static event Func<SemaphoresEnabler> OnGetSemEnabler;

		public static SemaphoresEnabler GetSemaphoresEnabler()
		{
			return OnGetSemEnabler?.Invoke();
		}

		public SemaphoresEnabler GetSemaphoresEnablerInternal()
		{
			return this;
		}

		private void Awake()
		{
			OnGetSemEnabler += GetSemaphoresEnablerInternal;
		}

		private void OnDestroy()
		{
			OnGetSemEnabler -= GetSemaphoresEnablerInternal;
		}
	}
}
