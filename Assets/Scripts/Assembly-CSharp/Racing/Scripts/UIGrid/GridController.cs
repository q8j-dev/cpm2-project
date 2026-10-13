using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Infrastructure;

namespace Racing.Scripts.UIGrid
{
	public abstract class GridController<T> : MonoBehaviour where T : MonoBehaviour, IGridElement<T>
	{
		[SerializeField]
		private T gridElementPrefab;

		[SerializeField]
		private string gridElementPrefabPath;

		[SerializeField]
		private bool isAutoPopulate = true;

		[SerializeField]
		private LayoutGroup layoutGroup;

		[SerializeField]
		private bool hasAddButton;

		[SerializeField]
		private bool isButtonsInteractable = true;

		[SerializeField]
		private bool isElementDestroyedOnDelete;

		public bool ignoreEmptySlots;

		protected List<T> _gridElements = new List<T>();

		protected T _currentSelect;

		protected IPrefabFactory _prefabFactory;

		protected abstract int ElementCount { get; }

		protected abstract int MaxElementCount { get; }

		protected virtual void Start()
		{
			if (isAutoPopulate)
			{
				PopulateGrid();
			}
		}

		protected void PopulateGrid()
		{
			T prefab = (gridElementPrefab ? gridElementPrefab : Resources.Load<T>(gridElementPrefabPath));
			for (int i = 0; i < ElementCount; i++)
			{
				T val = InstantiateGridElement(prefab);
				val.Populate(i, isButtonsInteractable, ignoreEmptySlots);
				PresetElement(val);
			}
			if (hasAddButton && ElementCount < MaxElementCount)
			{
				CreateAddButton();
			}
			RecalculateLayout().Forget();
		}

		protected virtual void PresetElement(T element)
		{
		}

		protected void RepopulateGrid()
		{
			for (int i = 0; i < ElementCount; i++)
			{
				_gridElements[i].Populate(i, isButtonsInteractable, ignoreEmptySlots);
			}
		}

		protected void OnElementCreated(int id)
		{
			_gridElements[id].Populate(id, isButtonsInteractable, ignoreEmptySlots);
			if (hasAddButton && ElementCount < MaxElementCount)
			{
				CreateAddButton();
			}
		}

		protected void OnElementDataChanged(int id)
		{
			_gridElements[id].Populate(id, isButtonsInteractable, ignoreEmptySlots);
		}

		protected void OnElementDeleted(int id)
		{
			T val = _gridElements[id];
			bool flag = ElementCount + 1 == MaxElementCount;
			val.DeleteElement();
			if (isElementDestroyedOnDelete)
			{
				_gridElements.Remove(val);
				Object.Destroy(val.gameObject);
			}
			RepopulateGrid();
			if (hasAddButton & flag)
			{
				CreateAddButton();
			}
			RecalculateLayout().Forget();
		}

		public void OnElementSelect(T gridElement)
		{
			if (_currentSelect != null)
			{
				_currentSelect.SetSelected(isSelected: false);
			}
			_currentSelect = gridElement;
			_currentSelect.SetSelected(isSelected: true);
		}

		public void SelectElementById(int id)
		{
			if (_gridElements != null && _gridElements.Count > id)
			{
				OnElementSelect(_gridElements[id]);
			}
		}

		private void CreateAddButton()
		{
			T prefab = (gridElementPrefab ? gridElementPrefab : Resources.Load<T>(gridElementPrefabPath));
			InstantiateGridElement(prefab).InitAddButton();
		}

		private async UniTaskVoid RecalculateLayout()
		{
			await UniTask.DelayFrame(1);
			if ((bool)layoutGroup)
			{
				layoutGroup.CalculateLayoutInputHorizontal();
				layoutGroup.CalculateLayoutInputVertical();
			}
		}

		private T InstantiateGridElement(T prefab)
		{
			T val = _prefabFactory.Instantiate(prefab, layoutGroup.transform);
			val.Init(this);
			_gridElements.Add(val);
			return val;
		}
	}
}
