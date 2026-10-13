using System;
using UniRx;
using UnityEngine;

public class PersonSelectingControl : MonoBehaviour
{
	private IDisposable _setIdleDisposable;

	public void SetAnimClip(string animName)
	{
		GetComponent<Animator>().CrossFade(animName, 0.1f, 0, UnityEngine.Random.Range(0f, 1f));
	}

	public void SetIdle(float aftter)
	{
		_setIdleDisposable?.Dispose();
		if (UnityEngine.Random.Range(0, 2) == 1)
		{
			_setIdleDisposable = Observable.Timer(TimeSpan.FromSeconds(aftter)).Subscribe((long _) =>
			{
				setIdle1();
			}).AddTo(this);
		}
		else
		{
			_setIdleDisposable = Observable.Timer(TimeSpan.FromSeconds(aftter)).Subscribe((long _) =>
			{
				setIdle2();
			}).AddTo(this);
		}
	}

	private void setIdle1()
	{
		GetComponent<Animator>().CrossFade("idle1", 0.1f, 0, UnityEngine.Random.Range(0f, 1f));
	}

	private void setIdle2()
	{
		GetComponent<Animator>().CrossFade("idle2", 0.1f, 0, UnityEngine.Random.Range(0f, 1f));
	}

	public void SetCheer()
	{
		if (UnityEngine.Random.Range(0, 2) == 1)
		{
			SetAnimClip("cheer");
		}
		else
		{
			SetAnimClip("claphands");
		}
	}
}
