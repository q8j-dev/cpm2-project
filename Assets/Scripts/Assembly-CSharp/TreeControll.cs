using System;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;

public class TreeControll : MonoBehaviour
{
	public Material[] materials;

	public GameObject startLine;

	private IDisposable _disposable;

	private int _intensity = Shader.PropertyToID("_Intensity");

	private LevelsProvider _levelsProvider;

	private CarInputManager _carInputManager;

	[Inject]
	public void Construct(LevelsProvider levelsProvider, CarInputManager carInputManager)
	{
		_levelsProvider = levelsProvider;
		_carInputManager = carInputManager;
	}

	private void Start()
	{
		ResetLights();
		_levelsProvider.TreeControl = this;
	}

	private void OnDestroy()
	{
		_levelsProvider.TreeControl = null;
		_disposable?.Dispose();
	}

	private void LightOnOff(int index, int i)
	{
		Material material = materials[index];
		material.color = ((index == 7) ? Color.green : Color.yellow);
		if (i == 0)
		{
			material.SetFloat(_intensity, 0f);
			material.color = Color.gray;
		}
		else
		{
			material.SetFloat(_intensity, 3f);
		}
	}

	private void TreeControlSingle()
	{
		if ((bool)_levelsProvider.RacingCarLevel)
		{
			if (Vector3.Distance(_levelsProvider.RacingCarLevel.transform.position, startLine.transform.position) < 8f)
			{
				LightOnOff(1, 1);
			}
			if (FlagsStorage.IsReadyMultRaceCar2)
			{
				LightOnOff(3, 1);
			}
			if (_carInputManager.State == 2 && Vector3.Distance(_levelsProvider.LevelScript.FirstCarLevel.transform.position, startLine.transform.position) < 8f)
			{
				LightOnOff(0, 1);
			}
			if (_carInputManager.Ready)
			{
				LightOnOff(2, 1);
			}
		}
	}

	public void OnBothCarOnStartLine()
	{
		_levelsProvider.LevelScript.ShowMessage("TreeControll.StartonGREENLight!", isImportant: true);
		TestTreeControl();
	}

	private void TestTreeControl()
	{
		_disposable?.Dispose();
		_disposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Do((long _) =>
		{
			First();
		}).SelectMany(Observable.Timer(TimeSpan.FromSeconds(1.0)))
			.Do((long _) =>
			{
				Second();
			})
			.SelectMany(Observable.Timer(TimeSpan.FromSeconds(0.44999998807907104)))
			.Do((long _) =>
			{
				Third();
			})
			.SelectMany(Observable.Timer(TimeSpan.FromSeconds(0.44999998807907104)))
			.Do((long _) =>
			{
				Forth();
			})
			.Subscribe()
			.AddTo(this);
	}

	public void ResetLights()
	{
		Material[] array = materials;
		foreach (Material obj in array)
		{
			obj.SetFloat(_intensity, 0f);
			obj.color = Color.gray;
		}
		_disposable?.Dispose();
	}

	private void First()
	{
		LightOnOff(4, 1);
	}

	private void Second()
	{
		LightOnOff(4, 0);
		LightOnOff(5, 1);
		if ((bool)_levelsProvider.RacingCarLevel)
		{
			_levelsProvider.RacingCarLevel.GetComponent<CarAIController>().KeepHighRpm = true;
		}
	}

	private void Third()
	{
		LightOnOff(5, 0);
		LightOnOff(6, 1);
	}

	private void Forth()
	{
		LightOnOff(0, 0);
		LightOnOff(1, 0);
		LightOnOff(2, 0);
		LightOnOff(3, 0);
		LightOnOff(6, 0);
		LightOnOff(7, 1);
		_levelsProvider.LevelScript.OnRaceStarted();
	}

	private void Update()
	{
		if (FlagsStorage.IsLevelScene)
		{
			TreeControlSingle();
		}
		else
		{
			base.enabled = false;
		}
	}
}
