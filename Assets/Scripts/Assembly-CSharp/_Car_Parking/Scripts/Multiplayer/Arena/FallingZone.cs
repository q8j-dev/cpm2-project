using System;
using System.Collections.Generic;
using CPM.Obstacles;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public class FallingZone : Obstacle
	{
		[SerializeField]
		private HexGridSpawner HexGridSpawnerPrefab = default;

		[SerializeField]
		private Transform GridRoot = default;

		private FallingZoneConfig _config;

		private readonly List<FallingTile> _tiles = new List<FallingTile>();

		private readonly List<FallingTile> _fallingTiles = new List<FallingTile>();

		public override bool UsesSmoothedClock => false;

		public event Action<int> OnTileStepped;

		public override void EvaluateAt(double elapsedSeconds)
		{
			for (int num = _fallingTiles.Count - 1; num >= 0; num--)
			{
				FallingTile fallingTile = _fallingTiles[num];
				if (!fallingTile || fallingTile.IsDestroyed)
				{
					_fallingTiles.RemoveAt(num);
				}
				else
				{
					fallingTile.EvaluateAt(elapsedSeconds);
					if (fallingTile.IsDestroyed)
					{
						_fallingTiles.RemoveAt(num);
					}
				}
			}
		}

		public void Init(FallingZoneConfig config)
		{
			_config = config;
			int num = 0;
			for (int i = 0; i < config.Count; i++)
			{
				HexGridSpawner hexGridSpawner = UnityEngine.Object.Instantiate(HexGridSpawnerPrefab, GridRoot.position + Vector3.down * config.OffsetSpawn * i, GridRoot.rotation, GridRoot);
				HexGridSettings hexGridSettings = hexGridSpawner.HexGridSettings;
				hexGridSettings.Radius = config.Radius;
				foreach (GameObject item in hexGridSpawner.Spawn(hexGridSettings))
				{
					if (item.TryGetComponent<FallingTile>(out var component))
					{
						component.OnDriverPersonTriggered += OnFallingTileDriverPersonTriggered;
						component.Init(num++);
						_tiles.Add(component);
					}
				}
			}
		}

		public void StartTileFall(int id, double startTime)
		{
			if (_config != null && _config.TileDestroy != null && id >= 0 && id < _tiles.Count)
			{
				FallingTile fallingTile = _tiles[id];
				if ((bool)fallingTile && fallingTile.Id == id && !fallingTile.IsFalling)
				{
					fallingTile.StartFalling(startTime, _config.TileDestroy);
					_fallingTiles.Add(fallingTile);
				}
			}
		}

		private void OnDestroy()
		{
			for (int i = 0; i < _tiles.Count; i++)
			{
				FallingTile fallingTile = _tiles[i];
				if ((bool)fallingTile)
				{
					fallingTile.OnDriverPersonTriggered -= OnFallingTileDriverPersonTriggered;
				}
			}
		}

		private void OnFallingTileDriverPersonTriggered(FallingTile fallingTile)
		{
			if ((bool)fallingTile && !fallingTile.IsFalling)
			{
				OnTileStepped?.Invoke(fallingTile.Id);
			}
		}
	}
}
