using UnityEngine;

namespace Racing.Scripts.UIGrid
{
	public interface IGridElement<T> where T : MonoBehaviour, IGridElement<T>
	{
		int ID { get; set; }

		void Init(GridController<T> gridController);

		void Populate(int elementId, bool isInteractable = true, bool ignoreEmptySlots = false);

		void InitAddButton();

		void SetSelected(bool isSelected);

		void DeleteElement();
	}
}
