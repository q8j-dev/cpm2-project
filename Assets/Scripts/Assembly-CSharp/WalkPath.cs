using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WalkPath : MonoBehaviour
{
	public enum EnumDir
	{
		Forward = 0,
		Backward = 1,
		HugLeft = 2,
		HugRight = 3,
		WeaveLeft = 4,
		WeaveRight = 5
	}

	public enum EnumMove
	{
		Walk = 0,
		Run = 1
	}

	public GameObject[] peoplePrefabs;

	[HideInInspector]
	public List<Vector3> pathPoint = new List<Vector3>();

	[HideInInspector]
	public List<GameObject> pathPointTransform = new List<GameObject>();

	[HideInInspector]
	public Vector3[,] points;

	[HideInInspector]
	public List<Vector3> CalcPoint = new List<Vector3>();

	public int numberOfWays;

	[HideInInspector]
	public int[] pointLength = new int[10];

	public float lineSpacing;

	public float density;

	public bool loopPath;

	public int skipCount;

	public EnumDir direction;

	public EnumMove _moveType;

	public float moveSpeed = 1f;

	[HideInInspector]
	public bool[] _forward;

	[HideInInspector]
	public bool isWalk;

	[HideInInspector]
	public GameObject par;

	public Vector3 getNextPoint(int w, int index)
	{
		return points[w, index];
	}

	public Vector3 getStartPoint(int w)
	{
		return points[w, 1];
	}

	public int getPointsTotal(int w)
	{
		return pointLength[w];
	}

	private void Awake()
	{
		DrawCurved(withDraw: false);
	}

	public void OnDrawGizmos()
	{
		DrawCurved(withDraw: true);
	}

	public void DrawCurved(bool withDraw)
	{
		if (numberOfWays < 1)
		{
			numberOfWays = 1;
		}
		if (lineSpacing < 0.6f)
		{
			lineSpacing = 0.6f;
		}
		if (density < 0.01f)
		{
			density = 0.01f;
		}
		if (moveSpeed < 0.1f)
		{
			moveSpeed = 0.1f;
		}
		_forward = new bool[numberOfWays];
		isWalk = _moveType.ToString() == "Walk";
		for (int i = 0; i < numberOfWays; i++)
		{
			if (direction.ToString() == "Forward")
			{
				_forward[i] = true;
			}
			else if (direction.ToString() == "Backward")
			{
				_forward[i] = false;
			}
			else if (direction.ToString() == "HugLeft")
			{
				if ((i + 2) % 2 == 0)
				{
					_forward[i] = true;
				}
				else
				{
					_forward[i] = false;
				}
			}
			else if (direction.ToString() == "HugRight")
			{
				if ((i + 2) % 2 == 0)
				{
					_forward[i] = false;
				}
				else
				{
					_forward[i] = true;
				}
			}
			else if (direction.ToString() == "WeaveLeft")
			{
				if (i == 1 || i == 2 || (i - 1) % 4 == 0 || (i - 2) % 4 == 0)
				{
					_forward[i] = false;
				}
				else
				{
					_forward[i] = true;
				}
			}
			else if (direction.ToString() == "WeaveRight")
			{
				if (i == 1 || i == 2 || (i - 1) % 4 == 0 || (i - 2) % 4 == 0)
				{
					_forward[i] = true;
				}
				else
				{
					_forward[i] = false;
				}
			}
		}
		if (pathPoint.Count < 2)
		{
			return;
		}
		points = new Vector3[numberOfWays + 2, pathPoint.Count + 2];
		pointLength[0] = pathPoint.Count + 2;
		for (int j = 0; j < pathPoint.Count; j++)
		{
			points[0, j + 1] = pathPointTransform[j].transform.position;
		}
		points[0, 0] = points[0, 1];
		points[0, pointLength[0] - 1] = points[0, pointLength[0] - 2];
		for (int k = 0; k < pointLength[0]; k++)
		{
			if (k != 0 && withDraw)
			{
				Gizmos.color = (_forward[0] ? Color.green : Color.red);
				Gizmos.DrawLine(points[0, k], points[0, k - 1]);
			}
		}
		if (loopPath && withDraw)
		{
			Gizmos.color = (_forward[0] ? Color.green : Color.red);
			Gizmos.DrawLine(points[0, 1], points[0, pointLength[0] - 2]);
		}
		for (int l = 1; l < numberOfWays; l++)
		{
			if (numberOfWays <= 1)
			{
				continue;
			}
			if (!loopPath)
			{
				Vector3 vector = points[0, 2] - points[0, 1];
				vector = Quaternion.Euler(0f, -90f, 0f) * vector;
				if (l % 2 == 0)
				{
					vector = vector.normalized * ((float)l * 0.5f * lineSpacing);
				}
				else if (l % 2 == 1)
				{
					vector = vector.normalized * ((float)(l + 1) * 0.5f * lineSpacing);
				}
				Vector3 vector2 = Vector3.zero;
				if (l % 2 == 1)
				{
					vector2 = points[0, 1] - vector;
				}
				else if (l % 2 == 0)
				{
					vector2 = points[0, 1] + vector;
				}
				vector2.y = points[0, 1].y;
				points[l, 0] = vector2;
				points[l, 1] = vector2;
				Vector3 vector3 = points[0, pointLength[0] - 3] - points[0, pointLength[0] - 2];
				vector3 = Quaternion.Euler(0f, 90f, 0f) * vector3;
				if (l % 2 == 0)
				{
					vector3 = vector3.normalized * ((float)l * 0.5f * lineSpacing);
				}
				else if (l % 2 == 1)
				{
					vector3 = vector3.normalized * ((float)(l + 1) * 0.5f * lineSpacing);
				}
				Vector3 vector4 = Vector3.zero;
				if (l % 2 == 1)
				{
					vector4 = points[0, pointLength[0] - 2] - vector3;
				}
				else if (l % 2 == 0)
				{
					vector4 = points[0, pointLength[0] - 2] + vector3;
				}
				vector4.y = points[0, pointLength[0] - 2].y;
				points[l, pointLength[0] - 2] = vector4;
				points[l, pointLength[0] - 1] = vector4;
			}
			else
			{
				Vector3 vector5 = points[0, pointLength[0] - 2] - points[0, 1];
				Vector3 vector6 = points[0, 1] - points[0, 2];
				Vector3 vector7 = vector5;
				float num = Mathf.DeltaAngle(Mathf.Atan2(vector6.x, vector6.z) * 57.29578f, Mathf.Atan2(vector7.x, vector7.z) * 57.29578f);
				if (l % 2 == 0)
				{
					vector6 = vector6.normalized * ((float)l * 0.5f * lineSpacing);
				}
				else if (l % 2 == 1)
				{
					vector6 = vector6.normalized * ((float)(l + 1) * 0.5f * lineSpacing);
				}
				vector6 = Quaternion.Euler(0f, 90f + num / 2f, 0f) * vector6;
				Vector3 vector8 = Vector3.zero;
				if (l % 2 == 1)
				{
					vector8 = points[0, 1] - vector6;
				}
				else if (l % 2 == 0)
				{
					vector8 = points[0, 1] + vector6;
				}
				vector8.y = points[0, 1].y;
				points[l, 1] = vector8;
				points[l, 0] = vector8;
				Vector3 vector9 = points[0, pointLength[0] - 2] - points[0, 1];
				Vector3 vector10 = points[0, pointLength[0] - 3] - points[0, pointLength[0] - 2];
				Vector3 vector11 = vector9;
				float num2 = Mathf.DeltaAngle(Mathf.Atan2(vector10.x, vector10.z) * 57.29578f, Mathf.Atan2(vector11.x, vector11.z) * 57.29578f);
				if (l % 2 == 0)
				{
					vector10 = vector10.normalized * ((float)l * 0.5f * lineSpacing);
				}
				else if (l % 2 == 1)
				{
					vector10 = vector10.normalized * ((float)(l + 1) * 0.5f * lineSpacing);
				}
				vector10 = Quaternion.Euler(0f, 90f + num2 / 2f, 0f) * vector10;
				Vector3 vector12 = Vector3.zero;
				if (l % 2 == 1)
				{
					vector12 = points[0, pointLength[0] - 2] - vector10;
				}
				else if (l % 2 == 0)
				{
					vector12 = points[0, pointLength[0] - 2] + vector10;
				}
				vector12.y = points[0, pointLength[0] - 2].y;
				points[l, pointLength[0] - 2] = vector12;
				points[l, pointLength[0] - 1] = vector12;
			}
			for (int m = 2; m < pointLength[0] - 2; m++)
			{
				Vector3 vector13 = points[0, m] - points[0, m + 1];
				Vector3 vector14 = points[0, m - 1] - points[0, m];
				Vector3 vector15 = vector13;
				float num3 = Mathf.DeltaAngle(Mathf.Atan2(vector14.x, vector14.z) * 57.29578f, Mathf.Atan2(vector15.x, vector15.z) * 57.29578f);
				if (l % 2 == 0)
				{
					vector14 = vector14.normalized * ((float)l * 0.5f * lineSpacing);
				}
				else if (l % 2 == 1)
				{
					vector14 = vector14.normalized * ((float)(l + 1) * 0.5f * lineSpacing);
				}
				vector14 = Quaternion.Euler(0f, 90f + num3 / 2f, 0f) * vector14;
				Vector3 vector16 = Vector3.zero;
				if (l % 2 == 1)
				{
					vector16 = points[0, m] - vector14;
				}
				else if (l % 2 == 0)
				{
					vector16 = points[0, m] + vector14;
				}
				vector16.y = points[0, m].y;
				points[l, m] = vector16;
				if (withDraw)
				{
					Gizmos.color = (_forward[l] ? Color.green : Color.red);
					Gizmos.DrawLine(points[l, m - 1], points[l, m]);
				}
			}
			if (withDraw)
			{
				Gizmos.color = (_forward[l] ? Color.green : Color.red);
				Gizmos.DrawLine(points[l, pointLength[0] - 2], points[l, pointLength[0] - 3]);
			}
			if (withDraw && loopPath)
			{
				Gizmos.color = (_forward[l] ? Color.green : Color.red);
				Gizmos.DrawLine(points[l, 1], points[l, pointLength[0] - 2]);
			}
		}
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

	public void SpawnOnePeople(int w, bool forward, float mSpeed)
	{
		int num = UnityEngine.Random.Range(0, 16);
		GameObject gameObject = base.gameObject;
		gameObject = (forward ? (UnityEngine.Object.Instantiate(Resources.Load("persons/person" + num, typeof(GameObject)), points[w, 1], Quaternion.identity) as GameObject) : (UnityEngine.Object.Instantiate(Resources.Load("persons/person" + num, typeof(GameObject)), points[w, pointLength[0] - 2], Quaternion.identity) as GameObject));
		MovePath movePath = gameObject.AddComponent<MovePath>();
		gameObject.transform.parent = par.transform;
		movePath.walkPath = base.gameObject;
		string anim = ((!isWalk) ? "run" : "walk");
		if (!forward)
		{
			movePath.MyStart(w, pointLength[0] - 2, anim, loopPath, forward, mSpeed);
			gameObject.transform.LookAt(points[w, pointLength[0] - 3]);
		}
		else
		{
			movePath.MyStart(w, 1, anim, loopPath, forward, mSpeed);
			gameObject.transform.LookAt(points[w, 2]);
		}
	}

	public void SpawnPeople()
	{
		if (par == null)
		{
			par = new GameObject();
			par.transform.parent = base.gameObject.transform;
			par.name = "people";
		}
		int num = (loopPath ? (pointLength[0] - 1) : (pointLength[0] - 2));
		bool flag = false;
		for (int i = 0; i < numberOfWays; i++)
		{
			float num2 = moveSpeed + UnityEngine.Random.Range(moveSpeed * -0.15f, moveSpeed * 0.15f);
			int num3 = 0;
			int num4 = 0;
			for (int j = 1; j < num; j++)
			{
				if (num3 == skipCount)
				{
					flag = false;
					num3 = 0;
				}
				else
				{
					flag = true;
					num3++;
				}
				if (!flag)
				{
					num4++;
					bool flag2 = false;
					if (direction.ToString() == "Forward")
					{
						flag2 = true;
					}
					else if (direction.ToString() == "Backward")
					{
						flag2 = false;
					}
					else if (direction.ToString() == "HugLeft")
					{
						flag2 = (i + 2) % 2 == 0;
					}
					else if (direction.ToString() == "HugRight")
					{
						flag2 = (((i + 2) % 2 != 0) ? true : false);
					}
					else if (direction.ToString() == "WeaveLeft")
					{
						flag2 = ((i != 1 && i != 2 && (i - 1) % 4 != 0 && (i - 2) % 4 != 0) ? true : false);
					}
					else if (direction.ToString() == "WeaveRight")
					{
						flag2 = ((i == 1 || i == 2 || (i - 1) % 4 == 0 || (i - 2) % 4 == 0) ? true : false);
					}
					Vector3 zero = Vector3.zero;
					zero = ((!loopPath || j != num - 1) ? (points[i, j + 1] - points[i, j]) : (points[i, 1] - points[i, num]));
					float magnitude = zero.magnitude;
					int num5 = (int)(density / 5f * magnitude);
					if (num5 < 1)
					{
						num5 = 1;
					}
					float num6 = magnitude / (float)num5;
					float minInclusive = 0f;
					for (int k = 0; k < num5; k++)
					{
						float num7 = 0f;
						num7 = ((k != 0) ? UnityEngine.Random.Range(minInclusive, (float)(k + 1) * num6 - 0.5f) : UnityEngine.Random.Range(minInclusive, num6 - 0.5f));
						minInclusive = num7 + 0.5f;
						int num8 = UnityEngine.Random.Range(0, 13);
						GameObject gameObject = base.gameObject;
						gameObject = ((!flag2) ? (UnityEngine.Object.Instantiate(Resources.Load("persons/person" + num8, typeof(GameObject)), points[i, j] + zero * (num7 / magnitude), Quaternion.identity) as GameObject) : (UnityEngine.Object.Instantiate(Resources.Load("persons/person" + num8, typeof(GameObject)), points[i, j] + zero * (num7 / magnitude), Quaternion.identity) as GameObject));
						MovePath movePath = gameObject.AddComponent<MovePath>();
						gameObject.transform.parent = par.transform;
						movePath.walkPath = base.gameObject;
						string anim = ((!isWalk) ? "run" : "walk");
						movePath.MyStart(i, j, anim, loopPath, flag2, num2);
					}
				}
			}
		}
	}
}
