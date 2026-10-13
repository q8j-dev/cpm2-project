using System;

namespace _Car_Parking.Scripts.GameFacts.Consumers
{
	public interface IContextCondition
	{
		bool IsTrue { get; }

		event Action Changed;
	}
}
