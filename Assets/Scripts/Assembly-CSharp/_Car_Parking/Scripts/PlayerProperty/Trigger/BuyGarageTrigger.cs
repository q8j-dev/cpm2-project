using CPM.Scripts.Garage.GarageBuyer;
using MVPFree.MapIcons.Enum;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.PlayerProperty.Processor;

namespace _Car_Parking.Scripts.PlayerProperty.Trigger
{
	public class BuyGarageTrigger : PropertyTrigger
	{
		[SerializeField]
		private int ID;

		private IGarageBuyService _garageBuyService;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		private void Construct(IGarageBuyService garageBuyService)
		{
			_garageBuyService = garageBuyService;
		}

		protected override void Start()
		{
			base.Start();
			if (_garageBuyService != null)
			{
				bool flag = !_garageBuyService.BuyingGaragesStatus[ID];
				base.gameObject.SetActive(flag);
				SetEnabled(flag);
				InitializeProcessor(new GarageBuyProcessor(ID));
				if (flag)
				{
					SetMainImage(IconType.BuyGarage);
				}
				_garageBuyService.GarageBought += OnGarageBought;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (_garageBuyService != null)
			{
				_garageBuyService.GarageBought -= OnGarageBought;
			}
		}

		private void OnGarageBought(int garageId)
		{
			if (garageId == ID)
			{
				HideTriggerVisual();
				SetEnabled(value: false);
			}
		}
	}
}
