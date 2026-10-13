using AK.Wwise;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	[RequireComponent(typeof(SphereCollider))]
	public class LighthouseFoghornController : MonoBehaviour
	{
		private const string MainCameraTag = "MainCamera";

		public AK.Wwise.Event PlayEvent = new AK.Wwise.Event();

		public AK.Wwise.Event StopEvent = new AK.Wwise.Event();

		[Tooltip("Минимальная облачность (0..10) для запуска гудка. По умолчанию 4.")]
		[SerializeField]
		private float _cloudinessThreshold = 4f;

		private const float CheckInterval = 0.5f;

		private SunAndSkyController _sun;

		private bool _sunLookupDone;

		private bool _isPlayerInside;

		private bool _isPlaying;

		private float _nextCheckTime;

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("MainCamera"))
			{
				_isPlayerInside = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("MainCamera"))
			{
				_isPlayerInside = false;
				StopIfPlaying();
			}
		}

		private void Update()
		{
			if (!_isPlayerInside || Time.time < _nextCheckTime)
			{
				return;
			}
			_nextCheckTime = Time.time + 0.5f;
			if (!_sunLookupDone)
			{
				_sun = Object.FindObjectOfType<SunAndSkyController>();
				_sunLookupDone = true;
			}
			if (!(_sun == null))
			{
				bool flag = CPMGraphicsSettingsManager.Instance != null && CPMGraphicsSettingsManager.Instance.FogQuality != CPMGraphicsSettings.FogQuality.Off && _sun.CurrentCloudiness >= _cloudinessThreshold;
				if (flag && !_isPlaying)
				{
					PlayEvent.Post(base.gameObject);
					_isPlaying = true;
				}
				else if (!flag && _isPlaying)
				{
					StopEvent.Post(base.gameObject);
					_isPlaying = false;
				}
			}
		}

		private void OnDisable()
		{
			StopIfPlaying();
		}

		private void OnDestroy()
		{
			StopIfPlaying();
		}

		private void StopIfPlaying()
		{
			if (_isPlaying)
			{
				StopEvent.Post(base.gameObject);
				_isPlaying = false;
			}
		}
	}
}
