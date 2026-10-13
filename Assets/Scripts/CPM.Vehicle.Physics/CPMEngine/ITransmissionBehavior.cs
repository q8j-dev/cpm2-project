namespace CPMEngine
{
	public interface ITransmissionBehavior
	{
		void Tick(ITransmissionHost h, float dt);

		bool WantsShift(ITransmissionHost h, out int gear);
	}
}
