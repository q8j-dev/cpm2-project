using System;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

public class TrafficBody : MonoBehaviour
{
	private const string NpcCarAlarm = "Play_NPC_Car_Alarm";

	private float _blinkerSpeed = 7.2f;

	private float _waitTime = 10.25f;

	public bool AlarmStarted;

	public Material[] Mat;

	private float _intensityStart = 1f;

	private float _intensityEnd = 4f;

	private MeshRenderer _renderer;

	private readonly int _intensity = Shader.PropertyToID("_Intensity");

	private IDisposable _disposablePlayNpcCarAlarm;

	private void Awake()
	{
		_renderer = GetComponent<MeshRenderer>();
		Mat = _renderer.materials;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!AlarmStarted && collision.relativeVelocity.magnitude > 3f)
		{
			_disposablePlayNpcCarAlarm?.Dispose();
			_disposablePlayNpcCarAlarm = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				PlayNpcCarAlarm();
			}).AddTo(this);
		}
	}

	private void CallAlarm(object in_cookie, AkCallbackType callType, object in_info)
	{
		if (callType == AkCallbackType.AK_EndOfEvent)
		{
			AlarmRestart();
		}
	}

	private void PlayNpcCarAlarm()
	{
		AudioWrapper.PostEvent("Play_NPC_Car_Alarm", base.gameObject, 1u, CallAlarm, base.gameObject);
		AlarmStarted = true;
	}

	private void OnDestroy()
	{
		AudioWrapper.StopAll(base.gameObject);
	}

	private void LightBlink()
	{
		if (AlarmStarted)
		{
			float value = Mathf.PingPong(Time.time * _blinkerSpeed, _intensityEnd) + _intensityStart;
			Material[] mat = Mat;
			for (int i = 0; i < mat.Length; i++)
			{
				mat[i].SetFloat(_intensity, value);
			}
		}
	}

	private void Update()
	{
		LightBlink();
	}

	public void AlarmRestart()
	{
		AlarmStarted = false;
		Material[] mat = Mat;
		for (int i = 0; i < mat.Length; i++)
		{
			mat[i].SetFloat(_intensity, _intensityStart);
		}
	}
}
