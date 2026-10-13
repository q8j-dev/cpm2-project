using CPM.Account;
using CPM.Connection;
using CPM.DailyTasks;
using VContainer;

namespace Rating.DailyTasks.Scripts
{
	public class DailyServicesInitializer : IDailyServicesDependant
	{
		private readonly DailyTasksManager dailyTasksManager;

		private readonly IAccountController accountController;

		[Inject]
		public DailyServicesInitializer(DailyTasksManager dailyTasksManager, IConnectionDataHandler connectionDataHandler, IAccountController accountController)
		{
			this.accountController = accountController;
			this.dailyTasksManager = dailyTasksManager;
			connectionDataHandler.SubscribeForDailyServicesInitialization(this);
		}

		public void Initialize(double timestamp)
		{
			if (accountController.Authorized && accountController.HasEmail)
			{
				dailyTasksManager.timeMilliseconds = timestamp;
			}
		}
	}
}
