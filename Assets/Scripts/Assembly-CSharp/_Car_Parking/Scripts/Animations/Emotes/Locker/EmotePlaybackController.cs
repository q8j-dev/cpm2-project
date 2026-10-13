using System;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	public class EmotePlaybackController : IDisposable
	{
		private readonly FreeDriveProvider _freeDriveProvider;

		private IDisposable _delayedReset;

		public EmotePlaybackController(FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
		}

		public bool TryPlayMove(IEmoteEntry entry, MonoBehaviour timerHost)
		{
			if (entry == null || !V.DriverPerson)
			{
				return false;
			}
			if (RagdollCPM.IsDriverRagdolled())
			{
				return false;
			}
			PersonLocomotion component = V.DriverPerson.GetComponent<PersonLocomotion>();
			NewMoveController component2 = V.DriverPerson.GetComponent<NewMoveController>();
			if ((!component2 || !component2.IsHoldingAnimation) && (bool)component && component.IsActive && !component.IsFullyStopped)
			{
				return true;
			}
			MainPerson component3 = V.DriverPerson.GetComponent<MainPerson>();
			EmoteWheelContext ctx = new EmoteWheelContext(V.DriverPerson, component3 ? ((int)component3.Gender) : (-1), () => EmotionController.oldSelected, (int value) =>
			{
				EmotionController.oldSelected = value;
			}, () =>
			{
				_delayedReset?.Dispose();
			}, (float seconds) =>
			{
				_delayedReset?.Dispose();
				_delayedReset = Observable.Timer(TimeSpan.FromSeconds(seconds)).Subscribe((long _) =>
				{
					ResetState();
				}).AddTo(timerHost);
			});
			switch (entry.ActivateFromWheel(in ctx))
			{
			case EmoteWheelTapResult.Swallowed:
				return false;
			case EmoteWheelTapResult.Activated:
				if (_freeDriveProvider != null && (bool)_freeDriveProvider.FreeDriveDB)
				{
					++_freeDriveProvider.FreeDriveDB.emotions;
				}
				break;
			}
			return true;
		}

		public bool TryPlayEmoji(int id)
		{
			if (!V.DriverPerson)
			{
				return false;
			}
			Emoji component = V.DriverPerson.GetComponent<Emoji>();
			if (!component)
			{
				return false;
			}
			component.EmojiController(id);
			if (_freeDriveProvider != null && (bool)_freeDriveProvider.FreeDriveDB)
			{
				++_freeDriveProvider.FreeDriveDB.emotions;
			}
			return true;
		}

		public void Dispose()
		{
			_delayedReset?.Dispose();
		}

		private void ResetState()
		{
			if ((bool)V.DriverPerson)
			{
				NewMoveController component = V.DriverPerson.GetComponent<NewMoveController>();
				if ((bool)component)
				{
					component.Input.Emotion = 0;
				}
				EmotionController.oldSelected = 0;
			}
		}
	}
}
