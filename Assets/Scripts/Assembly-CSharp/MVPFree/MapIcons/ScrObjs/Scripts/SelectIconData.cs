using System;
using AK.Wwise;
using UnityEngine;

namespace MVPFree.MapIcons.ScrObjs.Scripts
{
	[Serializable]
	public class SelectIconData
	{
		public float clickDuration;

		public float clickDistance;

		public float selectDistance;

		[Header("move cross")]
		public float moveVelocity;

		public float maxMoveDuration;

		[Header("haptics")]
		public bool hapticsEnabled;

		public float hapticsDuration;

		public float hapticsIntensity;

		[Header("Sound")]
		public AK.Wwise.Event uiEvent;

		public Switch wwiseSound;
	}
}
