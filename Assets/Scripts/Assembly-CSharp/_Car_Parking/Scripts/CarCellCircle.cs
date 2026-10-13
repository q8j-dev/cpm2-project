using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class CarCellCircle : BaseCircleNotification
	{
		protected override void HandleRequestAnswer(RequestAnswer requestAnswer)
		{
			switch (requestAnswer)
			{
			case RequestAnswer.Ignore:
				Debug.Log("Ignore buy");
				break;
			case RequestAnswer.Details:
				return;
			}
			Close();
		}
	}
}
