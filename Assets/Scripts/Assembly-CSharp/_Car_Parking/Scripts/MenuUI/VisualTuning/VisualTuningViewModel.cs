using System;
using System.Reflection;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class VisualTuningViewModel : IVisualTuningViewModel, IVisualTuningViewModelComplexOne, IVisualTuningViewModelComplexParams, IVisualTuningUpdateAction, IInitializable, IDisposable
	{
		private VisualTuning _visualTuning;

		public event Action OnValuesChanged;

		[Inject]
		public void Construct(VisualTuning visualTuning)
		{
			_visualTuning = visualTuning;
		}

		public void Initialize()
		{
			_visualTuning.OnValuesChanged += VisualTuningOnOnValuesChanged;
		}

		public void Dispose()
		{
			_visualTuning.OnValuesChanged -= VisualTuningOnOnValuesChanged;
		}

		private void VisualTuningOnOnValuesChanged()
		{
			OnValuesChanged?.Invoke();
		}

		public void Execute(VisualTuningTypes scopeType, string methodName, object argument = null)
		{
			if (_visualTuning.ScopesByType.ContainsKey(scopeType))
			{
				VisualTuningScope visualTuningScope = _visualTuning.ScopesByType[scopeType];
				MethodInfo method = visualTuningScope.GetType().GetMethod(methodName);
				object[] parameters = ((argument == null) ? null : new object[1] { argument });
				method?.Invoke(visualTuningScope, parameters);
			}
		}

		public VisualTuningScope GetScope(VisualTuningTypes scopeType)
		{
			if (!_visualTuning.ScopesByType.ContainsKey(scopeType))
			{
				return null;
			}
			return _visualTuning.ScopesByType[scopeType];
		}

		public void Execute(VisualTuningTypes scopeType, string methodName, params object[] arguments)
		{
			VisualTuningScope visualTuningScope = _visualTuning.ScopesByType[scopeType];
			Type type = visualTuningScope.GetType();
			Type[] array = new Type[arguments.Length];
			for (int i = 0; i < arguments.Length; i++)
			{
				object obj = arguments[i];
				array[i] = obj.GetType();
			}
			type.GetMethod(methodName, array)?.Invoke(visualTuningScope, arguments);
		}

		public void Execute(string key)
		{
			string[] array = key.Split("_");
			string key2 = array[0];
			string name = array[1];
			VisualTuningScope visualTuningScope = _visualTuning.Scopes[key2];
			MethodInfo method = visualTuningScope.GetType().GetMethod(name);
			string text = array[2];
			if (array.Length == 3)
			{
				if (!int.TryParse(text, out var result))
				{
					method?.Invoke(visualTuningScope, new object[1] { text });
				}
				else
				{
					method?.Invoke(visualTuningScope, new object[1] { result });
				}
			}
			else if (array.Length == 4)
			{
				if (!int.TryParse(text, out var result2))
				{
					Debug.LogError("Error parse: " + text);
				}
				string text2 = array[3];
				if (!int.TryParse(text2, out var result3))
				{
					Debug.LogError("Error parse: " + text2);
				}
				method?.Invoke(visualTuningScope, new object[2] { result2, result3 });
			}
			else
			{
				method?.Invoke(visualTuningScope, null);
			}
		}
	}
}
