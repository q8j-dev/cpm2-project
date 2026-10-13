using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class VehicleLightsComponentV2 : MonoBehaviour, IVehicleLightsComponent
	{
		[Tooltip("Blinking interval")]
		public float blinkTime = 0.5f;

		[Tooltip("A GameObject containing all main lights - will be active based on Manager API calls")]
		public GameObject[] frontLights;

		[Tooltip("A GameObject containing all reverse lights - will be active if a vehicle is reversing")]
		public GameObject[] reverseLights;

		[Tooltip("A GameObject containing all rear lights - will be active if main lights are active")]
		public GameObject[] rearLights;

		[Tooltip("A GameObject containing all brake lights - will be active when a vehicle is braking")]
		public GameObject[] stopLights;

		[Tooltip("A GameObject containing all blinker left lights - will be active when car turns left")]
		public GameObject[] blinkerLeft;

		[Tooltip("A GameObject containing all blinker right lights - will be active when car turns right")]
		public GameObject[] blinkerRight;

		private float currentTime;

		private bool updateLights;

		private bool leftBlink;

		private bool rightBlink;

		private BlinkType _currentBlinkType;

		public void Initialize()
		{
			currentTime = 0f;
			LightsSetup();
		}

		public void DeactivateLights()
		{
			LightsSetup();
			leftBlink = false;
			rightBlink = false;
		}

		private void LightsSetup()
		{
			if (frontLights.Length != 0)
			{
				GameObject[] array = frontLights;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
			}
			if (reverseLights.Length != 0)
			{
				GameObject[] array = reverseLights;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
			}
			if (rearLights.Length != 0)
			{
				GameObject[] array = rearLights;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
			}
			if (stopLights.Length != 0)
			{
				GameObject[] array = stopLights;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
			}
			if (blinkerLeft.Length != 0)
			{
				GameObject[] array = blinkerLeft;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
				updateLights = true;
			}
			if (blinkerRight.Length != 0)
			{
				GameObject[] array = blinkerRight;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
				updateLights = true;
			}
		}

		public void SetBrakeLights(bool active)
		{
			if (stopLights.Length == 0)
			{
				return;
			}
			GameObject[] array = stopLights;
			foreach (GameObject gameObject in array)
			{
				if (gameObject.activeSelf != active)
				{
					gameObject.SetActive(active);
				}
			}
		}

		public void SetMainLights(bool active)
		{
			if (frontLights.Length != 0)
			{
				GameObject[] array = frontLights;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(active);
				}
			}
			if (rearLights.Length != 0)
			{
				GameObject[] array = rearLights;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(active);
				}
			}
		}

		public void SetReverseLights(bool active)
		{
			if (reverseLights.Length == 0)
			{
				return;
			}
			GameObject[] array = reverseLights;
			foreach (GameObject gameObject in array)
			{
				if (gameObject.activeSelf != active)
				{
					gameObject.SetActive(active);
				}
			}
		}

		public void SetBlinker(BlinkType blinkType)
		{
			if (_currentBlinkType == BlinkType.StartHazard && blinkType != BlinkType.StopHazard)
			{
				return;
			}
			_currentBlinkType = blinkType;
			if (blinkerLeft.Length == 0 || blinkerRight.Length == 0)
			{
				return;
			}
			switch (blinkType)
			{
			case BlinkType.Stop:
				if (leftBlink)
				{
					leftBlink = false;
				}
				if (rightBlink)
				{
					rightBlink = false;
				}
				break;
			case BlinkType.Left:
				if (!leftBlink)
				{
					leftBlink = true;
				}
				if (rightBlink)
				{
					rightBlink = false;
				}
				break;
			case BlinkType.Right:
				if (!rightBlink)
				{
					rightBlink = true;
				}
				if (leftBlink)
				{
					leftBlink = false;
				}
				break;
			case BlinkType.StartHazard:
				if (!rightBlink)
				{
					rightBlink = true;
				}
				if (!leftBlink)
				{
					leftBlink = true;
				}
				break;
			}
		}

		public void UpdateLights(float realTimeSinceStartup)
		{
			if (!updateLights || !(realTimeSinceStartup - currentTime > blinkTime))
			{
				return;
			}
			currentTime = realTimeSinceStartup;
			if (!leftBlink)
			{
				GameObject[] array = blinkerLeft;
				foreach (GameObject gameObject in array)
				{
					if (gameObject.activeSelf != leftBlink)
					{
						gameObject.SetActive(leftBlink);
					}
				}
			}
			else
			{
				GameObject[] array = blinkerLeft;
				foreach (GameObject obj in array)
				{
					obj.SetActive(!obj.activeSelf);
				}
			}
			if (!rightBlink)
			{
				GameObject[] array = blinkerRight;
				foreach (GameObject gameObject2 in array)
				{
					if (gameObject2.activeSelf != rightBlink)
					{
						gameObject2.SetActive(rightBlink);
					}
				}
			}
			else
			{
				GameObject[] array = blinkerRight;
				foreach (GameObject obj2 in array)
				{
					obj2.SetActive(!obj2.activeSelf);
				}
			}
		}
	}
}
