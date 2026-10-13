namespace MVPFree
{
	public interface IUpdater
	{
		void Subscribe();

		void Unsubscribe();

		void Update(float deltaTime);
	}
}
