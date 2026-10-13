using UnityEngine;
using UnityEngine.Purchasing;
using _Car_Parking.Scripts.Other.Cypher;

namespace _Car_Parking.Scripts.Wallet
{
	public class PurchaseData
	{
		public string Value;

		public string ProductId;

		public string TransactionId;

		public string Platform;

		public int OrderId;

		public PurchaseData(Product argsPurchasedProduct, int value, bool cypherData = true)
		{
			Value = (cypherData ? WalletCypher.Encode(value.ToString(), argsPurchasedProduct.transactionID) : value.ToString());
			ProductId = (cypherData ? WalletCypher.Encode(argsPurchasedProduct.definition.id, argsPurchasedProduct.transactionID) : argsPurchasedProduct.definition.id);
			TransactionId = argsPurchasedProduct.transactionID;
			Platform = Application.platform.ToString();
		}

		public PurchaseData(string value)
		{
			string text = "debugTransactionId" + Random.Range(0, 100000);
			Value = WalletCypher.Encode(value, text);
			ProductId = WalletCypher.Encode("Debug Buy", text);
			TransactionId = text;
			Platform = "IPhonePlayer";
		}

		public object[] ToArrayData()
		{
			return new object[4] { Value, ProductId, TransactionId, Platform };
		}
	}
}
