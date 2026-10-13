using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

public class TrafficLightController : MonoBehaviourPunCallbacks
{
	public static TrafficLightController _object;

	public Material _red;

	public Material _yellow;

	public Material _green;

	public float timer;

	public bool _redBool;

	public bool _yellowBool;

	public bool _greenBool;

	public int _greenCounter;

	public PhotonView _photonView;

	private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

	private IDisposable _trafficLightTick;

	private readonly MaterialAssetStateGuard _materialGuard = new MaterialAssetStateGuard();

	private void Start()
	{
		_object = this;
		_materialGuard.Capture(_red);
		_materialGuard.Capture(_yellow);
		_materialGuard.Capture(_green);
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
			if (_green.color != Color.green)
			{
				ResetColors();
				Color green = Color.green;
				_green.color = green;
				_green.SetColor(EmissionColor, green);
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
				Color yellow = Color.yellow;
				_yellow.color = yellow;
				_yellow.SetColor(EmissionColor, yellow);
			}
		}
		else if (timer > 12f && timer < 22f)
		{
			if (_red.color != Color.red)
			{
				ResetColors();
				Color red = Color.red;
				_red.color = red;
				_red.SetColor(EmissionColor, red);
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
		_red.SetColor(EmissionColor, Color.black);
		_yellow.SetColor(EmissionColor, Color.black);
		_green.SetColor(EmissionColor, Color.black);
	}

	private void callRPC()
	{
		_ = PhotonNetwork.IsMasterClient;
	}

	private new void OnDisable()
	{
		_trafficLightTick?.Dispose();
		_trafficLightTick = null;
		ResetColors();
	}
}
