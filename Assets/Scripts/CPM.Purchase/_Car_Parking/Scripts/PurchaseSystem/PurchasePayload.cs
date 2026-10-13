using UnityEngine;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public abstract class PurchasePayload
	{
		public string PaymentProductId;

		public string TransactionId;

		public string Platform;

		public string AttemptId;

		public int Type;

		public PurchasePayload(string bundleID, string transactionId, string attemptId, PurchasePayloadType type)
		{
			PaymentProductId = bundleID;
			TransactionId = transactionId;
			AttemptId = attemptId;
			Type = (int)type;
			Platform = Application.platform.ToString();
		}
	}
}
