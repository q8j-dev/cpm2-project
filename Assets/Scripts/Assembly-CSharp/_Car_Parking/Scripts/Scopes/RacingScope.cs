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
		private CoreGameManager GameManager;

		[SerializeField]
		private PersonCameraControll PersonCameraControl;

		[SerializeField]
		private CarOutInControl CarOutInControl;

		[SerializeField]
		private DragRacingCamera DragRacingCamera;

		[SerializeField]
		private Camera MainCamera;

		[SerializeField]
		private ManagerUI ManagerUI;

		[SerializeField]
		private HudColorChanger HudColorChanger;

		[SerializeField]
		private RaceFinishUI RaceFinishUI;

		[SerializeField]
		private DragRacingUI DragRacingUI;

		[SerializeField]
		private DragRacingController DragRacingController;

		[SerializeField]
		private DragRacingTree DragRacingTree;

		[SerializeField]
		private UGUISteeringWheel UGUISteeringWheel;

		[SerializeField]
		private QuickCarSettings QuickCarSettings;

		[SerializeField]
		private MenuLevelParent MenuLevelParent;

		[SerializeField]
		private UIReferencesMediator UiReferencesMediator;

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
