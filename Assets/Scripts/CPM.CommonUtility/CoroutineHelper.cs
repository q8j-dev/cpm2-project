using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class CoroutineHelper : MonoBehaviour
{
	public enum Scope
	{
		Local = 0,
		Global = 1
	}

	public Scope scope;

	private void Awake()
	{
		SetScope(scope);
	}

	public void SetScope(Scope scope)
	{
		if (scope == Scope.Global)
		{
			base.transform.SetParent(null);
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
