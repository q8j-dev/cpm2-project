using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.Car.Sounds
{
	public class SoundControllerExtension
	{
		private readonly RainSoundState _rainSoundState;

		public SoundControllerExtension(RainSoundState rainSoundState)
		{
			_rainSoundState = rainSoundState;
		}

		public void UpdateInCarSound(int carID)
		{
			bool flag = carID != 155 && carID != 165;
			AudioWrapper.SetState(SoundController.StateCarInOut, flag ? SoundController.InCar : SoundController.OutCar);
			_rainSoundState.SetInCar(flag);
		}

		public void SetOutCarSound()
		{
			AudioWrapper.SetState(SoundController.StateCarInOut, SoundController.OutCar);
			_rainSoundState.SetInCar(inCar: false);
		}
	}
}
