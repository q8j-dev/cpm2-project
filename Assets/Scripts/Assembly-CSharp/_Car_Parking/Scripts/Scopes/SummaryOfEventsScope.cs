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
		private SummaryOfEventsView summaryOfEventsView;

		[SerializeField]
		private SummaryOfEventsConfig summaryOfEventsConfig;

		[SerializeField]
		private PatchNotesConfig patchNotesConfig;

		[SerializeField]
		private MainMenuWindow MainMenuWindow;

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
