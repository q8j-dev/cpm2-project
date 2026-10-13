using CPM.Scripts.VinylDrawer;
using Photon.Pun;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.DiscordPresence;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.UIElement.HUD;
using _Car_Parking.Scripts.WindowSystem.Mediators;

namespace _Car_Parking.Scripts.Scopes
{
	public class ParkingLevelsScope : LifetimeScope
	{
		[SerializeField]
		private CoreGameManager CoreGameManager = default;

		[SerializeField]
		private LevelScript LevelScript = default;

		[SerializeField]
		private Camera Camera = default;

		[SerializeField]
		private ParkingMenuUI ParkingMenuUI = default;

		[SerializeField]
		private HudColorChanger HudColorChanger = default;

		[SerializeField]
		private UGUISteeringWheel UGUISteeringWheel = default;

		[SerializeField]
		private MenuLevelParent MenuLevelParent = default;

		[SerializeField]
		private QuickCarSettings QuickCarSettings = default;

		[SerializeField]
		private ManagerUI ManagerUI = default;

		[SerializeField]
		private UIReferencesMediator UIReferencesMediator = default;

		[SerializeField]
		private ParkTronicControll ParkTronicControll = default;

		[SerializeField]
		private VinylDrawer VinylDrawer = default;

		[SerializeField]
		private TrafficCarControl TrafficCarControl = default;

		[SerializeField]
		private TouchField TouchField = default;

		[SerializeField]
		private LoadControl LoadControl = default;

		[SerializeField]
		private ThrottleSliderControl ThrottleSliderControl = default;

		[SerializeField]
		private FPSDisplay2 FPSDisplay2 = default;

		[SerializeField]
		private CarCameras CarCameras = default;

		[SerializeField]
		private BirdPoint BirdPoint = default;

		[SerializeField]
		private PedestrainCreate PedestrainCreate = default;

		[SerializeField]
		private JoyStickControl JoyStickControl = default;

		[SerializeField]
		private InputTouch InputTouch = default;

		[SerializeField]
		private BrakeSliderControl BrakeSliderControl = default;

		[SerializeField]
		private LevelUIController LevelUIController = default;

		[SerializeField]
		private CarCamerasController CarCamerasController = default;

		[SerializeField]
		private TouchMovePerson TouchMovePerson = default;

		protected override void Configure(IContainerBuilder builder)
		{
			CommonScopeUtility.Initialize(builder);
			builder.RegisterEntryPoint<DiscordScenePresence>().WithParameter("kind", DiscordActivityKind.ParkingLevels).WithParameter("reportEnvironment", true);
			builder.RegisterComponent(CoreGameManager);
			builder.RegisterComponent(LevelScript);
			builder.RegisterComponent(Camera);
			builder.RegisterComponent(ParkingMenuUI);
			builder.RegisterComponent(HudColorChanger);
			builder.RegisterComponent(UGUISteeringWheel);
			builder.RegisterComponent(MenuLevelParent);
			builder.RegisterComponent(QuickCarSettings);
			builder.RegisterComponent(ManagerUI);
			builder.RegisterComponent(UIReferencesMediator);
			builder.RegisterComponent(ParkTronicControll);
			builder.RegisterComponent(VinylDrawer);
			if ((bool)TouchField)
			{
				builder.RegisterComponent(TouchField);
			}
			if ((bool)LoadControl)
			{
				builder.RegisterComponent(LoadControl);
			}
			if ((bool)ThrottleSliderControl)
			{
				builder.RegisterComponent(ThrottleSliderControl);
			}
			if ((bool)FPSDisplay2)
			{
				builder.RegisterComponent(FPSDisplay2);
			}
			if ((bool)CarCameras)
			{
				builder.RegisterComponent(CarCameras);
			}
			if ((bool)BirdPoint)
			{
				builder.RegisterComponent(BirdPoint);
			}
			if ((bool)PedestrainCreate)
			{
				builder.RegisterComponent(PedestrainCreate);
			}
			if ((bool)JoyStickControl)
			{
				builder.RegisterComponent(JoyStickControl);
			}
			if ((bool)InputTouch)
			{
				builder.RegisterComponent(InputTouch);
			}
			if ((bool)BrakeSliderControl)
			{
				builder.RegisterComponent(BrakeSliderControl);
			}
			if ((bool)LevelUIController)
			{
				builder.RegisterComponent(LevelUIController);
			}
			if ((bool)CarCamerasController)
			{
				builder.RegisterComponent(CarCamerasController);
			}
			if ((bool)TouchMovePerson)
			{
				builder.RegisterComponent(TouchMovePerson);
			}
			if (TrafficCarControl != null)
			{
				builder.RegisterComponent(TrafficCarControl);
			}
		}

		private void Start()
		{
			CommonScopeUtility.Start(base.Container);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			PhotonNetwork.PrefabPool.ClearResources(destroyDistanceCheckers: true);
		}
	}
}
