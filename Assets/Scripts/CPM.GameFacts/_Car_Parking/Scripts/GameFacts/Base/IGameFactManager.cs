using System;

namespace _Car_Parking.Scripts.GameFacts.Base
{
	public interface IGameFactManager
	{
		event Action<GameFact> OnChanged;

		void Set(GameFact gameFact, object source, bool active);

		bool IsTrue(GameFact gameFact);

		void Release(object source);
	}
}
