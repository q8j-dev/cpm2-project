using System;
using DG.Tweening;
using UnityEngine;

namespace CPM.Scripts.Garage.Elevator
{
	public class Elevator : MonoBehaviour
	{
		[SerializeField]
		private ElevatorDoor[] Doors;

		[SerializeField]
		private Transform Podium;

		[SerializeField]
		private float MoveTime = 3f;

		private Sequence _sequence;

		public void MovePodiumToTarget(Transform target)
		{
			Podium.SetPositionAndRotation(target.position, target.rotation);
		}

		public void TranslatePodiumToDoor(int doorId, VehicleData vehicleData)
		{
			ElevatorDoor door = Array.Find(Doors, (ElevatorDoor d) => d.DoorId == doorId);
			if (!(door == null))
			{
				Rigidbody carRb = vehicleData.GetPowertrain.Rigidbody;
				Transform transform = vehicleData.transform;
				Vector3 position = door.PodiumTargetPos.position;
				Vector3 eulerAngles = door.PodiumTargetPos.eulerAngles;
				float num = transform.position.y - Podium.position.y;
				carRb.isKinematic = true;
				_sequence?.Kill();
				_sequence = DOTween.Sequence();
				_sequence.Join(transform.DOMoveY(position.y + num, MoveTime).SetEase(Ease.InOutSine));
				_sequence.Join(transform.DORotate(eulerAngles, MoveTime).SetEase(Ease.InOutSine));
				_sequence.Join(Podium.DOMoveY(position.y, MoveTime).SetEase(Ease.InOutSine).OnComplete(() =>
				{
					door.Open();
					carRb.isKinematic = false;
				}));
				_sequence.Play();
			}
		}

		private void OnDestroy()
		{
			_sequence?.Kill();
		}
	}
}
