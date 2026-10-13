using System;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	[AttributeUsage(AttributeTargets.Class)]
	public class MigratableDataAttribute : Attribute
	{
	}
}
