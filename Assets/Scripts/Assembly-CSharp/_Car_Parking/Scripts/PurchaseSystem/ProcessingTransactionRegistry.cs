using System.Collections.Generic;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class ProcessingTransactionRegistry
	{
		private readonly HashSet<string> _transactionIds = new HashSet<string>();

		public bool TryBegin(string transactionId)
		{
			return _transactionIds.Add(transactionId);
		}

		public void Release(string transactionId)
		{
			_transactionIds.Remove(transactionId);
		}
	}
}
