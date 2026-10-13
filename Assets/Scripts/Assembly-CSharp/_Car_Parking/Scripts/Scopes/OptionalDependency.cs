using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Scopes
{
	public sealed class OptionalDependency<T> where T : class
	{
		private readonly IObjectResolver _resolver;

		private bool _resolved;

		private T _value;

		public T Value
		{
			get
			{
				if (!_resolved)
				{
					_resolver.TryResolve<T>(out _value);
					_resolved = true;
				}
				if (_value is Object obj && obj == null)
				{
					return null;
				}
				return _value;
			}
		}

		public bool HasValue => Value != null;

		public OptionalDependency(IObjectResolver resolver)
		{
			_resolver = resolver;
		}
	}
}
