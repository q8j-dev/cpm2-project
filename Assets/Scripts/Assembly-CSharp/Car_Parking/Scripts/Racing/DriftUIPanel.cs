using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

namespace Car_Parking.Scripts.Racing
{
	public class DriftUIPanel : MonoBehaviour
	{
		private const string DegreeSymbol = "°";

		[SerializeField]
		private RectTransform CarIcon;

		[SerializeField]
		private TextMeshProUGUI DegreeText;

		private DriftController _driftController;

		private IDisposable _driftRotator;

		private Tween _tween;

		public void RegisterDriftController(DriftController driftController)
		{
			_driftController = driftController;
			base.gameObject.SetActive(value: true);
		}

		private void Start()
		{
			if ((bool)_driftController)
			{
				_driftRotator = Observable.Interval(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long l) =>
				{
					UpdateFrameTick();
				}).AddTo(this);
			}
		}

		private void UpdateFrameTick()
		{
			_tween?.Kill();
			float num = _driftController.DriftAngle * (float)_driftController.CurrentSign;
			DegreeText.text = string.Format("{0}{1}", (int)num, "°");
			num = Mathf.Clamp(num, -80f, 80f);
			Vector3 endValue = Vector3.forward * num;
			_tween = CarIcon.DORotate(endValue, 0.1f);
		}

		private void OnDestroy()
		{
			_tween?.Kill();
			_driftRotator?.Dispose();
		}
	}
}
