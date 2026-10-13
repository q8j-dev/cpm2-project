namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class MainScope : VisualTuningScope
	{
		private int currentRowSelected;

		public int MaxRowIndex { get; set; }

		public override void Init()
		{
		}

		public override void Apply()
		{
		}

		public override void Revert()
		{
		}

		public override void Select(int index)
		{
		}

		public int Next()
		{
			currentRowSelected++;
			if (currentRowSelected > MaxRowIndex)
			{
				currentRowSelected = 0;
			}
			return 1;
		}

		public int Prev()
		{
			currentRowSelected--;
			if (currentRowSelected < 0)
			{
				currentRowSelected = MaxRowIndex;
			}
			return 0;
		}
	}
}
