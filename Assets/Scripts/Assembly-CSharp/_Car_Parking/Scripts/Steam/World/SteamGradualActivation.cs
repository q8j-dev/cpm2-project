using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace _Car_Parking.Scripts.Steam.World
{
	public sealed class SteamGradualActivation
	{
		private const int MaxUnitTransforms = 64;

		private const double SlowUnitMs = 5.0;

		private readonly List<GameObject> _roots = new List<GameObject>();

		private readonly List<GameObject> _units = new List<GameObject>();

		private string _label;

		private bool _rootsOn;

		private int _count;

		public bool Activating { get; private set; }

		public bool IsDone
		{
			get
			{
				if (!Activating)
				{
					if (!_rootsOn)
					{
						return _count == _units.Count;
					}
					return false;
				}
				if (_rootsOn)
				{
					return _count == _units.Count;
				}
				return false;
			}
		}

		public static List<GameObject> CollectUnits(List<GameObject> roots)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject root in roots)
			{
				if ((bool)root)
				{
					CollectUnits(root.transform, list);
				}
			}
			return list;
		}

		public static SteamGradualActivation BeginActivation(List<GameObject> roots, List<GameObject> units, string label)
		{
			SteamGradualActivation steamGradualActivation = Create(roots, units, label, rootsOn: false);
			steamGradualActivation.Activating = true;
			return steamGradualActivation;
		}

		public static SteamGradualActivation BeginDeactivation(List<GameObject> roots, List<GameObject> units, string label)
		{
			SteamGradualActivation steamGradualActivation = Create(roots, units, label, rootsOn: true);
			steamGradualActivation.Activating = false;
			return steamGradualActivation;
		}

		private static SteamGradualActivation Create(List<GameObject> roots, List<GameObject> units, string label, bool rootsOn)
		{
			SteamGradualActivation steamGradualActivation = new SteamGradualActivation
			{
				_label = label,
				_rootsOn = rootsOn
			};
			foreach (GameObject root in roots)
			{
				if ((bool)root)
				{
					steamGradualActivation._roots.Add(root);
				}
			}
			foreach (GameObject unit in units)
			{
				if ((bool)unit && unit.activeSelf)
				{
					steamGradualActivation._units.Add(unit);
				}
			}
			steamGradualActivation._count = steamGradualActivation._units.Count;
			return steamGradualActivation;
		}

		public void Reverse()
		{
			Activating = !Activating;
		}

		public void Step(double budgetMs)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			double num = 0.0;
			while (!IsDone)
			{
				GameObject gameObject = StepOnce(out var heavy);
				double totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
				if (heavy && (bool)gameObject)
				{
					_ = totalMilliseconds - num;
					_ = 5.0;
				}
				num = totalMilliseconds;
				if (!(num < budgetMs))
				{
					break;
				}
			}
		}

		public void Complete()
		{
			_ = IsDone;
			Step(double.MaxValue);
		}

		public void RestoreAfterRootsDisabled()
		{
			_rootsOn = false;
			Activating = false;
			foreach (GameObject unit in _units)
			{
				if ((bool)unit)
				{
					unit.SetActive(value: true);
				}
			}
			_count = _units.Count;
		}

		private GameObject StepOnce(out bool heavy)
		{
			heavy = false;
			if (Activating)
			{
				if (!_rootsOn)
				{
					if (_count > 0)
					{
						return SetUnit(--_count, active: false);
					}
					SetRoots(active: true);
					return null;
				}
				heavy = true;
				return SetUnit(_count++, active: true);
			}
			if (_rootsOn)
			{
				if (_count > 0)
				{
					heavy = true;
					return SetUnit(--_count, active: false);
				}
				SetRoots(active: false);
				return null;
			}
			return SetUnit(_count++, active: true);
		}

		private GameObject SetUnit(int index, bool active)
		{
			GameObject gameObject = _units[index];
			if ((bool)gameObject)
			{
				gameObject.SetActive(active);
			}
			return gameObject;
		}

		private void SetRoots(bool active)
		{
			foreach (GameObject root in _roots)
			{
				if ((bool)root)
				{
					root.SetActive(active);
				}
			}
			_rootsOn = active;
			_count = 0;
		}

		private static void CollectUnits(Transform parent, List<GameObject> units)
		{
			for (int i = 0; i < parent.childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					if (child.gameObject.GetComponentCount() == 1 && child.childCount > 0 && CountTransforms(child, 65) > 64)
					{
						CollectUnits(child, units);
					}
					else
					{
						units.Add(child.gameObject);
					}
				}
			}
		}

		private static int CountTransforms(Transform root, int cap)
		{
			int num = 1;
			for (int i = 0; i < root.childCount; i++)
			{
				if (num >= cap)
				{
					break;
				}
				num += CountTransforms(root.GetChild(i), cap - num);
			}
			return num;
		}

		private static string PathOf(Transform transform)
		{
			string text = transform.name;
			Transform parent = transform.parent;
			while ((bool)parent)
			{
				text = parent.name + "/" + text;
				parent = parent.parent;
			}
			return text;
		}
	}
}
