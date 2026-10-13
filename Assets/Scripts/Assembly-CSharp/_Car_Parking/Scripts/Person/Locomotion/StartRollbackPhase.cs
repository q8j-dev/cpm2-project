namespace _Car_Parking.Scripts.Person.Locomotion
{
	public enum StartRollbackPhase
	{
		Idle = 0,
		Uncommitted = 1,
		RollingBack = 2,
		Committed = 3,
		Finished = 4
	}
}
