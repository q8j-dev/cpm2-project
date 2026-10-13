namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	public interface IInstallTimeCatalog
	{
		bool TryGet(PartType partType, out float seconds);
	}
}
