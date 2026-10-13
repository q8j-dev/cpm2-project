using System;
using UnityEngine;

namespace _Car_Parking.Scripts.GameInput
{
	[CreateAssetMenu(menuName = "CPM/Haptic Config", fileName = "HapticConfig")]
	public class HapticConfigSO : ScriptableObject
	{
		[Serializable]
		public struct Pulse
		{
			[Range(0f, 1f)]
			public float Strength;

			public float Seconds;

			[Range(0f, 1f)]
			public float HighFreqRatio;

			public float FadeSeconds;

			public float ModulationHz;
		}

		[Header("One-shot pulses")]
		public Pulse UiNavigate = new Pulse
		{
			Strength = 0.12f,
			Seconds = 0.04f,
			HighFreqRatio = 0.3f,
			FadeSeconds = 0f,
			ModulationHz = 0f
		};

		public Pulse GearChange = new Pulse
		{
			Strength = 0.15f,
			Seconds = 0.05f,
			HighFreqRatio = 0.4f,
			FadeSeconds = 0f,
			ModulationHz = 0f
		};

		public Pulse CircularHover = new Pulse
		{
			Strength = 0.1f,
			Seconds = 0.04f,
			HighFreqRatio = 0.3f,
			FadeSeconds = 0f,
			ModulationHz = 0f
		};

		[Header("Collision — one kick per impact, scaled by how hard it was")]
		public Pulse Collision = new Pulse
		{
			Strength = 0.45f,
			Seconds = 0.06f,
			HighFreqRatio = 0.5f,
			FadeSeconds = 0.08f,
			ModulationHz = 0f
		};

		public float CollisionMinImpactKmh = 8f;

		public float CollisionFullImpactKmh = 45f;

		public float CollisionCooldownSeconds = 0.15f;

		[Header("Engine start — crank chugs, then a 'caught' fire (use ModulationHz for a coarser/slower feel)")]
		public float EngineStartDelaySeconds = 0.5f;

		public float EngineCrankMaxSeconds = 5f;

		[Min(0f)]
		public int EngineCrankPulses = 5;

		[Range(0f, 1f)]
		public float EngineCrankStrength = 0.25f;

		public float EngineCrankPulseSeconds = 0.05f;

		public float EngineCrankInterval = 0.2f;

		[Range(0f, 1f)]
		public float EngineCrankHighFreqRatio = 0.15f;

		public float EngineCrankModulationHz;

		[Range(0f, 1f)]
		[Space]
		public float EngineFireStrength = 0.6f;

		public float EngineFireSeconds = 0.25f;

		[Range(0f, 1f)]
		public float EngineFireHighFreqRatio = 0.45f;

		public float EngineFireFadeSeconds = 0.15f;

		public float EngineFireModulationHz;

		[Header("Brake (continuous) — builds up while held; press force sets the ceiling")]
		public float BrakeMinSpeedKmh = 5f;

		[Range(0f, 1f)]
		public float BrakeHoldMinCeiling = 0.35f;

		public float BrakeHoldRampSeconds = 0.6f;

		public float BrakeHoldReleaseSeconds = 0.15f;

		public float BrakeModulationHz;

		[Range(0f, 1f)]
		public float BrakeLevel = 0.42f;

		[Range(0f, 1f)]
		[Header("Handbrake")]
		public float HandbrakeLevel = 0.6f;

		public float HandbrakeMinSpeedKmh = 8f;
	}
}
