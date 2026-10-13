namespace Service
{
	public interface ITask
	{
		void AddCollision();

		uint GetCollisionCount();

		void ResetAnimation();

		void SetState(TASK_STATE state);
	}
}
