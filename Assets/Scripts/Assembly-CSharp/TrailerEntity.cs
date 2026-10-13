using System;
using CPM.Database.Constructors;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Drag.Engine;

public class TrailerEntity : MonoBehaviour
{
	[SerializeField]
	public ChassisData ChassisInfo;

	[SerializeField]
	public RearWheelData WheelInfo;

	[SerializeField]
	protected Vector3 _anchor;

	[SerializeField]
	protected float _spring;

	[SerializeField]
	protected float _damper;

	[SerializeField]
	protected float _horAngLimit;

	[SerializeField]
	protected float _screwAngLimit;

	[SerializeField]
	protected float _hillAngLimit;

	private Powertrain _car;

	private Powertrain _trailer;

	private bool _started;

	private Axles _axles;

	public GameObject JackPoint;

	public Transform JackBall;

	private ConfigurableJoint _joint;

	public Axles GetAxles => _axles;

	public Vector3 Anchor
	{
		get
		{
			return _anchor;
		}
		set
		{
			_anchor = value;
			SetTrailer();
		}
	}

	private Powertrain Car
	{
		get
		{
			if ((bool)_car)
			{
				return _car;
			}
			Transform parent = base.transform;
			while ((bool)parent)
			{
				_car = parent.GetComponent<Powertrain>();
				if ((bool)_car)
				{
					break;
				}
				parent = parent.parent;
			}
			return _car;
		}
	}

	public float Damper
	{
		get
		{
			return _damper;
		}
		set
		{
			_damper = value;
			SetTrailer();
		}
	}

	public float HillAngLimit
	{
		get
		{
			return _hillAngLimit;
		}
		set
		{
			_hillAngLimit = value;
			SetTrailer();
		}
	}

	public float HorAngLimit
	{
		get
		{
			return _horAngLimit;
		}
		set
		{
			_horAngLimit = value;
			SetTrailer();
		}
	}

	public float ScrewAngLimit
	{
		get
		{
			return _screwAngLimit;
		}
		set
		{
			_screwAngLimit = value;
			SetTrailer();
		}
	}

	public float Spring
	{
		get
		{
			return _spring;
		}
		set
		{
			_spring = value;
			SetTrailer();
		}
	}

	private void Awake()
	{
		TryGetComponent<Axles>(out _axles);
		TryGetComponent<Powertrain>(out _trailer);
		TryGetComponent<ConfigurableJoint>(out _joint);
	}

	private void Start()
	{
		ApplySuspension();
	}

	private void SetTrailer()
	{
		if (!(_car == null))
		{
			if ((bool)_trailer)
			{
				_car.SetTrailer(_trailer, Anchor, JackBall.localPosition, _spring, _damper, _horAngLimit * ((float)Math.PI / 180f), _screwAngLimit * ((float)Math.PI / 180f), _hillAngLimit * ((float)Math.PI / 180f));
			}
			else
			{
				_car.SetTrailer(null, Vector3.zero, Vector3.zero, 0f, 0f, 0f, 0f, 0f);
			}
		}
	}

	public void Install(Powertrain body)
	{
		_car = body;
		_joint.connectedBody = body.Rigidbody;
		Anchor = _car.TrailerJoint.localPosition;
		_trailer.SimpleSimulationMode = true;
		SetTrailer();
		if (Anchor != Vector3.zero)
		{
			_joint.autoConfigureConnectedAnchor = false;
			_joint.connectedAnchor = Anchor;
		}
		if ((bool)JackBall)
		{
			_joint.anchor = JackBall.localPosition;
		}
		ApplySuspension();
		if ((bool)JackPoint)
		{
			JackPoint.transform.localPosition += Vector3.up * 0.465f;
		}
	}

	public void UnInstall()
	{
		if ((bool)JackPoint)
		{
			JackPoint.transform.localPosition -= Vector3.up * 0.465f;
		}
	}

	private void ApplySuspension()
	{
		WheelConstructor.SetTrailerWheelModel(base.gameObject, WheelInfo);
		_trailer.ApplyTrailerParams(ChassisInfo, WheelInfo);
	}
}
