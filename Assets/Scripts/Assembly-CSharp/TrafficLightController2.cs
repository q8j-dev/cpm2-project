using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

public class TrafficLightController2 : MonoBehaviour
{
	private IDisposable _trafficLightTick;

	public static TrafficLightController2 _object;

	public Material _red;

	public Material _yellow;

	public Material _green;

	public float timer;

	public bool _redBool;

	public bool _yellowBool;

	public bool _greenBool;

	public int _greenCounter;

	public PhotonView _photonView;

	private void Start()
	{
		_object = this;
		ResetColors();
		if (PhotonNetwork.IsConnected && V.DriverPerson != null && (bool)V.DriverPerson.GetComponent<PhotonView>())
		{
			_photonView = V.DriverPerson.GetComponent<PhotonView>();
		}
		_trafficLightTick?.Dispose();
		_trafficLightTick = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			TrafficLight();
		}).AddTo(this);
	}

	private void TrafficLight()
	{
		if (PhotonNetwork.IsConnected && !_photonView && V.DriverPerson != null && (bool)V.DriverPerson.GetComponent<PhotonView>())
		{
			_photonView = V.DriverPerson.GetComponent<PhotonView>();
		}
		timer++;
		if (timer < 10f)
		{
			if (_red.color != Color.red)
			{
				ResetColors();
				_red.color = Color.red;
				_greenCounter++;
				if (_greenCounter == 3 && PhotonNetwork.IsConnected)
				{
					callRPC();
				}
			}
		}
		else if (timer < 12f)
		{
			if (_yellow.color != Color.yellow)
			{
				ResetColors();
				_yellow.color = Color.yellow;
			}
		}
		else if (timer > 12f && timer < 22f)
		{
			if (_green.color != Color.green)
			{
				ResetColors();
				_green.color = Color.green;
			}
		}
		else if (timer > 22f)
		{
			timer = 0f;
		}
	}

	private void ResetColors()
	{
		_red.color = Color.gray;
		_yellow.color = Color.gray;
		_green.color = Color.gray;
	}

	private void callRPC()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			_photonView.RpcToDefaultGroup("TrafficLightRPC", RpcTarget.All);
		}
	}

	private void OnDisable()
	{
		_trafficLightTick?.Dispose();
		_trafficLightTick = null;
		ResetColors();
	}
}
