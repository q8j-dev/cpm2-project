namespace Gley.TrafficSystem.Internal
{
	public class SoundManager
	{
		private float _masterVolume;

		public float MasterVolume => _masterVolume;

		public SoundManager(float masterVolume)
		{
			_masterVolume = masterVolume;
		}

		public void UpdateMasterVolume(float volume)
		{
			_masterVolume = volume;
		}
	}
}
