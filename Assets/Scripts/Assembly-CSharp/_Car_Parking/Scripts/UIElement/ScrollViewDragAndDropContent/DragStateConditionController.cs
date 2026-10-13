namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public abstract class DragStateConditionController
	{
		protected ScrollViewDragAndDropItemContent scrollView;

		public abstract bool CanAddToEmpty { get; }

		public abstract bool CanAddNewItem { get; }

		public virtual void Init(ScrollViewDragAndDropItemContent scrollViewDragAndDropItemContent)
		{
			scrollView = scrollViewDragAndDropItemContent;
		}

		public virtual void Enter()
		{
		}

		public virtual void Exit()
		{
		}

		public virtual void FailToDragEmpty()
		{
		}

		public virtual void FailToDragNewPlace()
		{
		}

		public virtual void OnDataUpdated()
		{
		}
	}
}
