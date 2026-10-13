using CPM.Database.Constructors;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.SOAP_Registry;

public class RacingCarChangeTyreWindow : WindowScript, IBackableWindow
{
	[SerializeField]
	private Button BackButton = default;

	[SerializeField]
	private TyreTypeUIElement[] TyreTypes = default;

	private VehicleEntityProvider _vehicleEntityProvider;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(ManagerUI managerUI)
	{
		_managerUI = managerUI;
	}

	protected override void Start()
	{
		_vehicleEntityProvider = V.Car.GetComponent<VehicleEntityProvider>();
		BackButton.onClick.AddListener(Back);
		base.Start();
		InitBackButton();
		SelectInstalledTyre();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		BackButton.SetActive(active: false);
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	private void SelectInstalledTyre()
	{
		TyreType tyreType = _vehicleEntityProvider.GetVehicleData.Axles.FrontAxle.LeftWheel.TyreType;
		int carId = _vehicleEntityProvider.GetVehicleData.CarId;
		for (int i = 0; i < TyreTypes.Length; i++)
		{
			bool flag = TyreClassRules.IsTyreAllowed(TyreTypes[i].TyreType, carId);
			TyreTypes[i].gameObject.SetActive(flag);
			TyreTypes[i].Select(flag && tyreType == TyreTypes[i].TyreType);
		}
	}

	public void Back()
	{
		_managerUI.ShowWindow("RallyMenuWindow").Forget();
	}

	public void ChangeTyre(TyreType tyreType)
	{
		_vehicleEntityProvider.CarEntity.FrontWheel.tyreType = tyreType;
		_ = WheelConstructor.SetWheelModel(V.Car, AxleType.Front, _vehicleEntityProvider.CarEntity.FrontWheel);
		_ = WheelConstructor.SetWheelModel(V.Car, AxleType.Rear, _vehicleEntityProvider.CarEntity.FrontWheel);
	}
}
