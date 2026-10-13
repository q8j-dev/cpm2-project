using System;
using UniRx;
using UnityEngine;
using _Car_Parking;

public class DogMove : MonoBehaviour
{
	public Vector3 initialPoint;

	public Vector3 Target;

	public float Speed;

	public Animator animator;

	public int state;

	public bool canMove = true;

	public AudioSource barkSound;

	private bool soundPlayed;

	private IDisposable _resetSoundPlayedDisposable;

	private void Start()
	{
		animator = GetComponent<Animator>();
		Observable.Timer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
		{
			CheckDistacneForPerson();
		}).AddTo(this);
		initialPoint = base.transform.position;
	}

	private void Update()
	{
		if (!(Target != Vector3.zero))
		{
			return;
		}
		if (Vector3.Distance(Target, base.transform.position) > 2f && (state == 2 || state == 1) && canMove && FlagsStorage.OutOfCar)
		{
			float maxDistanceDelta = Speed * Time.deltaTime;
			base.transform.position = Vector3.MoveTowards(base.transform.position, Target, maxDistanceDelta);
			if (Speed != 0f)
			{
				base.transform.LookAt(Target);
				Vector3 eulerAngles = base.transform.eulerAngles;
				eulerAngles.z = 0f;
				eulerAngles.x = 0f;
				base.transform.eulerAngles = eulerAngles;
			}
		}
		else if (state != 0 && state != 3 && state != 4)
		{
			switch (UnityEngine.Random.Range(0, 4))
			{
			case 1:
			{
				int stateToSet = 3;
				SetState(stateToSet, canMove2: false);
				break;
			}
			case 2:
			{
				int stateToSet = 4;
				SetState(stateToSet, canMove2: false);
				break;
			}
			default:
			{
				int stateToSet = 0;
				SetState(stateToSet, canMove2: true);
				break;
			}
			}
			Speed = 0f;
		}
	}

	private void SetState(int stateToSet, bool canMove2)
	{
		if (stateToSet != state)
		{
			state = stateToSet;
			animator.SetInteger("state", state);
			canMove = canMove2;
		}
	}

	private void SetState(int stateToSet)
	{
		if (stateToSet != state)
		{
			state = stateToSet;
			animator.SetInteger("state", state);
			PlaySound();
		}
	}

	public void CanMove()
	{
		canMove = true;
	}

	public void PissingDone()
	{
		SetState(0, canMove2: true);
	}

	private void CheckDistacneForPerson()
	{
		Target = initialPoint + V.GlobalOffset;
		if ((bool)V.DriverPerson)
		{
			Target = V.DriverPerson.transform.position;
			float num = Vector3.Distance(Target, base.transform.position);
			if (num < 20f)
			{
				Vector3 direction = Target + new Vector3(0f, 0.5f, 0f) - (base.transform.position + new Vector3(0f, 0.3f, 0f));
				if (!Physics.Raycast(base.transform.position + new Vector3(0f, 0.3f, 0f), direction, num - 1f, -1, QueryTriggerInteraction.Ignore))
				{
					Target = V.DriverPerson.transform.position;
					if (num > 6f)
					{
						SetState(2);
						Speed = 5f;
					}
					else if (num < 2f)
					{
						if (Speed != 0f)
						{
							SetState(0);
						}
						Speed = 0f;
					}
					else
					{
						SetState(1);
						Speed = 0.5f;
					}
				}
				else
				{
					SetState(0);
					Speed = 0f;
				}
			}
			else if (num > 100f)
			{
				base.transform.position = initialPoint + V.GlobalOffset;
				SetState(3, canMove2: false);
			}
		}
		if (Speed > 0f && Physics.Raycast(base.transform.position + new Vector3(0f, 0.3f, 0f), -base.transform.up, out var hitInfo))
		{
			base.transform.position = new Vector3(base.transform.position.x, hitInfo.point.y, base.transform.position.z);
		}
	}

	private void PlaySound()
	{
		if (!soundPlayed)
		{
			if (!barkSound.isPlaying)
			{
				barkSound.Play();
			}
			soundPlayed = true;
			_resetSoundPlayedDisposable = Observable.Timer(TimeSpan.FromSeconds(UnityEngine.Random.Range(10, 20))).Subscribe((long _) =>
			{
				ResetSoundPlayed();
			}).AddTo(this);
			barkSound.pitch = (float)UnityEngine.Random.Range(5, 15) / 10f;
		}
	}

	private void ResetSoundPlayed()
	{
		soundPlayed = false;
	}
}
