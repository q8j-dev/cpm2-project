using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.SummaryOfEvents;

namespace _Car_Parking.Scripts.Scopes
{
	public class SummaryOfEventsScope : LifetimeScope
	{
		[SerializeField]
		private SummaryOfEventsView summaryOfEventsView = default;

		[SerializeField]
		private SummaryOfEventsConfig summaryOfEventsConfig = default;

		[SerializeField]
		private PatchNotesConfig patchNotesConfig = default;

		[SerializeField]
		private MainMenuWindow MainMenuWindow = default;

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(summaryOfEventsView);
			builder.RegisterInstance(summaryOfEventsConfig);
			builder.RegisterInstance(patchNotesConfig);
			builder.RegisterEntryPoint<SummaryOfEventsPresenter>();
			builder.RegisterInstance(MainMenuWindow);
		}
	}
}
