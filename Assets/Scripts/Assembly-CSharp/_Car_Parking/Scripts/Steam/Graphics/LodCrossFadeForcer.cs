using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Steam.Perf;

namespace _Car_Parking.Scripts.Steam.Graphics
{
	public class LodCrossFadeForcer : MonoBehaviour
	{
		private readonly List<LODGroup> _forced = new List<LODGroup>();

		private static bool CrossFadeEnabled => !SteamPerfFlags.IsOn("noCrossFade");

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap()
		{
			GameObject obj = new GameObject("[LodCrossFadeForcer]");
			Object.DontDestroyOnLoad(obj);
			obj.AddComponent<LodCrossFadeForcer>();
		}

		private void Awake()
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				ApplyToScene(SceneManager.GetSceneAt(i));
			}
			SceneManager.sceneLoaded += OnSceneLoaded;
			SteamPerfFlags.Changed += OnFlagsChanged;
		}

		private void OnDestroy()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			SteamPerfFlags.Changed -= OnFlagsChanged;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			ApplyToScene(scene);
		}

		private void ApplyToScene(Scene scene)
		{
			if (!scene.isLoaded)
			{
				return;
			}
			_forced.RemoveAll((LODGroup group) => !group);
			bool crossFadeEnabled = CrossFadeEnabled;
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int num = 0; num < rootGameObjects.Length; num++)
			{
				LODGroup[] componentsInChildren = rootGameObjects[num].GetComponentsInChildren<LODGroup>(includeInactive: true);
				foreach (LODGroup lODGroup in componentsInChildren)
				{
					if (lODGroup.fadeMode != LODFadeMode.CrossFade)
					{
						_forced.Add(lODGroup);
						SetCrossFade(lODGroup, crossFadeEnabled);
					}
				}
			}
		}

		private void OnFlagsChanged()
		{
			_forced.RemoveAll((LODGroup group) => !group);
			bool crossFadeEnabled = CrossFadeEnabled;
			for (int num = 0; num < _forced.Count; num++)
			{
				SetCrossFade(_forced[num], crossFadeEnabled);
			}
		}

		private static void SetCrossFade(LODGroup group, bool crossFade)
		{
			group.fadeMode = (crossFade ? LODFadeMode.CrossFade : LODFadeMode.None);
			group.animateCrossFading = crossFade;
		}
	}
}
