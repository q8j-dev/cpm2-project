using CPMEngine;
using UnityEngine;

namespace CarAnimationScripts
{
	public class RadiatorFanSpinner : MonoBehaviour
	{
		[SerializeField]
		private Vector3 axis = Vector3.forward;

		[SerializeField]
		private float maxSpeedDegPerSec = 720f;

		[SerializeField]
		private float spinUpTime = 0.5f;

		[SerializeField]
		private float spinDownTime = 1.5f;

		private Powertrain _powertrain;

		private float _speed;

		private void Update()
		{
			if (!_powertrain)
			{
				_powertrain = GetComponentInParent<Powertrain>();
				if (!_powertrain)
				{
					return;
				}
			}
			bool flag;
			try
			{
				flag = _powertrain.IsEngineWorks && (!PhysRuntimeFlags.EngineThermalEnabled || _powertrain.EngineThermal.FanOn);
			}
			catch
			{
				flag = false;
			}
			float target = (flag ? maxSpeedDegPerSec : 0f);
			float num = (flag ? spinUpTime : spinDownTime);
			float num2 = ((num > 0.0001f) ? (maxSpeedDegPerSec / num) : float.PositiveInfinity);
			_speed = Mathf.MoveTowards(_speed, target, num2 * Time.deltaTime);
			if (_speed > 0.001f)
			{
				base.transform.Rotate(axis, _speed * Time.deltaTime, Space.Self);
			}
		}
	}
}
