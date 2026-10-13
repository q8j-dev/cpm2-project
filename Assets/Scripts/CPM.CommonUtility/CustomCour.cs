using System;
using System.Collections;
using UnityEngine;

public class CustomCour
{
	private Func<IEnumerator> func;

	private Coroutine cour;

	private Coroutine subcour;

	private MonoBehaviour mono;

	public bool IsActive => cour != null;

	public CustomCour(MonoBehaviour mono, Func<IEnumerator> func)
	{
		this.mono = mono;
		this.func = func;
	}

	public void Run()
	{
		Stop();
		try
		{
			if (mono != null && mono.gameObject.activeInHierarchy)
			{
				cour = mono.StartCoroutine(Process());
			}
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}

	private IEnumerator Process()
	{
		subcour = mono.StartCoroutine(func?.Invoke());
		yield return subcour;
		cour = null;
		subcour = null;
	}

	public void Stop()
	{
		try
		{
			if (IsActive)
			{
				mono.StopCoroutine(cour);
				cour = null;
			}
			if (subcour != null)
			{
				mono.StopCoroutine(subcour);
				subcour = null;
			}
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}
}
public class CustomCour<T1>
{
	private readonly Func<T1, IEnumerator> func;

	private Coroutine cour;

	private Coroutine subcour;

	private readonly MonoBehaviour mono;

	public bool IsActive => cour != null;

	public CustomCour(MonoBehaviour mono, Func<T1, IEnumerator> func)
	{
		this.mono = mono;
		this.func = func;
	}

	public void Run(T1 param)
	{
		Stop();
		try
		{
			if (mono != null && mono.gameObject.activeInHierarchy)
			{
				cour = mono.StartCoroutine(Process(param));
			}
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}

	private IEnumerator Process(T1 param)
	{
		subcour = mono.StartCoroutine(func?.Invoke(param));
		yield return subcour;
		cour = null;
		subcour = null;
	}

	public void Stop()
	{
		try
		{
			if (IsActive)
			{
				mono.StopCoroutine(cour);
				cour = null;
			}
			if (subcour != null)
			{
				mono.StopCoroutine(subcour);
				subcour = null;
			}
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}
}
public class CustomCour<T1, T2>
{
	private readonly Func<T1, T2, IEnumerator> func;

	private Coroutine cour;

	private Coroutine subcour;

	private readonly MonoBehaviour mono;

	public bool IsActive => cour != null;

	public CustomCour(MonoBehaviour mono, Func<T1, T2, IEnumerator> func)
	{
		this.mono = mono;
		this.func = func;
	}

	public void Run(T1 param1, T2 param2)
	{
		Stop();
		try
		{
			if (mono != null && mono.gameObject.activeInHierarchy)
			{
				cour = mono.StartCoroutine(Process(param1, param2));
			}
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}

	private IEnumerator Process(T1 param1, T2 param2)
	{
		subcour = mono.StartCoroutine(func?.Invoke(param1, param2));
		yield return subcour;
		cour = null;
		subcour = null;
	}

	public void Stop()
	{
		try
		{
			if (IsActive)
			{
				mono.StopCoroutine(cour);
				cour = null;
			}
			if (subcour != null)
			{
				mono.StopCoroutine(subcour);
				subcour = null;
			}
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}
}
