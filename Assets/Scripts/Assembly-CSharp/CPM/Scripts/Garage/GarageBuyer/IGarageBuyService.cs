using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace CPM.Scripts.Garage.GarageBuyer
{
	[Preserve]
	public interface IGarageBuyService
	{
		Dictionary<int, bool> BuyingGaragesStatus { get; }

		int LastBuyingGarageId { get; }

		event Action<int> GarageBought;

		void Init();

		void ChangeToDefault();

		void GarageBuySuccess(int garageIndex);

		UniTask<PurchaseItemResult> ProcessGarageRestore(int garageIndex, PurchaseData purchaseData);

		AdvancedBoolean GetGarageStatus(int garageIndex);

		void SaveBuyingProgress();

		UniTask SetBlockGarage2Purchase(bool value);
	}
}
