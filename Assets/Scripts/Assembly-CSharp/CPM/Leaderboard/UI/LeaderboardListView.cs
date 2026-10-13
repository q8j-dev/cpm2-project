using System;
using System.Collections.Generic;
using System.Threading;
using CPM.PlayerData;
using CPM.RacingRecords;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.UIElement;

namespace CPM.Leaderboard.UI
{
	public sealed class LeaderboardListView : MonoBehaviour
	{
		[SerializeField]
		private ScrollRect _scrollRect = default;

		[SerializeField]
		private RecordRowUiElement _rowPrefab = default;

		[SerializeField]
		private RecordRowUiElement _selfRow = default;

		[SerializeField]
		private GameObject _emptyState = default;

		[SerializeField]
		private UiScrollRectOcclusiion _scrollRectOcclusion = default;

		private PlayerDataStorage _playerData;

		private IPrefabFactory _prefabFactory;

		private ILeaderboardRecordsSource _source;

		private long _localBest = -1L;

		private RacingRecordsCollection _collection;

		private readonly List<RecordRowUiElement> _rows = new List<RecordRowUiElement>();

		private CancellationTokenSource _cts;

		private string _mapId;

		private string _ownId;

		private RacingRecord _selfRecord;

		private RecordRowUiElement _ownListRow;

		private RecordRowUiElement _provisionalSelfRow;

		private int _provisionalRank;

		private bool _selfHasData;

		private const int TopCapacity = 250;

		private bool _selectable;

		private string _selectedUserId;

		private bool _selectedLocal;

		private static readonly Vector3[] _cornersA = new Vector3[4];

		private static readonly Vector3[] _cornersV = new Vector3[4];

		public event Action<RacingRecord> RowTapped;

		public event Action LocalBestTapped;

		public event Action Rendered;

		[Inject]
		public void Construct(PlayerDataStorage playerData, IPrefabFactory prefabFactory)
		{
			_playerData = playerData;
			_prefabFactory = prefabFactory;
		}

		private void OnEnable()
		{
			if ((bool)_scrollRect)
			{
				_scrollRect.onValueChanged.AddListener(OnScroll);
			}
			if ((bool)_selfRow)
			{
				_selfRow.gameObject.SetActive(value: false);
			}
			if (_source != null)
			{
				_source.Updated -= OnSourceUpdated;
				_source.Updated += OnSourceUpdated;
			}
		}

		private void OnDisable()
		{
			if ((bool)_scrollRect)
			{
				_scrollRect.onValueChanged.RemoveListener(OnScroll);
			}
			if (_source != null)
			{
				_source.Updated -= OnSourceUpdated;
			}
			CancelLoad();
			ClearRows();
			if ((bool)_selfRow)
			{
				_selfRow.gameObject.SetActive(value: false);
			}
		}

		public void Init(string mapId, ILeaderboardRecordsSource source, long localBest)
		{
			if (_source != null)
			{
				_source.Updated -= OnSourceUpdated;
			}
			_mapId = mapId;
			_source = source;
			_localBest = localBest;
			_ownId = _playerData?.AccountID;
			if (base.isActiveAndEnabled && _source != null)
			{
				_source.Updated += OnSourceUpdated;
			}
		}

		public void Refresh()
		{
			if (_source == null || string.IsNullOrEmpty(_mapId))
			{
				ShowEmpty(show: true);
				return;
			}
			CancelLoad();
			_cts = new CancellationTokenSource();
			LoadAndRender().Forget();
		}

		public void RefreshFromCache()
		{
			if (_source != null && !string.IsNullOrEmpty(_mapId) && _source.TryGet(_mapId, out var collection))
			{
				_collection = collection;
				RenderRows();
			}
			else
			{
				ShowEmpty(show: true);
			}
		}

		private async UniTaskVoid LoadAndRender()
		{
			RacingRecordsCollection collection = await _source.GetOrFetch(_mapId, _cts.Token);
			if (_cts != null && !_cts.IsCancellationRequested && (bool)this)
			{
				_collection = collection;
				RenderRows();
			}
		}

