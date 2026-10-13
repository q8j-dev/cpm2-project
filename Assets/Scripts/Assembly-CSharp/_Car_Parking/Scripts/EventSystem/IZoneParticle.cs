namespace _Car_Parking.Scripts.EventSystem
{
	public interface IZoneParticle
	{
		void ChangeState(bool state);

		void PlayParticleSystem();
	}
}
