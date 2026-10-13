namespace Service
{
	public interface IServiceController
	{
		void AddTask();

		void RefreshList();

		void ClickStartButton(int? taskID);

		void CreateOrders();

		void OnSceneLoaded();

		void ShowWindow(int i);

		void OnTaskFinished();
	}
}
