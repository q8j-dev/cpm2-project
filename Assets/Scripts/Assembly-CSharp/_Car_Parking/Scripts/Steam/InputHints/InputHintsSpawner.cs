using System.Linq;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	public class InputHintsSpawner : MonoBehaviour
	{
		[SerializeField]
		private InputHintElement InputHintElementPrefab = default;

		[SerializeField]
		private UITarget[] TargetsForHints = default;

		[SerializeField]
		private InputHintsSpawnerConfig Config = default;

		private void Start()
		{
			InstantiateHintsForTargets();
		}

		private void InstantiateHintsForTargets()
		{
			foreach (InputHintData hint in Config.Hints)
			{
				if (!hint.DontSpawn)
				{
					RectTransform rectTransform = FindTarget(hint.ObjectName);
					if ((bool)rectTransform)
					{
						InputHintElement inputHintElement = Object.Instantiate(InputHintElementPrefab);
						inputHintElement.gameObject.SetActive(value: false);
						inputHintElement.transform.SetParent(rectTransform, worldPositionStays: false);
						inputHintElement.transform.localScale = Vector3.one;
						inputHintElement.ApplyData(hint);
						inputHintElement.gameObject.SetActive(value: true);
					}
				}
			}
		}

		private RectTransform FindTarget(string name)
		{
			return (from uiTarget in TargetsForHints
				where uiTarget.name == name
				select uiTarget.target).FirstOrDefault();
		}
	}
}
