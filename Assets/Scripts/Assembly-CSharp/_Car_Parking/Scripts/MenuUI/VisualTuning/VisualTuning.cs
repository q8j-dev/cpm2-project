using System;
using System.Collections.Generic;
using CPM.Scripts.VinylDrawer;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class VisualTuning : IInitializable, IDisposable
	{
		private const string Wheels = "wheels";

		private const string BodyKits = "bodykits";

		private const string Vinyls = "vinyls";

		private const string Flags = "flags";

		private const string Fso = "fso";

		private const string WindowVinyls = "windowVinyls";

		private const string Accessories = "accessories";

		private const string PlateVinyls = "plateVinyls";

		private const string Colors = "colors";

		private const string Main = "main";

		private const string Brakes = "brakes";

		private const string Calipers = "calipers";

		private const string Police = "police";

		public Dictionary<string, VisualTuningScope> Scopes = new Dictionary<string, VisualTuningScope>();

		public Dictionary<VisualTuningTypes, VisualTuningScope> ScopesByType = new Dictionary<VisualTuningTypes, VisualTuningScope>();

		public WheelsTuningScope WheelsTuningScope;

		public ColorsScope ColorsScope;

		public MainScope MainScope;

		private WheelsTuningScope _wheelsTuningScope;

		private ColorsScope _colorsScope;

		private MainScope _mainScope;

		private FlagsVisualScope _flagsVisualScope;

		private VehicleData _vehicleData;

		private VinylDrawer _vinylDrawer;

		private BodyKitsVisualScope _bodyKitsVisualScope;

		private PlayerDatas.VinylsDataUtility _vinylsDataUtility;

		private BrakesVisualScope _brakesVisualScope;

		private CalipersVisualTuningScope _calipersVisualTuningScope;

		private PoliceVisualScope _policeVisualScope;

		private FreeDriveProvider _freeDriveProvider;

		public VisualTuningScope ActiveScope { get; set; }

		public event Action OnValuesChanged;

		[Inject]
		public void Construct(WheelsTuningScope wheelsTuningScope, MainScope mainScope, ColorsScope colorsScope, FlagsVisualScope flagsVisualScope, BodyKitsVisualScope bodyKitsVisualScope, VinylDrawer vinylDrawer, PlayerDatas.VinylsDataUtility vinylsDataUtility, BrakesVisualScope brakesVisualScope, CalipersVisualTuningScope calipersVisualTuningScope, PoliceVisualScope policeVisualScope, FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
			_vinylsDataUtility = vinylsDataUtility;
			_bodyKitsVisualScope = bodyKitsVisualScope;
			_vinylDrawer = vinylDrawer;
			_flagsVisualScope = flagsVisualScope;
			_wheelsTuningScope = wheelsTuningScope;
			_mainScope = mainScope;
			_colorsScope = colorsScope;
			_brakesVisualScope = brakesVisualScope;
			_calipersVisualTuningScope = calipersVisualTuningScope;
			_policeVisualScope = policeVisualScope;
			_bodyKitsVisualScope.Register(this);
		}

		public void Initialize()
		{
			Init();
		}

		public void Init()
		{
			Scopes = new Dictionary<string, VisualTuningScope>
			{
				["wheels"] = _wheelsTuningScope,
				["colors"] = _colorsScope,
				["main"] = _mainScope,
				["flags"] = _flagsVisualScope,
				["bodykits"] = _bodyKitsVisualScope,
				["brakes"] = _brakesVisualScope,
				["calipers"] = _calipersVisualTuningScope,
				["police"] = _policeVisualScope
			};
			ScopesByType = new Dictionary<VisualTuningTypes, VisualTuningScope>
			{
				[VisualTuningTypes.Wheels] = _wheelsTuningScope,
				[VisualTuningTypes.Flags] = _flagsVisualScope,
				[VisualTuningTypes.Main] = _mainScope,
				[VisualTuningTypes.Colors] = _colorsScope,
				[VisualTuningTypes.BodyKits] = _bodyKitsVisualScope,
				[VisualTuningTypes.Brakes] = _brakesVisualScope,
				[VisualTuningTypes.Calipers] = _calipersVisualTuningScope,
				[VisualTuningTypes.Police] = _policeVisualScope
			};
			foreach (VisualTuningScope value in Scopes.Values)
			{
				value.OnValuesChanged += OnValuesChangedInvoke;
			}
		}

		public void Dispose()
		{
			foreach (VisualTuningScope value in Scopes.Values)
			{
				value.OnValuesChanged -= OnValuesChangedInvoke;
			}
		}

		private void OnValuesChangedInvoke()
		{
			OnValuesChanged?.Invoke();
		}

		public void PlaceCamera(int pos)
		{
			switch (pos)
			{
			case 3:
				_freeDriveProvider.CarCameras.mycamera = CarCameras.Cameras.CraftTable;
				_freeDriveProvider.CarCameras.pitchAngle = 65f;
				_freeDriveProvider.CarCameras.yawAngle = 180f;
				_freeDriveProvider.CarCameras.height = 2f;
				break;
			case 7:
				_freeDriveProvider.CarCameras.mycamera = CarCameras.Cameras.InsideCarForAccessories;
				break;
			default:
				_freeDriveProvider.CarCameras.mycamera = CarCameras.Cameras.SmoothLookAt;
				_freeDriveProvider.CarCameras.pitchAngle = -2.5f;
				break;
			}
		}
	}
}
