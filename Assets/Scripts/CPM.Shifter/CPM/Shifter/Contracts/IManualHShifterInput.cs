using System;

namespace CPM.Shifter.Contracts
{
	public interface IManualHShifterInput
	{
		bool IsTouching { get; }

		event Action<int> OnGearRequested;

		event Action<int> OnPreviewGearChanged;

		event Action OnGrabStarted;

		event Action OnGrabReleased;

		void Bind(IManualHShifterView view, Func<float> clutchProvider);

		void Unbind();
	}
}
