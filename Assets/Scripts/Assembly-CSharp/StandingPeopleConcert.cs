using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking;

public class StandingPeopleConcert : MonoBehaviour
{
	public enum TestEnum
	{
		Rectangle = 0,
		Circle = 1
	}

	[HideInInspector]
	public GameObject planePrefab;

	[HideInInspector]
	public GameObject circlePrefab;

	[HideInInspector]
	public GameObject surface;

	[HideInInspector]
	public Vector2 planeSize = new Vector2(1f, 1f);

	public GameObject[] peoplePrefabs = new GameObject[0];

	[HideInInspector]
	private List<Vector3> spawnPoints = new List<Vector3>();

	[HideInInspector]
	public GameObject target;

	[HideInInspector]
	public int peopleCount;

	[HideInInspector]
	public bool isCircle;

	[HideInInspector]
	public float circleDiametr = 1f;

	[HideInInspector]
	public bool showSurface = true;

	public TestEnum SurfaceType;

	[HideInInspector]
	public GameObject par;

	public void OnDrawGizmos()
	{
		if (!isCircle)
		{
			surface.transform.localScale = new Vector3(planeSize.x, planeSize.y, 1f);
		}
		else
		{
			surface.transform.localScale = new Vector3(circleDiametr, circleDiametr, 1f);
		}
	}

	public void SpawnRectangleSurface()
	{
		if (surface != null)
		{
			UnityEngine.Object.DestroyImmediate(surface);
		}
		GameObject gameObject = (surface = UnityEngine.Object.Instantiate(planePrefab, base.transform.position, Quaternion.identity));
		isCircle = false;
		gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x - 90f, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
		gameObject.transform.position += new Vector3(0f, 0.01f, 0f);
		gameObject.transform.parent = base.transform;
		gameObject.name = "surface";
	}

	public void SpawnCircleSurface()
	{
		if (surface != null)
		{
			UnityEngine.Object.DestroyImmediate(surface);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(circlePrefab, base.transform.position, Quaternion.identity);
		isCircle = true;
		gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x - 90f, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
		gameObject.transform.position += new Vector3(0f, 0.01f, 0f);
		gameObject.transform.parent = base.transform;
		gameObject.name = "surface";
		surface = gameObject;
	}

	public void RemoveButton()
	{
		if (par != null)
		{
			UnityEngine.Object.DestroyImmediate(par);
		}
	}

	public void PopulateButton()
	{
		RemoveButton();
		GameObject gameObject = (par = new GameObject());
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.name = "people";
		spawnPoints.Clear();
		SpawnPeople(peopleCount);
	}

	private void UpdatePeoplePrefabs()
	{
		int num = 0;
		GameObject[] array = peoplePrefabs;
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i])
			{
				num++;
			}
		}
		GameObject[] array2 = new GameObject[num];
		num = 0;
		array = peoplePrefabs;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				array2[num] = gameObject;
				num++;
			}
		}
		peoplePrefabs = array2;
	}

	private void SpawnPeople(int _peopleCount)
	{
		for (int i = 0; i < _peopleCount; i++)
		{
			UpdatePeoplePrefabs();
			int num = UnityEngine.Random.Range(0, 13);
			Vector3 vector = (isCircle ? RandomCirclePosition() : RandomRectanglePosition());
			if (vector != Vector3.zero)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("persons/person" + num, typeof(GameObject)), vector, Quaternion.identity) as GameObject;
				if (Physics.Raycast(gameObject.transform.position + new Vector3(0f, 1f, 0f), -gameObject.transform.up, out var _, 100f, -1, QueryTriggerInteraction.Ignore))
				{
					float y = gameObject.transform.position.y;
					gameObject.transform.position = new Vector3(gameObject.transform.position.x, y, gameObject.transform.position.z);
				}
				if (FlagsStorage.IsFreeDriveMode)
				{
					gameObject.GetComponent<Collider>().enabled = false;
				}
				gameObject.AddComponent<PeopleController>();
				Vector3 position = gameObject.transform.position;
				if (Mathf.Abs(position.y) > 10f)
				{
					gameObject.transform.position = position;
				}
				spawnPoints.Add(gameObject.transform.position);
				if (target != null)
				{
					gameObject.GetComponent<PeopleController>().SetTarget(target.transform.position);
				}
				else
				{
					gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.rotation.x, base.transform.rotation.y, gameObject.transform.rotation.z);
				}
				gameObject.GetComponent<PeopleController>().animNames = new string[4] { "idle1", "idle2", "cheer", "claphands" };
				gameObject.transform.parent = par.transform;
			}
		}
	}

	private Vector3 RandomRectanglePosition()
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < 10; i++)
		{
			vector.x = surface.transform.position.x - GetRealPlaneSize().x / 2f + 0.3f + UnityEngine.Random.Range(0f, GetRealPlaneSize().x - 0.6f);
			vector.z = surface.transform.position.z - GetRealPlaneSize().y / 2f + 0.3f + UnityEngine.Random.Range(0f, GetRealPlaneSize().y - 0.6f);
			vector.y = surface.transform.position.y;
			if (IsRandomPositionFree(vector))
			{
				return vector;
			}
		}
		return Vector3.zero;
	}

	private Vector3 RandomCirclePosition()
	{
		Vector3 position = surface.transform.position;
		float num = GetRealPlaneSize().x / 2f;
		Vector3 vector = default;
		for (int i = 0; i < 10; i++)
		{
			float num2 = UnityEngine.Random.value * num;
			float num3 = UnityEngine.Random.value * 360f;
			vector.x = position.x + num2 * Mathf.Sin(num3 * ((float)Math.PI / 180f));
			vector.y = position.y;
			vector.z = position.z + num2 * Mathf.Cos(num3 * ((float)Math.PI / 180f));
			if (IsRandomPositionFree(vector))
			{
				return vector;
			}
		}
		return Vector3.zero;
	}

	private bool IsRandomPositionFree(Vector3 pos)
	{
		for (int i = 0; i < spawnPoints.Count; i++)
		{
			if (spawnPoints[i].x - 0.6f < pos.x && spawnPoints[i].x + 1f > pos.x && spawnPoints[i].z - 0.5f < pos.z && spawnPoints[i].z + 0.6f > pos.z)
			{
				return false;
			}
		}
		return true;
	}

	private Vector2 GetRealPlaneSize()
	{
		Vector3 size = surface.GetComponent<MeshRenderer>().bounds.size;
		return new Vector2(size.x, size.z);
	}

	private Vector2 GetRealPeopleModelSize()
	{
		Vector3 size = peoplePrefabs[1].GetComponent<MeshRenderer>().bounds.size;
		return new Vector2(size.x, size.z);
	}
}
