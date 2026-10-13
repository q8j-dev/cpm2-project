using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.SendMoney
{
	public class MoneyTradeService
	{
		public List<MoneySent> MoneySentList = new List<MoneySent>();

		public Dictionary<string, MoneySent> Moneys = new Dictionary<string, MoneySent>();

		public void CancelMoneySending(string id)
		{
			TryRemove(id);
		}

		private void TryRemove(string id)
		{
			if (Moneys.ContainsKey(id))
			{
				Moneys.Remove(id);
			}
		}

		public void ConfirmReceived(string id)
		{
		}

		public void CancelReceived(string id)
		{
		}
	}
}
