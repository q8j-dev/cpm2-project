using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Arena.UI;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceSpectatorDriver : IDisposable
	{
		private const float NoRunnersNotificationSeconds = 3f;

		private readonly IArenaContext _ctx;

		private readonly LobbyRoster _roster;

		private readonly OnlineCollectionsProvider _online;

		private readonly ArenaSpectatorCamera _camera;

		private readonly ObstacleRaceSpectatorPanel _panel;

		private readonly Func<SurvivalEliminationBoard> _readBoard;

		private readonly Action _restoreCamera;

		private readonly ObstacleRaceStandingsDriver _standings;

		private readonly SpectatorTargetList _targets = new SpectatorTargetList();

		private readonly List<int> _candidateActors = new List<int>();

		private readonly Dictionary<int, Transform> _resolvedTargets = new Dictionary<int, Transform>();

		private readonly List<SpectatorListRow> _listRows = new List<SpectatorListRow>();

		private readonly List<int> _standingsOrder = new List<int>();

		private ObstacleRaceStandingsUpdate _latest;

		private bool _watching;

		private int _cameraActor;

		public ObstacleRaceSpectatorDriver(IArenaContext ctx, LobbyRoster roster, OnlineCollectionsProvider online, ArenaSpectatorCamera camera, ObstacleRaceSpectatorPanel panel, Func<SurvivalEliminationBoard> readBoard, Action restoreCamera, ObstacleRaceStandingsDriver standings)
		{
			_ctx = ctx;
			_roster = roster;
			_online = online;
			_camera = camera;
			_panel = panel;
			_readBoard = readBoard;
			_restoreCamera = restoreCamera;
			_standings = standings;
			if ((bool)_panel)
			{
				_panel.WatchRequested += StartWatching;
				_panel.BackRequested += StopWatching;
				_panel.NextRequested += Next;
				_panel.PrevRequested += Prev;
				_panel.RowClicked += OnRowClicked;
			}
			if ((bool)_camera && (bool)_panel)
			{
				_camera.BindInput(_panel.EventRegistry, _panel.ControlsSettings);
			}
			if (_standings != null)
			{
				_standings.Updated += OnStandingsUpdated;
				_latest = _standings.Latest;
			}
			PersonMultiplayerDataSending.OnPersonDataRemoved += OnPersonRemoved;
			Refresh();
			StartWatching();
		}

		public void OnStateChanged()
		{
			Refresh();
		}

		public void Dispose()
		{
			PersonMultiplayerDataSending.OnPersonDataRemoved -= OnPersonRemoved;
			if ((bool)_panel)
			{
				_panel.WatchRequested -= StartWatching;
				_panel.BackRequested -= StopWatching;
				_panel.NextRequested -= Next;
				_panel.PrevRequested -= Prev;
				_panel.RowClicked -= OnRowClicked;
			}
			if (_standings != null)
			{
				_standings.Updated -= OnStandingsUpdated;
			}
			StopWatching();
		}

		private void Refresh()
		{
			BuildCandidates();
			_targets.Refresh(_candidateActors, _readBoard?.Invoke());
			ApplyStandingsOrder();
			if ((bool)_panel)
			{
				_panel.SetWatchAvailable(_targets.HasTarget);
			}
			RedrawPanel();
			if (_watching)
			{
				if (!_targets.HasTarget)
				{
					NotifyNoRunnersLeft();
					StopWatching();
				}
				else
				{
					PointCameraAtCurrent();
				}
			}
		}

		private void StartWatching()
		{
			if (!_watching && _targets.HasTarget)
			{
				_watching = true;
				if ((bool)_panel)
				{
					_panel.SetWatching(watching: true);
				}
				PointCameraAtCurrent();
			}
		}

		private void StopWatching()
		{
			if (_watching)
			{
				_watching = false;
				if ((bool)_camera)
				{
					_camera.Detach();
				}
				_cameraActor = 0;
				_restoreCamera?.Invoke();
				if ((bool)_panel)
				{
					_panel.SetWatching(watching: false);
				}
			}
		}

		private void Next()
		{
			if (_targets.Next())
			{
				PointCameraAtCurrent();
			}
		}

		private void Prev()
		{
			if (_targets.Prev())
			{
				PointCameraAtCurrent();
			}
		}

		private void PointCameraAtCurrent()
		{
			int current = _targets.Current;
			if ((bool)_camera && current != _cameraActor && _resolvedTargets.TryGetValue(current, out var value) && (bool)value)
			{
				if (_camera.HasTarget)
				{
					_camera.SetTarget(value);
				}
				else
				{
					_camera.Attach(value);
				}
				_cameraActor = current;
			}
			if ((bool)_panel)
			{
				_panel.SetTarget(_roster.GetDisplayName(_targets.Current), _targets.PositionOfCurrent, _targets.Count);
			}
			RedrawPanel();
		}

		private void OnStandingsUpdated(ObstacleRaceStandingsUpdate update)
		{
			_latest = update;
			ApplyStandingsOrder();
			if (_watching)
			{
				PointCameraAtCurrent();
			}
			else
			{
				RedrawPanel();
			}
		}

		private void ApplyStandingsOrder()
		{
			IReadOnlyList<ObstacleRaceStandingRow> rows = _latest.Rows;
			if (rows != null)
			{
				_standingsOrder.Clear();
				for (int i = 0; i < rows.Count; i++)
				{
					_standingsOrder.Add(rows[i].Actor);
				}
				_targets.Reorder(_standingsOrder);
			}
		}

		private void RedrawPanel()
		{
			if (!_panel)
			{
				return;
			}
			IReadOnlyList<ObstacleRaceStandingRow> rows = _latest.Rows;
			if (rows == null)
			{
				_panel.HideStandings();
				return;
			}
			_listRows.Clear();
			SpectatorListRow? local = null;
			for (int i = 0; i < rows.Count; i++)
			{
				ObstacleRaceStandingRow row = rows[i];
				SpectatorListRow spectatorListRow = new SpectatorListRow(row.Actor, row.Place, row.UserName, ValueTextOf(in row), _targets.Contains(row.Actor), row.Actor == _targets.Current);
				if (row.IsLocal)
				{
					local = spectatorListRow;
				}
				else
				{
					_listRows.Add(spectatorListRow);
				}
			}
			_panel.SetStandings(_listRows, local, rows.Count);
		}

		private static string ValueTextOf(in ObstacleRaceStandingRow row)
		{
			if (!row.IsRunning)
			{
				return ObstacleRaceStandingsFormat.Elapsed(row.FinishTime);
			}
			return ObstacleRaceStandingsFormat.Remaining(row.RemainingMeters);
		}

		private void OnRowClicked(int actor)
		{
			if (_targets.TrySelect(actor))
			{
				if (_watching)
				{
					PointCameraAtCurrent();
				}
				else
				{
					StartWatching();
				}
			}
		}

		private void OnPersonRemoved(MultiPersonControl person)
		{
			Refresh();
		}

		private void BuildCandidates()
		{
			_candidateActors.Clear();
			_resolvedTargets.Clear();
			IReadOnlyList<int> readOnlyList = _ctx?.ParticipantActorNumbers;
			if (readOnlyList == null)
			{
				return;
			}
			for (int i = 0; i < readOnlyList.Count; i++)
			{
				int num = readOnlyList[i];
				if (IsInRoom(num))
				{
					Transform transform = ResolveTarget(num);
					if ((bool)transform)
					{
						_candidateActors.Add(num);
						_resolvedTargets[num] = transform;
					}
				}
			}
		}

		private static bool IsInRoom(int actor)
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			if (currentRoom?.Players != null)
			{
				return currentRoom.Players.ContainsKey(actor);
			}
			return false;
		}

		private Transform ResolveTarget(int actor)
		{
			if (actor == 0 || _online == null)
			{
				return null;
			}
			MultiPersonControl playerByOwnerActorNr = _online.GetPlayerByOwnerActorNr(actor);
			if (!playerByOwnerActorNr)
			{
				return null;
			}
			return playerByOwnerActorNr.transform;
		}

		private void NotifyNoRunnersLeft()
		{
			ManagerUIHelper.SetMessage(new LocalizedString("ObstacleRace.NoRunnersLeft"), 3f);
		}
	}
}