		private void OnSourceUpdated(string mapId)
		{
			if ((bool)this && !string.IsNullOrEmpty(_mapId) && !(mapId != _mapId) && _source.TryGet(_mapId, out var collection))
			{
				_collection = collection;
				RenderRows();
			}
		}

		private void RenderRows()
		{
			ClearRows();
			if (_collection == null)
			{
				ShowEmpty(show: true);
				Rendered?.Invoke();
				return;
			}
			IReadOnlyList<RacingRecord> items = _collection.Items;
			int num = ComputeProvisionalIndex(items);
			int num2 = ((num >= 0 && items.Count >= 250) ? 249 : items.Count);
			for (int i = 0; i < num2; i++)
			{
				if (i == num)
				{
					SpawnProvisionalSelfRow(i + 1);
				}
				SpawnRow(items[i], (num >= 0 && i >= num) ? (items[i].Rank + 1) : items[i].Rank);
			}
			if (num == num2)
			{
				SpawnProvisionalSelfRow(num2 + 1);
			}
			ShowEmpty(items.Count == 0 && num < 0);
			BuildSelfRow();
			ApplySelectable();
			ApplySelectionVisuals();
			RefreshOcclusion().Forget();
			Rendered?.Invoke();
		}

		private void SpawnRow(RacingRecord record, int displayRank)
		{
			if ((bool)_rowPrefab && (bool)_scrollRect)
			{
				RecordRowUiElement recordRowUiElement = _prefabFactory.Instantiate(_rowPrefab, _scrollRect.content);
				bool flag = !string.IsNullOrEmpty(_ownId) && record.UserId == _ownId;
				recordRowUiElement.Bind(record, displayRank, flag, OnRowTapped);
				if (flag)
				{
					_ownListRow = recordRowUiElement;
				}
				_rows.Add(recordRowUiElement);
			}
		}

		private int ComputeProvisionalIndex(IReadOnlyList<RacingRecord> items)
		{
			if (_localBest <= 0)
			{
				return -1;
			}
			if (!string.IsNullOrEmpty(_ownId))
			{
				for (int i = 0; i < items.Count; i++)
				{
					if (items[i].UserId == _ownId)
					{
						return -1;
					}
				}
			}
			int j;
			for (j = 0; j < items.Count && items[j].Score <= _localBest; j++)
			{
			}
			if (j >= 250)
			{
				return -1;
			}
			return j;
		}

		private void SpawnProvisionalSelfRow(int rank)
		{
			if ((bool)_rowPrefab && (bool)_scrollRect)
			{
				RecordRowUiElement recordRowUiElement = _prefabFactory.Instantiate(_rowPrefab, _scrollRect.content);
				recordRowUiElement.BindLocalBest(_localBest, OnLocalBestTapped, rank);
				_provisionalSelfRow = recordRowUiElement;
				_provisionalRank = rank;
				_rows.Add(recordRowUiElement);
			}
		}

		private void BuildSelfRow()
		{
			_selfRecord = null;
			_selfHasData = false;
			if ((bool)_selfRow)
			{
				if ((bool)_ownListRow && _ownListRow.Record != null)
				{
					_selfRecord = _ownListRow.Record;
					_selfRow.Bind(_selfRecord, _selfRecord.Rank, isOwn: true, OnRowTapped);
					_selfHasData = true;
				}
				else if ((bool)_provisionalSelfRow)
				{
					_selfRow.BindLocalBest(_localBest, OnLocalBestTapped, _provisionalRank);
					_selfHasData = true;
				}
				else if (!string.IsNullOrEmpty(_mapId) && _localBest > 0)
				{
					_selfRow.BindLocalBest(_localBest, OnLocalBestTapped);
					_selfHasData = true;
				}
				UpdateSelfRowVisibility();
			}
		}

		private void OnRowTapped(RacingRecord record)
		{
			RowTapped?.Invoke(record);
		}

		private void OnLocalBestTapped()
		{
			LocalBestTapped?.Invoke();
		}

		public void SetSelectable(bool on)
		{
			_selectable = on;
			ApplySelectable();
		}

