using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.ToRoad;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ToRoadPointTrigger : MonoBehaviour, IToRoadHandler
	{
		[Tooltip("Where the player lands. Empty means this trigger's own pivot.")]
		[SerializeField]
		private Transform ReturnPoint;

		[SerializeField]
		[Tooltip("Random side-to-side offset from the point, in metres each way. 0 lands exactly on it.")]
		private float LateralSpread;

		private ArenaFlowController _arenaFlow;

		private ArenaModeRegistry _modeRegistry;

		public ToRoadPriority Priority => ToRoadPriority.Override;

		private Transform Anchor
		{
			get
			{
				if (!ReturnPoint)
				{
					return base.transform;
				}
				return ReturnPoint;
			}
		}

		[Inject]
		public void Construct(ArenaFlowController arenaFlow, ArenaModeRegistry modeRegistry)
		{
			_arenaFlow = arenaFlow;
			_modeRegistry = modeRegistry;
		}

		public bool TryResolve(in ToRoadRequest request, out ToRoadResult result)
		{
			Vector3 position = Anchor.position + Anchor.right * Random.Range(0f - LateralSpread, LateralSpread);
			result = ToRoadResult.Place(position, Anchor.rotation, loadEnvironment: false);
			return true;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (_arenaFlow != null && _modeRegistry != null && !((other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject) != V.DriverPerson) && _modeRegistry.TryGet(_arenaFlow.CurrentMode, out var controller) && controller is IToRoadOverridable toRoadOverridable)
			{
				toRoadOverridable.SetOverride(this);
			}
		}

		bool IToRoadHandler.TryResolve(in ToRoadRequest request, out ToRoadResult result)
		{
			return TryResolve(in request, out result);
		}
	}
}
