using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Other.Helpers;

public class ParkTronicTrigger : MonoBehaviour
{
	private List<Transform> _signs = new List<Transform>();

	private int _i1 = -1;

	private int _i2 = -1;

	public int index;

	public Color indexColor;

	public bool EnableAfter;

	private CarInputManager _carInputManager;

	private IDisposable _disposableEnableThis;

	private IDisposable _disposableCheckNeighbours;

	[Inject]
	public void Construct(CarInputManager carInputManager)
	{
		_carInputManager = carInputManager;
	}

	private void Start()
	{
		if (ScenesHelper.IsParkingLevelActive() || ScenesHelper.IsFreeDriveSceneActive())
		{
			if ((bool)_carInputManager.ParkTronicControl)
			{
				Init();
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		_disposableEnableThis?.Dispose();
		_disposableEnableThis = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			EnableThis();
		}).AddTo(this);
	}

	private void OnDestroy()
	{
		_disposableCheckNeighbours?.Dispose();
		_disposableEnableThis?.Dispose();
	}

	private void Init()
	{
		indexColor = Color.yellow;
		if (_carInputManager.ParkTronicControl != null && _carInputManager.ParkTronicControl.ParkTronicItems2 != null && (bool)_carInputManager.ParkTronicControl.ParkTronicItems2[index])
		{
			indexColor = _carInputManager.ParkTronicControl.ParkTronicItems2[index].color;
			_carInputManager.ParkTronicControl.ParkTronicItems2[index].color = Color.white;
			_disposableCheckNeighbours?.Dispose();
			_disposableCheckNeighbours = Observable.Timer(TimeSpan.FromSeconds(1.0)).Concat(Observable.Interval(TimeSpan.FromSeconds(0.9f + (float)index / 10f))).Subscribe((long _) =>
			{
				CheckNeighbours();
			})
				.AddTo(this);
		}
		indexColor = GetColorPark(index);
		switch (index)
		{
		case 2:
			_i1 = 0;
			_i2 = 1;
			break;
		case 1:
			_i1 = 0;
			break;
		case 5:
			_i1 = 4;
			_i2 = 3;
			break;
		case 4:
			_i1 = 3;
			break;
		case 8:
			_i1 = 6;
			_i2 = 7;
			break;
		case 7:
			_i1 = 6;
			break;
		case 11:
			_i1 = 10;
			_i2 = 9;
			break;
		case 10:
			_i1 = 9;
			break;
		case 14:
			_i1 = 12;
			_i2 = 13;
			break;
		case 13:
			_i1 = 12;
			break;
		case 17:
			_i1 = 15;
			_i2 = 16;
			break;
		case 16:
			_i1 = 15;
			break;
		case 3:
		case 6:
		case 9:
		case 12:
		case 15:
			break;
		}
	}

	private void EnableThis()
	{
		EnableAfter = true;
	}

	private Color GetColorPark(int i)
	{
		return (i % 3) switch
		{
			0 => Color.red, 
			1 => Color.yellow, 
			2 => Color.green, 
			_ => Color.white, 
		};
	}

	private void CheckNeighbours()
	{
		if (!_carInputManager.ParkTronicControl)
		{
			return;
		}
		if (_signs.Count <= 0)
		{
			TurnOffOwn();
		}
		if (_i1 >= 0 && _i1 < _carInputManager.ParkTronicControl.partTronicValues.Length && _i2 >= 0 && _i2 < _carInputManager.ParkTronicControl.partTronicValues.Length)
		{
			bool num = _carInputManager.ParkTronicControl.partTronicValues[_i1];
			bool flag = _carInputManager.ParkTronicControl.partTronicValues[_i2];
			if ((num | flag) && _signs.Count > 0)
			{
				TurnOnOwn();
			}
			else if (_signs.Count <= 0)
			{
				TurnOffOwn();
			}
		}
		else if (_i2 >= 0 && _i2 < _carInputManager.ParkTronicControl.partTronicValues.Length)
		{
			if (_carInputManager.ParkTronicControl.partTronicValues[_i1] && _signs.Count > 0)
			{
				TurnOnOwn();
			}
			else if (_signs.Count <= 0)
			{
				TurnOffOwn();
			}
		}
	}

	private void TurnOnOwn()
	{
		try
		{
			if ((bool)_carInputManager.ParkTronicControl)
			{
				if (_carInputManager.ParkTronicControl.ParkTronicItems2 != null && _carInputManager.ParkTronicControl.ParkTronicItems2.Length > index && (bool)_carInputManager.ParkTronicControl.ParkTronicItems2[index])
				{
					_carInputManager.ParkTronicControl.ParkTronicItems2[index].color = indexColor;
				}
				if (_carInputManager.ParkTronicControl.partTronicValues != null && _carInputManager.ParkTronicControl.partTronicValues.Length > index)
				{
					_carInputManager.ParkTronicControl.partTronicValues[index] = true;
				}
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	private void TurnOffOwn()
	{
		try
		{
			if ((bool)_carInputManager.ParkTronicControl && _carInputManager.ParkTronicControl.ParkTronicItems2 != null && _carInputManager.ParkTronicControl.ParkTronicItems2.Length > index && (bool)_carInputManager.ParkTronicControl.ParkTronicItems2[index])
			{
				_carInputManager.ParkTronicControl.ParkTronicItems2[index].color = Color.white;
			}
			if ((bool)_carInputManager.ParkTronicControl && _carInputManager.ParkTronicControl.partTronicValues != null && _carInputManager.ParkTronicControl.partTronicValues.Length > index)
			{
				_carInputManager.ParkTronicControl.partTronicValues[index] = false;
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!ScenesHelper.IsMenuSceneActive() && !FlagsStorage.SingeDragRacingMode && EnableAfter && !other.isTrigger && !other.CompareTag("CarSellBlockCollider") && (!_carInputManager.ParkTronicControl || _carInputManager.ParkTronicControl.ParkTronicItems2 != null))
		{
			if (other.TryGetComponent<SignHandler>(out var component))
			{
				SignHandler signHandler = component;
				signHandler.OnSignBroke = (Action<Transform>)Delegate.Combine(signHandler.OnSignBroke, new Action<Transform>(RemoveSign));
			}
			(from _ in other.OnDisableAsObservable()
				where other
				select _).Subscribe((Unit _) =>
			{
				RemoveSign(other.transform);
			}).AddTo(this);
			_signs.Add(other.transform);
			TurnOnOwn();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!ScenesHelper.IsMenuSceneActive() && !FlagsStorage.SingeDragRacingMode && EnableAfter && !other.isTrigger && !other.CompareTag("CarSellBlockCollider") && (!_carInputManager.ParkTronicControl || _carInputManager.ParkTronicControl.ParkTronicItems2 != null))
		{
			_signs.Remove(other.transform);
			if (_signs.Count <= 0)
			{
				TurnOffOwn();
			}
		}
	}

	private void RemoveSign(Transform signTransform)
	{
		if (signTransform.TryGetComponent<SignHandler>(out var component))
		{
			SignHandler signHandler = component;
			signHandler.OnSignBroke = (Action<Transform>)Delegate.Remove(signHandler.OnSignBroke, new Action<Transform>(RemoveSign));
		}
		_signs.Remove(signTransform);
	}

	public void RemoveAllSigns()
	{
		_signs.Clear();
	}
}
