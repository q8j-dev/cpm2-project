using DG.Tweening;
using UnityEngine;

namespace CPM.Scripts.Garage.Elevator
{
	public class ElevatorDoor : MonoBehaviour
	{
		public Transform PodiumTargetPos;

		public int DoorId;

		[SerializeField]
		private Elevator Elevator = default;

		[SerializeField]
		[Space]
		private float DoorOpenTime = 1f;

		[SerializeField]
		private bool UsePosOnOpen = default;

		[SerializeField]
		private bool UseRotOnOpen = default;

		[Space]
		[SerializeField]
		private Transform LeftDoor = default;

		[SerializeField]
		private Vector3 LeftDoorOpenPos = default;

		[SerializeField]
		private Vector3 LeftDoorOpenRot = default;

		[SerializeField]
		[Space]
		private Transform RightDoor = default;

		[SerializeField]
		private Vector3 RightDoorOpenPos = default;

		[SerializeField]
		private Vector3 RightDoorOpenRot = default;

		private Vector3 _leftDoorClosedPos;

		private Vector3 _leftDoorClosedRot;

		private Vector3 _rightDoorClosedPos;

		private Vector3 _rightDoorClosedRot;

		private Sequence _sequence;

		public bool IsOpen { get; set; }

		private void Awake()
		{
			_leftDoorClosedPos = LeftDoor.localPosition;
			_leftDoorClosedRot = LeftDoor.eulerAngles;
			_rightDoorClosedPos = RightDoor.localPosition;
			_rightDoorClosedRot = RightDoor.eulerAngles;
		}

		public void Open()
		{
			if (IsOpen)
			{
				return;
			}
			Elevator.MovePodiumToTarget(PodiumTargetPos);
			_sequence?.Kill();
			_sequence = DOTween.Sequence();
			if ((bool)LeftDoor)
			{
				if (UsePosOnOpen)
				{
					_sequence.Join(LeftDoor.DOLocalMove(LeftDoorOpenPos, DoorOpenTime));
				}
				if (UseRotOnOpen)
				{
					_sequence.Join(LeftDoor.DOLocalRotate(LeftDoorOpenRot, DoorOpenTime));
				}
			}
			if ((bool)RightDoor)
			{
				if (UsePosOnOpen)
				{
					_sequence.Join(RightDoor.DOLocalMove(RightDoorOpenPos, DoorOpenTime));
				}
				if (UseRotOnOpen)
				{
					_sequence.Join(RightDoor.DOLocalRotate(RightDoorOpenRot, DoorOpenTime));
				}
			}
			_sequence.Play();
			IsOpen = true;
		}

		public void Close()
		{
			if (!IsOpen)
			{
				return;
			}
			_sequence?.Kill();
			_sequence = DOTween.Sequence();
			if ((bool)LeftDoor)
			{
				if (UsePosOnOpen)
				{
					_sequence.Join(LeftDoor.DOLocalMove(_leftDoorClosedPos, DoorOpenTime));
				}
				if (UseRotOnOpen)
				{
					_sequence.Join(LeftDoor.DOLocalRotate(_leftDoorClosedRot, DoorOpenTime));
				}
			}
			if ((bool)RightDoor)
			{
				if (UsePosOnOpen)
				{
					_sequence.Join(RightDoor.DOLocalMove(_rightDoorClosedPos, DoorOpenTime));
				}
				if (UseRotOnOpen)
				{
					_sequence.Join(RightDoor.DOLocalRotate(_rightDoorClosedRot, DoorOpenTime));
				}
			}
			_sequence.Play();
			IsOpen = false;
		}

		private void OnDestroy()
		{
			_sequence?.Kill();
		}
	}
}
