internal static class DriverHandHelpers
{
	public static void Shiftgear()
	{
		if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<DriverActionController>(out var component))
		{
			component.Shiftgear();
		}
	}

	public static void GripWheel()
	{
		if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<DriverActionController>(out var component))
		{
			component.GripWheel();
		}
	}
}
