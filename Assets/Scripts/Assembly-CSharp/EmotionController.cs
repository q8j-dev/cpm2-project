using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Animations.Emotes;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.UIElement.AnimationUI;

public class EmotionController : MonoBehaviour
{
	[SerializeField]
	private ParkingMenuUI ParkingMenuUI;

	[SerializeField]
	private List<AnimationPresetItem> PresetItems;

	private EmoteCatalog _catalog;

	private IDisposable _delayedReset;

	private FreeDriveProvider _freeDriveProvider;

	public static int oldSelected;

	public event Action AnimationTriggered;

	[Inject]
	public void Construct(FreeDriveProvider freeDriveProvider, EmoteCatalog emoteCatalog)
	{
		_freeDriveProvider = freeDriveProvider;
		_catalog = emoteCatalog;
	}

	private void OnEnable()
	{
		InitPreset();
	}

	private void InitPreset()
	{
		MainPerson mainPerson = (V.DriverPerson ? V.DriverPerson.GetComponent<MainPerson>() : null);
		int num = 0;
		foreach (AnimationPresetItem presetItem in PresetItems)
		{
			IEmoteEntry emoteEntry = _catalog.ResolveWheelSlot(num);
			if (emoteEntry != null)
			{
				bool genderLocked = (bool)mainPerson && AnimationPresetItem.IsGenderLocked(emoteEntry, mainPerson.Gender);
				presetItem.Init(emoteEntry, this, genderLocked);
			}
			num++;
		}
	}

	public void MakeCurrentMovement(IEmoteEntry entry)
	{
		if (!V.DriverPerson)
		{
			return;
		}
		PersonLocomotion component = V.DriverPerson.GetComponent<PersonLocomotion>();
		if (!NewMoveController.IsDriverHoldingAnimation() && (bool)component && component.IsActive && !component.IsFullyStopped)
		{
			ParkingMenuUI.OpenAnimationsPresetWindow(isOn: false);
			return;
		}
		MainPerson component2 = V.DriverPerson.GetComponent<MainPerson>();
		EmoteWheelContext ctx = new EmoteWheelContext(V.DriverPerson, component2 ? ((int)component2.Gender) : (-1), () => oldSelected, (int value) =>
		{
			oldSelected = value;
		}, () =>
		{
			_delayedReset?.Dispose();
		}, (float seconds) =>
		{
			_delayedReset?.Dispose();
			_delayedReset = Observable.Timer(TimeSpan.FromSeconds(seconds)).Subscribe((long _) =>
			{
				ResetState();
			}).AddTo(this);
		});
		switch (entry.ActivateFromWheel(in ctx))
		{
		case EmoteWheelTapResult.Swallowed:
			return;
		case EmoteWheelTapResult.Activated:
			if ((bool)_freeDriveProvider.FreeDriveDB)
			{
				++_freeDriveProvider.FreeDriveDB.emotions;
			}
			break;
		}
		NotifyAnimationTriggered();
	}

	private void NotifyAnimationTriggered()
	{
		if ((bool)ParkingMenuUI)
		{
			ParkingMenuUI.OpenAnimationsPresetWindow(isOn: false);
		}
		AnimationTriggered?.Invoke();
	}

	private void ResetState()
	{
		if ((bool)V.DriverPerson)
		{
			V.DriverPerson.GetComponent<NewMoveController>().Input.Emotion = 0;
			oldSelected = 0;
		}
	}
}
