using System;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

public class DoorOpen : MonoBehaviour
{
	public const string PlayGarageDoor = "Play_Gate_Opening_Loop";

	public const string StopGarageDoor = "Stop_Gate_Opening_Loop";

	private static readonly int OpenFront = Animator.StringToHash("open");

	private static readonly int OpenRear = Animator.StringToHash("open2");

	public bool front;

	public GameObject otherSide;

	public bool _policeStatement;

	private Animator _animator;

	public bool _garageDoor;

	private IDisposable _doorCloseTimeDisposable;

	private void Start()
	{
		if ((bool)base.transform.parent.GetChild(2).GetComponent<Animator>())
		{
			_animator = base.transform.parent.GetChild(2).GetComponent<Animator>();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("DriverPerson") && !other.CompareTag("Car"))
		{
			return;
		}
		MainPerson component = other.GetComponent<MainPerson>();
		if (_policeStatement && (bool)component)
		{
			if (component.equipmentRole == Equipment.Role.Police)
			{
				_doorCloseTimeDisposable?.Dispose();
				int id = (front ? OpenFront : OpenRear);
				_animator.SetBool(id, value: true);
			}
		}
		else
		{
			int id2 = (front ? OpenFront : OpenRear);
			_animator.SetBool(id2, value: true);
			if (front)
			{
				_doorCloseTimeDisposable?.Dispose();
			}
		}
		if (_garageDoor)
		{
			PlaySound();
		}
		TurnOfOtherSide();
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("DriverPerson") && !other.CompareTag("Car"))
		{
			return;
		}
		MainPerson component = other.GetComponent<MainPerson>();
		if (_policeStatement && (bool)component)
		{
			if (component.equipmentRole == Equipment.Role.Police)
			{
				_doorCloseTimeDisposable = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
				{
					CloseTheDoor();
				}).AddTo(this);
			}
		}
		else
		{
			_doorCloseTimeDisposable?.Dispose();
			_doorCloseTimeDisposable = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
			{
				CloseTheDoor();
			}).AddTo(this);
		}
	}

	private void CloseTheDoor()
	{
		if (_garageDoor)
		{
			PlaySound();
		}
		int id = (front ? OpenFront : OpenRear);
		_animator.SetBool(id, value: false);
		TurnOnOtherSide();
	}

	private void TurnOfOtherSide()
	{
		if (!_policeStatement && (bool)otherSide)
		{
			otherSide.SetActive(value: false);
		}
	}

	private void TurnOnOtherSide()
	{
		if (!_policeStatement && (bool)otherSide)
		{
			otherSide.SetActive(value: true);
		}
	}

	private void PlaySound()
	{
		Observable.Timer(TimeSpan.FromSeconds(0.800000011920929)).Subscribe((long _) =>
		{
			AudioWrapper.PostEvent("Play_Gate_Opening_Loop", base.gameObject);
		}).AddTo(this);
		Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			AudioWrapper.PostEvent("Stop_Gate_Opening_Loop", base.gameObject);
		}).AddTo(this);
	}
}
