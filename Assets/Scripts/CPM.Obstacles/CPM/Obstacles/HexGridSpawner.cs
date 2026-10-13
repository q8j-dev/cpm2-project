using System.Collections.Generic;
using UnityEngine;

namespace CPM.Obstacles
{
	public sealed class HexGridSpawner : MonoBehaviour
	{
		private const float Sqrt3 = 1.7320508f;

		[Tooltip("The tile to clone. Its own rotation and scale are kept as authored.")]
		[SerializeField]
		private GameObject TilePrefab;

		[SerializeField]
		[Tooltip("Parent for the spawned tiles. Empty means this transform.")]
		private Transform Root;

		[SerializeField]
		private HexGridSettings Settings = HexGridSettings.Default;

		[SerializeField]
		private bool SpawnOnStart;

		private readonly List<GameObject> _tiles = new List<GameObject>();

		public HexGridSettings HexGridSettings => Settings;

		public IReadOnlyList<GameObject> Tiles => _tiles;

		private void Start()
		{
			if (SpawnOnStart)
			{
				Spawn();
			}
		}

		public IReadOnlyList<GameObject> Spawn()
		{
			return Spawn(Settings);
		}

		public IReadOnlyList<GameObject> Spawn(HexGridSettings settings)
		{
			Clear();
			if (!TilePrefab)
			{
				Debug.LogError("[HexGridSpawner] " + base.name + ": TilePrefab must be assigned.", this);
				return _tiles;
			}
			Transform parent = (Root ? Root : base.transform);
			foreach (Vector2Int item in Coordinates(settings))
			{
				GameObject gameObject = CreateTile(parent);
				gameObject.transform.localPosition = ToLocalPosition(item.x, item.y, settings);
				gameObject.name = $"Hex_{item.x}_{item.y}";
				_tiles.Add(gameObject);
			}
			return _tiles;
		}

		public void Clear()
		{
			foreach (GameObject tile in _tiles)
			{
				if ((bool)tile)
				{
					if (Application.isPlaying)
					{
						Object.Destroy(tile);
					}
					else
					{
						Object.DestroyImmediate(tile);
					}
				}
			}
			_tiles.Clear();
		}

		private GameObject CreateTile(Transform parent)
		{
			return Object.Instantiate(TilePrefab, parent);
		}

		public static IEnumerable<Vector2Int> Coordinates(HexGridSettings settings)
		{
			if (settings.Shape == HexShape.Hexagon)
			{
				int radius = Mathf.Max(0, settings.Radius);
				for (int q = -radius; q <= radius; q++)
				{
					int num = Mathf.Max(-radius, -q - radius);
					int max = Mathf.Min(radius, -q + radius);
					for (int r = num; r <= max; r++)
					{
						yield return new Vector2Int(q, r);
					}
				}
				yield break;
			}
			int columns = Mathf.Max(1, settings.Columns);
			int rows = Mathf.Max(1, settings.Rows);
			for (int radius = 0; radius < columns; radius++)
			{
				for (int q = 0; q < rows; q++)
				{
					yield return (settings.Orientation == HexOrientation.FlatTop) ? new Vector2Int(radius, q - (radius - (radius & 1)) / 2) : new Vector2Int(radius - (q - (q & 1)) / 2, q);
				}
			}
		}

		public static Vector3 ToLocalPosition(int q, int r, HexGridSettings settings)
		{
			float num = settings.TileSize * settings.Spacing;
			if (settings.Orientation != HexOrientation.FlatTop)
			{
				return new Vector3(num * 1.7320508f * ((float)q + (float)r * 0.5f), 0f, num * 1.5f * (float)r);
			}
			return new Vector3(num * 1.5f * (float)q, 0f, num * 1.7320508f * ((float)r + (float)q * 0.5f));
		}
	}
}
