using System;
using UnityEngine;
using UnityEngine.UI;

namespace PolyAndCode.UI
{
	public class RecyclableScrollRect : ScrollRect
	{
		public enum DirectionType
		{
			Vertical = 0,
			Horizontal = 1
		}

		[HideInInspector]
		public IRecyclableScrollRectDataSource DataSource;

		public bool IsGrid;

		public RectTransform PrototypeCell;

		public bool SelfInitialize = true;

		public DirectionType Direction;

		[SerializeField]
		private int _segments;

		private RecyclingSystem _recyclingSystem;

		private Vector2 _prevAnchoredPos;

		public int Segments
		{
			get
			{
				return _segments;
			}
			set
			{
				_segments = Math.Max(value, 2);
			}
		}

		protected override void Start()
		{
			base.vertical = true;
			base.horizontal = false;
			if (Application.isPlaying && SelfInitialize)
			{
				Initialize();
			}
		}

		private void Initialize()
		{
			if (Direction == DirectionType.Vertical)
			{
				_recyclingSystem = new VerticalRecyclingSystem(PrototypeCell, base.viewport, base.content, DataSource, IsGrid, Segments);
			}
			else if (Direction == DirectionType.Horizontal)
			{
				_recyclingSystem = new HorizontalRecyclingSystem(PrototypeCell, base.viewport, base.content, DataSource, IsGrid, Segments);
			}
			base.vertical = Direction == DirectionType.Vertical;
			base.horizontal = Direction == DirectionType.Horizontal;
			_prevAnchoredPos = base.content.anchoredPosition;
			base.onValueChanged.RemoveListener(OnValueChangedListener);
			StartCoroutine(_recyclingSystem.InitCoroutine(() =>
			{
				base.onValueChanged.AddListener(OnValueChangedListener);
			}));
		}

		public void Initialize(IRecyclableScrollRectDataSource dataSource)
		{
			DataSource = dataSource;
			Initialize();
		}

		public void OnValueChangedListener(Vector2 normalizedPos)
		{
			Vector2 direction = base.content.anchoredPosition - _prevAnchoredPos;
			m_ContentStartPosition += _recyclingSystem.OnValueChangedListener(direction);
			_prevAnchoredPos = base.content.anchoredPosition;
		}

		public void ReloadData()
		{
			ReloadData(DataSource);
		}

		public void ReloadData(IRecyclableScrollRectDataSource dataSource)
		{
			if (_recyclingSystem != null)
			{
				StopMovement();
				base.onValueChanged.RemoveListener(OnValueChangedListener);
				_recyclingSystem.DataSource = dataSource;
				StartCoroutine(_recyclingSystem.InitCoroutine(() =>
				{
					base.onValueChanged.AddListener(OnValueChangedListener);
				}));
				_prevAnchoredPos = base.content.anchoredPosition;
			}
		}
	}
}
