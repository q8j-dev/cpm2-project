using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.DiscordPresence;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.UIElement.HUD;
using _Car_Parking.Scripts.WindowSystem.Mediators;

namespace _Car_Parking.Scripts.Scopes
{
	public class RacingScope : LifetimeScope
	{
		[SerializeField]
		private CoreGameManager GameManager = default;

		[SerializeField]
		private PersonCameraControll PersonCameraControl = default;

		[SerializeField]
		private CarOutInControl CarOutInControl = default;

		[SerializeField]
		private DragRacingCamera DragRacingCamera = default;

		[SerializeField]
		private Camera MainCamera = default;

		[SerializeField]
		private ManagerUI ManagerUI = default;

		[SerializeField]
		private HudColorChanger HudColorChanger = default;

		[SerializeField]
		private RaceFinishUI RaceFinishUI = default;

		[SerializeField]
		private DragRacingUI DragRacingUI = default;

		[SerializeField]
		private DragRacingController DragRacingController = default;

		[SerializeField]
		private DragRacingTree DragRacingTree = default;

		[SerializeField]
		private UGUISteeringWheel UGUISteeringWheel = default;

		[SerializeField]
		private QuickCarSettings QuickCarSettings = default;

		[SerializeField]
		private MenuLevelParent MenuLevelParent = default;

		[SerializeField]
		private UIReferencesMediator UiReferencesMediator = default;

		private void Start()
		{
			CommonScopeUtility.Start(base.Container);
		}

		protected override void Configure(IContainerBuilder builder)
		{
			CommonScopeUtility.Initialize(builder);
			builder.RegisterEntryPoint<DiscordScenePresence>().WithParameter("kind", DiscordActivityKind.DragRacing).WithParameter("reportEnvironment", true);
			builder.RegisterComponent(GameManager);
			builder.RegisterInstance(PersonCameraControl);
			builder.RegisterComponent(CarOutInControl);
			builder.RegisterComponent(DragRacingCamera);
			builder.RegisterInstance(MainCamera);
			builder.RegisterComponent(ManagerUI);
			builder.RegisterInstance(HudColorChanger);
			builder.RegisterInstance(RaceFinishUI);
			builder.RegisterComponent(DragRacingUI);
			builder.RegisterComponent(DragRacingController);
			builder.RegisterComponent(DragRacingTree);
			builder.RegisterInstance(UGUISteeringWheel);
			builder.RegisterComponent(QuickCarSettings);
			builder.RegisterComponent(MenuLevelParent);
			builder.RegisterInstance(UiReferencesMediator);
		}
	}
}
