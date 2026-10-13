using System;
using CodeStage.AntiCheat.Storage;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

public class HornItem : MonoBehaviour
{
	private static readonly uint PlayHornsSwitch = AudioWrapper.GetId("Play_Horns_Switch");

	private static readonly uint StopHornsSwitch = AudioWrapper.GetId("Stop_Horns_Switch");

	public GameObject buyButton;

	public GameObject upButton;

	public GameObject downButton;

	public int currentHornID;

	public int allHornCount = 6;

	public int[] AllHorns;

	public GameObject hornSelectedMark;

	public GameObject setThisHornButton;

	private bool _isPlaying;

	private IDisposable _disposableStopHorn;

	private void Start()
	{
		buyButton.SetActive(value: false);
		AllHorns = new int[allHornCount];
		for (int i = 0; i < AllHorns.Length; i++)
		{
			AllHorns[i] = i;
		}
		UpdateCurrentVehicleHornSignal();
	}

	public void UpdateCurrentVehicleHornSignal()
	{
		int value = ObscuredPrefs.GetInt($"hornID{V.CarId}");
		value = Mathf.Clamp(value, 0, AllHorns.Length);
		SetVehicleHornSignal(value);
	}

	public void SetCurrentVehicleHornSignal()
	{
		ObscuredPrefs.SetInt($"hornID{V.CarId}", currentHornID);
		SetVehicleHornSignal(currentHornID);
	}

	private void SetVehicleHornSignal(int id)
	{
		currentHornID = id;
		upButton.SetActive(value: true);
		downButton.SetActive(value: true);
		if (id == 0)
		{
			upButton.SetActive(value: false);
		}
		if (id + 1 == AllHorns.Length)
		{
			downButton.SetActive(value: false);
		}
		if (ObscuredPrefs.GetInt($"horn{id}") == 1 || id == 0)
		{
			buyButton.SetActive(value: false);
			setThisHornButton.SetActive(value: true);
		}
		else
		{
			buyButton.SetActive(value: true);
			setThisHornButton.SetActive(value: false);
		}
		bool flag = ObscuredPrefs.GetInt($"hornID{V.CarId}") == currentHornID;
		hornSelectedMark.SetActive(flag);
		if (flag)
		{
			setThisHornButton.SetActive(value: false);
		}
	}

	public void onUpPressed()
	{
		if (_isPlaying)
		{
			StopHorn();
		}
		currentHornID++;
		if (AllHorns.Length <= currentHornID)
		{
			currentHornID = 0;
		}
		SetVehicleHornSignal(currentHornID);
		PlayThisHorn();
	}

	public void onDownPressed()
	{
		if (_isPlaying)
		{
			StopHorn();
		}
		currentHornID--;
		if (currentHornID < 0)
		{
			currentHornID = AllHorns.Length;
		}
		SetVehicleHornSignal(currentHornID);
		PlayThisHorn();
	}

	public void PlayThisHorn()
	{
		_disposableStopHorn?.Dispose();
		AudioWrapper.SetSwitch("Horns_Switch", $"horn_{currentHornID}", V.Car);
		AudioWrapper.PostEvent(PlayHornsSwitch, V.Car);
		_disposableStopHorn = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
		{
			StopHorn();
		}).AddTo(this);
		_isPlaying = true;
	}

	private void StopHorn()
	{
		_disposableStopHorn?.Dispose();
		AudioWrapper.PostEvent(StopHornsSwitch, V.Car);
		_isPlaying = false;
	}

	private void OnDisable()
	{
		StopHorn();
	}
}
