using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	public class VehicleLightsComponent : MonoBehaviour, IVehicleLightsComponent
	{
		[Tooltip("Blinking interval")]
		public float blinkTime = 0.5f;

		[Tooltip("A GameObject containing all main lights - will be active based on Manager API calls")]
		public GameObject frontLights;

		[Tooltip("A GameObject containing all reverse lights - will be active if a vehicle is reversing")]
		public GameObject reverseLights;

		[Tooltip("A GameObject containing all rear lights - will be active if main lights are active")]
		public GameObject rearLights;

		[Tooltip("A GameObject containing all brake lights - will be active when a vehicle is braking")]
		public GameObject stopLights;

		[Tooltip("A GameObject containing all blinker left lights - will be active when car turns left")]
		public GameObject blinkerLeft;

		[Tooltip("A GameObject containing all blinker right lights - will be active when car turns right")]
		public GameObject blinkerRight;

		private float _currentTime;

		private bool _updateBlinkers;

		private bool _updateLeftBlinker;

		private bool _updateRightBlinker;

		private BlinkType _currentBlinkType;

		public void Initialize()
		{
			_currentTime = 0f;
			LightsSetup();
		}

		public void DeactivateLights()
		{
			LightsSetup();
			_updateLeftBlinker = false;
			_updateRightBlinker = false;
		}

		private void LightsSetup()
		{
			if (frontLights != null)
			{
				frontLights.SetActive(value: false);
			}
			if (reverseLights != null)
			{
				reverseLights.SetActive(value: false);
			}
			if (rearLights != null)
			{
				rearLights.SetActive(value: false);
			}
			if (stopLights != null)
			{
				stopLights.SetActive(value: false);
			}
			if (blinkerLeft != null)
			{
				blinkerLeft.SetActive(value: false);
				_updateBlinkers = true;
			}
			if (blinkerRight != null)
			{
				blinkerRight.SetActive(value: false);
				_updateBlinkers = true;
			}
		}

		public void SetBrakeLights(bool active)
		{
			if ((bool)stopLights && stopLights.activeSelf != active)
			{
				stopLights.SetActive(active);
			}
		}

		public void SetMainLights(bool active)
		{
			if ((bool)frontLights)
			{
				frontLights.SetActive(active);
			}
			if ((bool)rearLights)
			{
				rearLights.SetActive(active);
			}
		}

		public void SetReverseLights(bool active)
		{
			if ((bool)reverseLights && reverseLights.activeSelf != active)
			{
				reverseLights.SetActive(active);
			}
		}

		public void SetBlinker(BlinkType blinkType)
		{
			if (_currentBlinkType == BlinkType.StartHazard && blinkType != BlinkType.StopHazard)
			{
				return;
			}
			_currentBlinkType = blinkType;
			if (!blinkerLeft || !blinkerRight)
			{
				return;
			}
			switch (blinkType)
			{
			case BlinkType.Stop:
				if (_updateLeftBlinker)
				{
					_updateLeftBlinker = false;
				}
				if (_updateRightBlinker)
				{
					_updateRightBlinker = false;
				}
				break;
			case BlinkType.Left:
				if (!_updateLeftBlinker)
				{
					_updateLeftBlinker = true;
				}
				if (_updateRightBlinker)
				{
					_updateRightBlinker = false;
				}
				break;
			case BlinkType.Right:
				if (!_updateRightBlinker)
				{
					_updateRightBlinker = true;
				}
				if (_updateLeftBlinker)
				{
					_updateLeftBlinker = false;
				}
				break;
			case BlinkType.StartHazard:
				if (!_updateRightBlinker)
				{
					_updateRightBlinker = true;
				}
				if (!_updateLeftBlinker)
				{
					_updateLeftBlinker = true;
				}
				break;
			}
		}

		public void UpdateLights(float realTimeSinceStartup)
		{
			if (!_updateBlinkers || !(realTimeSinceStartup - _currentTime > blinkTime))
			{
				return;
			}
			_currentTime = realTimeSinceStartup;
			if (!_updateLeftBlinker)
			{
				if (blinkerLeft.activeSelf)
				{
					blinkerLeft.SetActive(value: false);
				}
			}
			else
			{
				blinkerLeft.SetActive(!blinkerLeft.activeSelf);
			}
			if (!_updateRightBlinker)
			{
				if (blinkerRight.activeSelf)
				{
					blinkerRight.SetActive(value: false);
				}
			}
			else
			{
				blinkerRight.SetActive(!blinkerRight.activeSelf);
			}
		}
	}
}
