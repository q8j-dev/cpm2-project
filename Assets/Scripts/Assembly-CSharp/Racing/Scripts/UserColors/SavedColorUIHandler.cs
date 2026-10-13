using Racing.Scripts.UIGrid;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace Racing.Scripts.UserColors
{
	public class SavedColorUIHandler : GridController<SavedColorUIElement>
	{
		private SavedColorCollection _savedColorCollection;

		public ColorPicker colorPicker;

		[field: SerializeField]
		public VisualTuningWindow VisualTuningWindow { get; set; }

		protected override int ElementCount => _savedColorCollection.Count;

		protected override int MaxElementCount => 10;

		[Inject]
		public void Construct(SavedColorCollection savedColorCollection, IPrefabFactory prefabFactory)
		{
			_prefabFactory = prefabFactory;
			_savedColorCollection = savedColorCollection;
		}

		private void OnEnable()
		{
			_savedColorCollection.OnColorAdded += base.OnElementCreated;
			_savedColorCollection.OnColorAdded += UnselectCurrent;
			_savedColorCollection.OnColorChanged += base.OnElementDataChanged;
			_savedColorCollection.OnColorRemoved += base.OnElementDeleted;
		}

		private void OnDisable()
		{
			_savedColorCollection.OnColorAdded -= base.OnElementCreated;
			_savedColorCollection.OnColorAdded -= UnselectCurrent;
			_savedColorCollection.OnColorChanged -= base.OnElementDataChanged;
			_savedColorCollection.OnColorRemoved -= base.OnElementDeleted;
		}

		private void UnselectCurrent(int id)
		{
			if (_currentSelect != null)
			{
				_currentSelect.SetSelected(isSelected: false);
			}
		}
	}
}
