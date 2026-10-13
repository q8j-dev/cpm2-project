using System;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	public abstract class PlaceUIItem<TC, TD> : MonoBehaviour
	{
		[SerializeField]
		private int Index = default;

		private bool _isChosen;

		protected TC config;

		private TD _dataItem;

		protected int index => Index;

		public TD DataItem => _dataItem;

		public bool IsChosen
		{
			get
			{
				return _isChosen;
			}
			set
			{
				_isChosen = value;
				Toggled?.Invoke(Index);
				OnChosenChanged();
			}
		}

		public event Action<int> Toggled;

		[Inject]
		public void Construct(TC config)
		{
			this.config = config;
		}

		public virtual void Init()
		{
			_dataItem = GetData(Index);
		}

		protected virtual void OnChosenChanged()
		{
		}

		protected abstract TD GetData(int index);
	}
}
