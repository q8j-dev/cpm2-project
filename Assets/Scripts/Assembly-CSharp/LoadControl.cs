using System;
using CPMEngine;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Scopes;

public class LoadControl : MonoBehaviour
{
	private Vector3[][] _loadsInitPositions;

	private Quaternion[][] _loadsEulerAngles;

	public GameObject[] parentsOfLoads;

	private int _usingIndex;

	private IDisposable _timer;

	private LevelScript _levelScript;

	private OptionalDependency<LevelScript> _levelScriptDep;

	[Inject]
	public void Construct(OptionalDependency<LevelScript> levelScript)
	{
		_levelScriptDep = levelScript;
	}

	private void Awake()
	{
		_levelScript = _levelScriptDep.Value;
		if (_levelScript != null)
		{
			_levelScript.LoadControl = this;
		}
	}

	private void Start()
	{
		_loadsInitPositions = new Vector3[3][];
		_loadsEulerAngles = new Quaternion[3][];
		for (int i = 0; i < parentsOfLoads.Length; i++)
		{
			LoadTriger[] componentsInChildren = parentsOfLoads[i].GetComponentsInChildren<LoadTriger>();
			_loadsInitPositions[i] = new Vector3[componentsInChildren.Length];
			_loadsEulerAngles[i] = new Quaternion[componentsInChildren.Length];
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				_loadsInitPositions[i][j] = componentsInChildren[j].transform.localPosition;
				_loadsEulerAngles[i][j] = componentsInChildren[j].transform.localRotation;
				componentsInChildren[j].LevelScript = _levelScript;
			}
		}
		DisableLoad();
	}

	public void ResetPosition(int indexLoad, GameObject car, bool makeKinematic)
	{
		_usingIndex = indexLoad;
		parentsOfLoads[indexLoad].SetActive(value: true);
		parentsOfLoads[indexLoad].transform.parent = car.transform;
		if (car.TryGetComponent<Powertrain>(out var component))
		{
			parentsOfLoads[indexLoad].transform.SetLocalPositionAndRotation(component.Loads.localPosition, component.Loads.localRotation);
		}
		if (!makeKinematic)
		{
			_timer?.Dispose();
			_timer = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				ResetKinematic();
			}).AddTo(this);
		}
		LoadTriger[] componentsInChildren = parentsOfLoads[indexLoad].GetComponentsInChildren<LoadTriger>();
		for (int num = 0; num < componentsInChildren.Length; num++)
		{
			componentsInChildren[num].transform.SetLocalPositionAndRotation(_loadsInitPositions[indexLoad][num], _loadsEulerAngles[indexLoad][num]);
			if (componentsInChildren[num].TryGetComponent<Rigidbody>(out var component2))
			{
				component2.velocity = Vector3.zero;
				component2.angularVelocity = Vector3.zero;
				component2.isKinematic = true;
			}
			if (componentsInChildren[num].TryGetComponent<Collider>(out var component3))
			{
				component3.enabled = false;
			}
		}
	}

	private void ResetKinematic()
	{
		LoadTriger[] componentsInChildren = parentsOfLoads[_usingIndex].GetComponentsInChildren<LoadTriger>();
		_timer?.Dispose();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].transform.SetLocalPositionAndRotation(_loadsInitPositions[_usingIndex][i], _loadsEulerAngles[_usingIndex][i]);
			if (componentsInChildren[i].TryGetComponent<Rigidbody>(out var component))
			{
				component.velocity = Vector3.zero;
				component.angularVelocity = Vector3.zero;
				component.isKinematic = false;
			}
			if (componentsInChildren[i].TryGetComponent<Collider>(out var component2))
			{
				component2.enabled = true;
			}
		}
	}

	public void DisableLoad()
	{
		GameObject[] array = parentsOfLoads;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
	}

	public void ResetParentBack()
	{
		GameObject[] array = parentsOfLoads;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].transform.parent = base.transform;
		}
	}
}
