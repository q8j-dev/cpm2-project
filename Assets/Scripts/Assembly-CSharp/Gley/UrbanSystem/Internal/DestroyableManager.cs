using System.Collections.Generic;

namespace Gley.UrbanSystem.Internal
{
	public class DestroyableManager
	{
		private static DestroyableManager _instance;

		private List<IDestroyable> _destroyables = new List<IDestroyable>();

		public static DestroyableManager Instance => _instance ?? (_instance = new DestroyableManager());

		public void Register(IDestroyable destroyable)
		{
			_destroyables.Add(destroyable);
		}

		public void Unregister(IDestroyable destroyable)
		{
			_destroyables.Remove(destroyable);
		}

		public void DestroyAll()
		{
			foreach (IDestroyable destroyable in _destroyables)
			{
				destroyable.OnDestroy();
			}
			_destroyables = new List<IDestroyable>();
		}
	}
}
