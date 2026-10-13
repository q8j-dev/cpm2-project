using AK.Wwise;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;

namespace _Car_Parking.WwisePrefabs.WiseSoundScripts
{
	public class AkAmbientCustom : MonoBehaviour
	{
		public AK.Wwise.Event PlayEvent = new AK.Wwise.Event();

		public SoundBankType SoundBankType;

		private SoundBankLoadService _soundBankLoadService;

		[Inject]
		public void Construct(SoundBankLoadService soundBankLoadService)
		{
			_soundBankLoadService = soundBankLoadService;
		}

		private void OnEnable()
		{
			_soundBankLoadService?.TryLoadBank(SoundBankType).Forget();
			_soundBankLoadService?.PostEvent(PlayEvent, SoundBankType, base.gameObject).Forget();
		}

		private void OnDisable()
		{
			if (PlayEvent.IsValid())
			{
				PlayEvent?.Stop(base.gameObject);
			}
			_soundBankLoadService?.TryUnloadBank(SoundBankType);
		}
	}
}
