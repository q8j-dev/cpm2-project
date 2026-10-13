using UnityEngine;

namespace _Car_Parking.Scripts
{
	[CreateAssetMenu(menuName = "CPM/Steam Speed Camera Config", fileName = "SteamSpeedCameraConfig")]
	public class SteamSpeedCameraConfig : ScriptableObject
	{
		[Header("FOV by speed (added on top of manual zoom)")]
		public bool EnableFov = true;

		[Tooltip("Degrees added to the current zoom FOV at max speed.")]
		public float MaxFovAdd = 25f;

		[Tooltip("Speed (km/h) below which there is NO effect — the car must accelerate past this first.")]
		public float StartSpeedKmh = 60f;

		[Tooltip("Speed (km/h) at which the full add is reached. Bigger = ramps slower / needs more speed.")]
		public float SpeedAtMaxKmh = 200f;

		public float FovLerpSpeed = 3f;

		[Header("Distance pullback by speed")]
		public bool EnablePullback = true;

		[Tooltip("Extra chase distance added at max speed (chase cameras only).")]
		public float MaxPullback = 1.5f;

		public float PullbackLerpSpeed = 3f;
	}
}