		private void ApplySelectable()
		{
			foreach (RecordRowUiElement row in _rows)
			{
				if ((bool)row)
				{
					row.SetSelectable(_selectable);
				}
			}
			if ((bool)_selfRow && _selfHasData)
			{
				_selfRow.SetSelectable(_selectable);
			}
		}

		public void SetSelectedByUserId(string userId)
		{
			_selectedUserId = userId;
			_selectedLocal = false;
			ApplySelectionVisuals();
		}

		public void SetSelectedLocal()
		{
			_selectedUserId = null;
			_selectedLocal = true;
			ApplySelectionVisuals();
		}

		public void ClearSelected()
		{
			_selectedUserId = null;
			_selectedLocal = false;
			ApplySelectionVisuals();
		}

		private void ApplySelectionVisuals()
		{
			foreach (RecordRowUiElement row in _rows)
			{
				if ((bool)row)
				{
					if (row == _provisionalSelfRow)
					{
						row.SetSelected(_selectedLocal);
					}
					else
					{
						row.SetSelected(_selectedUserId != null && row.Record != null && row.Record.UserId == _selectedUserId);
					}
				}
			}
			if ((bool)_selfRow && _selfHasData)
			{
				bool selected = _selectedLocal || (_selectedUserId != null && _selfRecord != null && _selfRecord.UserId == _selectedUserId);
				_selfRow.SetSelected(selected);
			}
		}

		private void OnScroll(Vector2 _)
		{
			UpdateSelfRowVisibility();
		}

		private void UpdateSelfRowVisibility()
		{
			if (!_selfRow)
			{
				return;
			}
			if (!_selfHasData)
			{
				_selfRow.gameObject.SetActive(value: false);
				return;
			}
			bool flag = true;
			RecordRowUiElement recordRowUiElement = (_ownListRow ? _ownListRow : _provisionalSelfRow);
			if ((bool)recordRowUiElement && (bool)_scrollRect && (bool)_scrollRect.viewport)
			{
				flag = !IsVerticallyVisible(recordRowUiElement.GetComponent<RectTransform>(), _scrollRect.viewport);
			}
			if (_selfRow.gameObject.activeSelf != flag)
			{
				_selfRow.gameObject.SetActive(flag);
			}
		}

		private static bool IsVerticallyVisible(RectTransform row, RectTransform viewport)
		{
			if (!row || !viewport)
			{
				return true;
			}
			row.GetWorldCorners(_cornersA);
			viewport.GetWorldCorners(_cornersV);
			float y = _cornersA[0].y;
			float y2 = _cornersA[1].y;
			float y3 = _cornersV[0].y;
			float y4 = _cornersV[1].y;
			if (y2 >= y3)
			{
				return y <= y4;
			}
			return false;
		}

		private async UniTaskVoid RefreshOcclusion()
		{
			if (!_scrollRect)
			{
				return;
			}
			UIHelper.DelayedRebuildLayout(_scrollRect.content).Forget();
			await UniTask.Yield();
			await UniTask.Yield();
			await UniTask.Yield();
			if ((bool)this && (bool)_scrollRect)
			{
				if ((bool)_scrollRectOcclusion)
				{
					_scrollRectOcclusion.RebuildOcclusion().Forget();
				}
				_scrollRect.onValueChanged.RemoveListener(OnScroll);
				_scrollRect.onValueChanged.AddListener(OnScroll);
				UpdateSelfRowVisibility();
			}
		}

		private void ClearRows()
		{
			foreach (RecordRowUiElement row in _rows)
			{
				if ((bool)row)
				{
					UnityEngine.Object.Destroy(row.gameObject);
				}
			}
			_rows.Clear();
			_ownListRow = null;
			_provisionalSelfRow = null;
			_provisionalRank = 0;
		}

		private void ShowEmpty(bool show)
		{
			if ((bool)_emptyState)
			{
				_emptyState.SetActive(show);
			}
		}

		private void CancelLoad()
		{
			if (_cts != null)
			{
				_cts.Cancel();
				_cts.Dispose();
				_cts = null;
			}
		}
	}
}
