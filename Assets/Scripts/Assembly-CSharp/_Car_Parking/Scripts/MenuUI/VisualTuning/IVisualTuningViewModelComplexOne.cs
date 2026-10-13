namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public interface IVisualTuningViewModelComplexOne
	{
		void Execute(VisualTuningTypes scopeType, string method, object argument = null);

		VisualTuningScope GetScope(VisualTuningTypes scopeType);
	}
}
