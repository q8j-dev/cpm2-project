using System.Collections.Generic;
using UnityEngine;

namespace MVPFree
{
	public abstract class MvpStarter : MonoBehaviour
	{
		protected List<IPresenter> presenters = new List<IPresenter>();

		protected List<IUpdater> updaters = new List<IUpdater>();

		private void OnEnable()
		{
			foreach (IPresenter presenter in presenters)
			{
				presenter.Subscribe();
			}
			foreach (IUpdater updater in updaters)
			{
				updater.Subscribe();
			}
		}

		private void OnDisable()
		{
			foreach (IPresenter presenter in presenters)
			{
				presenter.Unsubscribe();
			}
			foreach (IUpdater updater in updaters)
			{
				updater.Unsubscribe();
			}
		}

		private void OnDestroy()
		{
			foreach (IPresenter presenter in presenters)
			{
				presenter.Unsubscribe();
			}
			foreach (IUpdater updater in updaters)
			{
				updater.Unsubscribe();
			}
		}

		private void Update()
		{
			foreach (IUpdater updater in updaters)
			{
				updater.Update(Time.deltaTime);
			}
		}
	}
}
