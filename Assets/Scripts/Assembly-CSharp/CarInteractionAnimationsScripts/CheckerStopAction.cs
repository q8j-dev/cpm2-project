using System.Threading;
using CarAnimationScripts;

namespace CarInteractionAnimationsScripts
{
	public class CheckerStopAction
	{
		public bool CheckStop(PersonAnimationsController personAnimationsController, CarAnimationsController carAnimController, CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested && (bool)personAnimationsController && (bool)carAnimController && !personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsForceSeatIntoCar)
			{
				return personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsForceGetOutCar;
			}
			return true;
		}
	}
}
