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
		private CoreGameManager CoreGameManager;

		[SerializeField]
		private LevelScript LevelScript;

		[SerializeField]
		private Camera Camera;

		[SerializeField]
		private ParkingMenuUI ParkingMenuUI;

		[SerializeField]
		private HudColorChanger HudColorChanger;

		[SerializeField]
		private UGUISteeringWheel UGUISteeringWheel;

		[SerializeField]
		private MenuLevelParent MenuLevelParent;

		[SerializeField]
		private QuickCarSettings QuickCarSettings;

		[SerializeField]
		private ManagerUI ManagerUI;

		[SerializeField]
		private UIReferencesMediator UIReferencesMediator;

		[SerializeField]
		private ParkTronicControll ParkTronicControll;

		[SerializeField]
		private VinylDrawer VinylDrawer;

		[SerializeField]
		private TrafficCarControl TrafficCarControl;

		[SerializeField]
		private TouchField TouchField;

		[SerializeField]
		private LoadControl LoadControl;

		[SerializeField]
		private ThrottleSliderControl ThrottleSliderControl;

		[SerializeField]
		private FPSDisplay2 FPSDisplay2;

		[SerializeField]
		private CarCameras CarCameras;

		[SerializeField]
		private BirdPoint BirdPoint;

		[SerializeField]
		private PedestrainCreate PedestrainCreate;

		[SerializeField]
		private JoyStickControl JoyStickControl;

		[SerializeField]
		private InputTouch InputTouch;

		[SerializeField]
		private BrakeSliderControl BrakeSliderControl;

		[SerializeField]
		private LevelUIController LevelUIController;

		[SerializeField]
		private CarCamerasController CarCamerasController;

		[SerializeField]
		private TouchMovePerson TouchMovePerson;

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
