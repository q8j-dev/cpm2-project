using System;
using System.Collections.Generic;
using UnityEngine;

public class StandingPeopleStreet : MonoBehaviour
{
	public enum TestEnum
	{
		Rectangle = 0,
		Circle = 1
	}

	public const int MaxPersonCount = 13;

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
	public List<Vector3> spawnPoints = new List<Vector3>();

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

	public GameObject[] persons;

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
		par = null;
	}

	public void PopulateButton()
	{
		RemoveButton();
		GameObject gameObject = (par = new GameObject());
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.name = "people";
		spawnPoints.Clear();
		SpawnPeople(peopleCount);
		int childCount = par.transform.childCount;
		persons = new GameObject[childCount];
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
		int num = UnityEngine.Random.Range(0, _peopleCount / 3) * 3;
		int num2 = UnityEngine.Random.Range(0, (_peopleCount - num) / 2) * 2;
		int num3 = _peopleCount - num - num2;
		for (int i = 0; i < num3; i++)
		{
			int num4 = UnityEngine.Random.Range(0, 13);
			Vector3 vector = (isCircle ? RandomCirclePosition() : RandomRectanglePosition());
			if (vector != Vector3.zero)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("persons/person" + num4, typeof(GameObject))) as GameObject;
				if (Physics.Raycast(gameObject.transform.position + new Vector3(0f, 1f, 0f), -gameObject.transform.up, out var hitInfo))
				{
					gameObject.transform.position = new Vector3(gameObject.transform.position.x, hitInfo.point.y, gameObject.transform.position.z);
				}
				gameObject.AddComponent<PeopleController>();
				spawnPoints.Add(gameObject.transform.position);
				gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.rotation.x, UnityEngine.Random.Range(1, 359), gameObject.transform.rotation.z);
				gameObject.GetComponent<PeopleController>().animNames = new string[2] { "idle1", "idle2" };
				gameObject.transform.parent = par.transform;
			}
		}
		for (int j = 0; j < num2 / 2; j++)
		{
			Vector3 vector2 = (isCircle ? RandomCirclePosition() : RandomRectanglePosition());
			if (!(vector2 != Vector3.zero))
			{
				continue;
			}
			Vector3 vector3 = Vector3.zero;
			Vector3 vector4 = Vector3.zero;
			for (int k = 0; k < 100; k++)
			{
				for (int l = 0; l < 10; l++)
				{
					vector3 = vector2 + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0f, UnityEngine.Random.Range(-0.5f, 0.5f));
					if (IsRandomPositionFree(vector3, Vector3.zero, Vector3.zero))
					{
						break;
					}
					vector3 = Vector3.zero;
				}
				for (int m = 0; m < 10; m++)
				{
					vector4 = vector2 + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0f, UnityEngine.Random.Range(-0.5f, 0.5f));
					if (IsRandomPositionFree(vector4, vector3, Vector3.zero))
					{
						break;
					}
					vector4 = Vector3.zero;
				}
				if (vector3 != Vector3.zero && vector4 != Vector3.zero)
				{
					spawnPoints.Add(vector3);
					spawnPoints.Add(vector4);
					break;
				}
				vector3 = Vector3.zero;
				vector4 = Vector3.zero;
			}
			if (vector3 != Vector3.zero && vector4 != Vector3.zero)
			{
				int num5 = UnityEngine.Random.Range(0, peoplePrefabs.Length);
				GameObject gameObject2 = base.gameObject;
				gameObject2 = UnityEngine.Object.Instantiate(peoplePrefabs[num5], vector3, Quaternion.identity);
				if (Physics.Raycast(gameObject2.transform.position + new Vector3(0f, 1f, 0f), -gameObject2.transform.up, out var hitInfo2))
				{
					gameObject2.transform.position = new Vector3(gameObject2.transform.position.x, hitInfo2.point.y, gameObject2.transform.position.z);
				}
				gameObject2.AddComponent<PeopleController>();
				gameObject2.GetComponent<PeopleController>().animNames = new string[3] { "talk1", "talk2", "listen" };
				gameObject2.transform.parent = par.transform;
				num5 = UnityEngine.Random.Range(0, peoplePrefabs.Length);
				GameObject gameObject3 = base.gameObject;
				gameObject3 = UnityEngine.Object.Instantiate(peoplePrefabs[num5], vector4, Quaternion.identity);
				if (Physics.Raycast(gameObject3.transform.position + new Vector3(0f, 1f, 0f), -gameObject3.transform.up, out var hitInfo3))
				{
					gameObject3.transform.position = new Vector3(gameObject3.transform.position.x, hitInfo3.point.y, gameObject3.transform.position.z);
				}
				gameObject3.AddComponent<PeopleController>();
				gameObject3.GetComponent<PeopleController>().animNames = new string[3] { "talk1", "talk2", "listen" };
				gameObject3.transform.parent = par.transform;
				gameObject3.GetComponent<PeopleController>().SetTarget(gameObject2.transform.position);
				gameObject2.GetComponent<PeopleController>().SetTarget(gameObject3.transform.position);
			}
		}
		for (int n = 0; n < num / 3; n++)
		{
			Vector3 vector5 = (isCircle ? RandomCirclePosition() : RandomRectanglePosition());
			if (!(vector5 != Vector3.zero))
			{
				continue;
			}
			int num6 = UnityEngine.Random.Range(0, peoplePrefabs.Length);
			Vector3 vector6 = Vector3.zero;
			Vector3 vector7 = Vector3.zero;
			Vector3 vector8 = Vector3.zero;
			for (int num7 = 0; num7 < 100; num7++)
			{
				for (int num8 = 0; num8 < 10; num8++)
				{
					vector6 = vector5 + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0f, UnityEngine.Random.Range(-0.5f, 0.5f));
					if (IsRandomPositionFree(vector6, Vector3.zero, Vector3.zero))
					{
						break;
					}
					vector6 = Vector3.zero;
				}
				for (int num9 = 0; num9 < 10; num9++)
				{
					if (vector6 != Vector3.zero)
					{
						vector7 = vector5 + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0f, UnityEngine.Random.Range(-0.5f, 0.5f));
						if (IsRandomPositionFree(vector7, vector6, Vector3.zero))
						{
							break;
						}
						vector7 = Vector3.zero;
					}
					else
					{
						vector7 = Vector3.zero;
					}
				}
				for (int num10 = 0; num10 < 10; num10++)
				{
					if (vector7 != Vector3.zero && vector6 != Vector3.zero)
					{
						vector8 = vector5 + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0f, UnityEngine.Random.Range(-0.5f, 0.5f));
						if (IsRandomPositionFree(vector8, vector6, vector7))
						{
							break;
						}
						vector8 = Vector3.zero;
					}
					else
					{
						vector8 = Vector3.zero;
					}
				}
				if (vector6 != Vector3.zero && vector7 != Vector3.zero && vector8 != Vector3.zero)
				{
					spawnPoints.Add(vector6);
					spawnPoints.Add(vector7);
					spawnPoints.Add(vector8);
					break;
				}
				vector6 = Vector3.zero;
				vector7 = Vector3.zero;
				vector8 = Vector3.zero;
			}
			if (!(vector6 != Vector3.zero))
			{
				continue;
			}
			if (vector6 != Vector3.zero)
			{
				GameObject gameObject4 = UnityEngine.Object.Instantiate(peoplePrefabs[num6], vector6, Quaternion.identity);
				if (Physics.Raycast(gameObject4.transform.position + new Vector3(0f, 1f, 0f), -gameObject4.transform.up, out var hitInfo4))
				{
					gameObject4.transform.position = new Vector3(gameObject4.transform.position.x, hitInfo4.point.y, gameObject4.transform.position.z);
				}
				gameObject4.AddComponent<PeopleController>();
				gameObject4.GetComponent<PeopleController>().SetTarget(vector5);
				gameObject4.GetComponent<PeopleController>().animNames = new string[3] { "talk1", "talk2", "listen" };
				gameObject4.transform.parent = par.transform;
			}
			num6 = UnityEngine.Random.Range(0, peoplePrefabs.Length);
			if (vector6 != Vector3.zero)
			{
				GameObject gameObject5 = UnityEngine.Object.Instantiate(peoplePrefabs[num6], vector7, Quaternion.identity);
				if (Physics.Raycast(gameObject5.transform.position + new Vector3(0f, 1f, 0f), -gameObject5.transform.up, out var hitInfo5))
				{
					gameObject5.transform.position = new Vector3(gameObject5.transform.position.x, hitInfo5.point.y, gameObject5.transform.position.z);
				}
				gameObject5.AddComponent<PeopleController>();
				gameObject5.GetComponent<PeopleController>().SetTarget(vector5);
				gameObject5.GetComponent<PeopleController>().animNames = new string[3] { "talk1", "talk2", "listen" };
				gameObject5.transform.parent = par.transform;
			}
			num6 = UnityEngine.Random.Range(0, peoplePrefabs.Length);
			if (vector6 != Vector3.zero)
			{
				GameObject gameObject6 = UnityEngine.Object.Instantiate(peoplePrefabs[num6], vector8, Quaternion.identity);
				if (Physics.Raycast(gameObject6.transform.position + new Vector3(0f, 1f, 0f), -gameObject6.transform.up, out var hitInfo6))
				{
					gameObject6.transform.position = new Vector3(gameObject6.transform.position.x, hitInfo6.point.y, gameObject6.transform.position.z);
				}
				gameObject6.AddComponent<PeopleController>();
				gameObject6.GetComponent<PeopleController>().SetTarget(vector5);
				gameObject6.GetComponent<PeopleController>().animNames = new string[3] { "talk1", "talk2", "listen" };
				gameObject6.transform.parent = par.transform;
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
			if (IsRandomPositionFree(vector, Vector3.zero, Vector3.zero))
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
			if (Vector3.Distance(vector, position) < GetRealPlaneSize().x / 2f - 0.3f && IsRandomPositionFree(vector, Vector3.zero, Vector3.zero))
			{
				return vector;
			}
		}
		return Vector3.zero;
	}

	private bool IsRandomPositionFree(Vector3 pos, Vector3 helpPoint1, Vector3 helpPoint2)
	{
		for (int i = 0; i < spawnPoints.Count; i++)
		{
			if (spawnPoints[i].x - 0.5f < pos.x && spawnPoints[i].x + 0.5f > pos.x && spawnPoints[i].z - 0.5f < pos.z && spawnPoints[i].z + 0.5f > pos.z)
			{
				return false;
			}
		}
		if (helpPoint1 != Vector3.zero)
		{
			if (helpPoint1.x - 0.6f < pos.x && helpPoint1.x + 0.6f > pos.x && helpPoint1.z - 0.6f < pos.z && helpPoint1.z + 0.6f > pos.z)
			{
				return false;
			}
			if (!isCircle)
			{
				if (!(helpPoint1.x + 0.3f > surface.transform.position.x - GetRealPlaneSize().x / 2f) && !(helpPoint1.x - 0.3f < surface.transform.position.x + GetRealPlaneSize().x / 2f) && !(helpPoint1.z + 0.3f > surface.transform.position.z - GetRealPlaneSize().y / 2f) && !(helpPoint1.z - 0.3f < surface.transform.position.z + GetRealPlaneSize().y / 2f))
				{
					return false;
				}
			}
			else if (Vector3.Distance(helpPoint1, surface.transform.position) >= GetRealPlaneSize().x / 2f - 0.3f)
			{
				return false;
			}
		}
		if (helpPoint2 != Vector3.zero)
		{
			if (helpPoint2.x - 0.6f < pos.x && helpPoint2.x + 0.6f > pos.x && helpPoint2.z - 0.6f < pos.z && helpPoint2.z + 0.6f > pos.z)
			{
				return false;
			}
			if (!isCircle)
			{
				if (!(helpPoint2.x + 0.3f > surface.transform.position.x - GetRealPlaneSize().x / 2f) && !(helpPoint2.x - 0.3f < surface.transform.position.x + GetRealPlaneSize().x / 2f) && !(helpPoint2.z + 0.3f > surface.transform.position.z - GetRealPlaneSize().y / 2f) && !(helpPoint2.z - 0.3f < surface.transform.position.z + GetRealPlaneSize().y / 2f))
				{
					return false;
				}
			}
			else if (Vector3.Distance(helpPoint2, surface.transform.position) >= GetRealPlaneSize().x / 2f - 0.3f)
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
