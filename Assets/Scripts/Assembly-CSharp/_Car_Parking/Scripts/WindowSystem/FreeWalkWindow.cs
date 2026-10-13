using CPM.Scripts.Garage.FreeDriverService;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Person.Locomotion;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class FreeWalkWindow : WindowScript
	{
		private IFreeDriverService _freeDriverService;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Person;

		protected override bool ChangeRootGameInputGroup => true;

		[Inject]
		public void Construct(IFreeDriverService freeDriverService)
		{
			_freeDriverService = freeDriverService;
		}

		protected override void OnEnable()
		{
			_managerUI.OnBackButtonActivated(obj: true);
			base.OnEnable();
		}

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				base.Close();
				FlagsStorage.FreeWalk = false;
				V.DriverPerson.GetComponent<NewMoveController>().StopMove();
				if (V.DriverPerson.TryGetComponent<PersonLocomotion>(out var component))
				{
					component.ResetToRest();
				}
				_freeDriverService.SetupFreeDriver();
				_managerUI.OnBackButtonActivated(obj: false);
			}
		}
	}
}
