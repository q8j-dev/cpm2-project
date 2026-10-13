using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.GameFacts.Consumers
{
	public class GameFactVisibility : MonoBehaviour
	{
		[SerializeField]
		private ConditionState State;

		[Tooltip("Graphics to toggle (Image / TMP_Text / Button / GraphicRaycaster / Canvas …). List every graphic of the element, or a Canvas to cover a whole subtree.")]
		[SerializeField]
		private Behaviour[] Toggle;

		[SerializeField]
		private GameObject[] ToggleGO;

		private void Start()
		{
			StartAsync(base.destroyCancellationToken).Forget();
		}

		private async UniTask StartAsync(CancellationToken ct)
		{
			if ((bool)State)
			{
				State.OnStatusChanged += OnStateStatusChanged;
			}
			await UniTask.Yield(PlayerLoopTiming.LastPreLateUpdate, ct);
			Apply();
		}

		private void OnDestroy()
		{
			if ((bool)State)
			{
				State.OnStatusChanged -= OnStateStatusChanged;
			}
		}

		private void OnStateStatusChanged(bool met)
		{
			Apply();
		}

		private void Apply()
		{
			if (!State)
			{
				return;
			}
			bool current = State.Current;
			Behaviour[] toggle = Toggle;
			foreach (Behaviour behaviour in toggle)
			{
				if ((bool)behaviour)
				{
					behaviour.enabled = current;
				}
			}
			GameObject[] toggleGO = ToggleGO;
			foreach (GameObject gameObject in toggleGO)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(current);
				}
			}
		}
	}
}
