using CodeBase.Infrastructure.States;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.PoliceLights
{
	public class PoliceLightsStates : MonoBehaviour
	{
		public class EnabledState : IState, IExitableState
		{
			private PoliceLightControl _policeLightControl;

			public EnabledState(PoliceLightControl policeLightControl)
			{
				_policeLightControl = policeLightControl;
			}

			public void Enter()
			{
			}

			public void Exit()
			{
			}
		}

		public class EnabledLightsOnlyState : IState, IExitableState
		{
			private PoliceLightControl _policeLightControl;

			public EnabledLightsOnlyState(PoliceLightControl policeLightControl)
			{
				_policeLightControl = policeLightControl;
			}

			public void Enter()
			{
			}

			public void Exit()
			{
			}
		}

		public class DisabledState : IState, IExitableState
		{
			private PoliceLightControl _policeLightControl;

			public DisabledState(PoliceLightControl policeLightControl)
			{
				_policeLightControl = policeLightControl;
			}

			public void Enter()
			{
			}

			public void Exit()
			{
			}
		}
	}
}
