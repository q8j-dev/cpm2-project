using UnityEngine;

namespace Traffic
{
	[RequireComponent(typeof(Renderer))]
	public class DeactivatePerson : MonoBehaviour
	{
		private Behaviours _personBehaviours;

		private Renderer _renderer;

		public bool IsVisible => _renderer.isVisible;

		private void Awake()
		{
			_personBehaviours = GetComponentInParent<Behaviours>();
			_renderer = GetComponent<Renderer>();
			if (_personBehaviours == null)
			{
				Object.Destroy(this);
			}
		}

		private void OnBecameInvisible()
		{
			PeopleTraffic instance = PeopleTraffic.Instance;
			if (!(instance == null) && instance.visibleAgents != null && !(_personBehaviours == null))
			{
				instance.visibleAgents.Remove(_personBehaviours);
				_personBehaviours.DeactivateBehaviours();
			}
		}

		private void OnBecameVisible()
		{
			PeopleTraffic instance = PeopleTraffic.Instance;
			if (!(instance == null) && instance.visibleAgents != null && !(_personBehaviours == null))
			{
				instance.visibleAgents.Add(_personBehaviours);
				_personBehaviours.ActivateBehaviours();
			}
		}
	}
}
