using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag
{
	public class DynamicBrakeDisk : MonoBehaviour
	{
		private const float RimRadiusToMaxDiameter = 1.9f;

		private const float OrigWidth = 0.014f;

		private const float OrigDiam = 0.137f;

		public MeshRenderer DiskModel;

		public GameObject CaliperModel;

		private WheelProvider _wheel;

		private float _currentDiameter;

		private bool _isInited;

		private float MaxDiameter
		{
			get
			{
				if (!_wheel)
				{
					return 9999f;
				}
				return _wheel.GetRimSize * 1.9f;
			}
		}

		public void Init(WheelProvider wheel)
		{
			_wheel = wheel;
			base.transform.localScale = Vector3.one;
			Resize(0.24f);
			_isInited = true;
		}

		public void Init(SellCarWheel wheel)
		{
			base.transform.localScale = Vector3.one;
			Resize(0.24f);
		}

		public void Resize(float diameter)
		{
			_currentDiameter = Mathf.Min(MaxDiameter, diameter);
			float num = _currentDiameter / 0.137f;
			base.transform.localScale = new Vector3(2f, num, num);
		}

		private void Start()
		{
			if ((bool)CaliperModel)
			{
				Transform[] componentsInChildren = CaliperModel.GetComponentsInChildren<Transform>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].gameObject.layer = LayerViaName.WheelLayer;
				}
			}
		}

		private void LateUpdate()
		{
			if ((bool)CaliperModel && (bool)_wheel && _wheel.Wheel != null && _wheel.Wheel.Enabled)
			{
				CaliperModel.transform.localRotation = Quaternion.AngleAxis((0f - _wheel.Wheel.RollAngle) * 57.29578f, Vector3.right);
			}
		}
	}
}
