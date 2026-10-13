using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class CarExchangeCircle : BaseCircleNotification
	{
		protected override void HandleRequestAnswer(RequestAnswer requestAnswer)
		{
			switch (requestAnswer)
			{
			case RequestAnswer.Ignore:
				Debug.Log("Ignore exchange");
				break;
			case RequestAnswer.Details:
				return;
			}
			Close();
		}

		protected override void TimeOut()
		{
			HandleRequestAnswer(RequestAnswer.Ignore);
		}
	}
}
