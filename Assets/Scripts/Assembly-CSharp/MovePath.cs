using System;
using UnityEngine;

[Serializable]
public class MovePath : MonoBehaviour
{
	[SerializeField]
	public Vector3 startPos;

	[SerializeField]
	public Vector3 finishPos;

	[SerializeField]
	public int w;

	[SerializeField]
	public int targetPoint;

	[SerializeField]
	public int targetPointsTotal;

	[SerializeField]
	public string animName;

	[SerializeField]
	public float moveSpeed;

	[SerializeField]
	public bool loop;

	[SerializeField]
	public bool forward;

	[SerializeField]
	public GameObject walkPath;

	private int count;

	public void MyStart(int _w, int _i, string anim, bool _loop, bool _forward, float _moveSpeed)
	{
		forward = _forward;
		moveSpeed = _moveSpeed;
		WalkPath component = walkPath.GetComponent<WalkPath>();
		w = _w;
		targetPointsTotal = component.getPointsTotal(0) - 2;
		loop = _loop;
		animName = anim;
		if (loop)
		{
			if (_i < targetPointsTotal && _i > 0)
			{
				if (forward)
				{
					targetPoint = _i + 1;
					finishPos = component.getNextPoint(w, _i + 1);
				}
				else
				{
					targetPoint = _i;
					finishPos = component.getNextPoint(w, _i);
				}
			}
			else if (forward)
			{
				targetPoint = 1;
				finishPos = component.getNextPoint(w, 1);
			}
			else
			{
				targetPoint = targetPointsTotal;
				finishPos = component.getNextPoint(w, targetPointsTotal);
			}
		}
		else if (forward)
		{
			targetPoint = _i + 1;
			finishPos = component.getNextPoint(w, _i + 1);
		}
		else
		{
			targetPoint = _i;
			finishPos = component.getNextPoint(w, _i);
		}
	}

	private void Start()
	{
		Vector3 worldPosition = new Vector3(finishPos.x, base.transform.position.y, finishPos.z);
		base.transform.LookAt(worldPosition);
		GetComponent<Animator>().CrossFade(animName, 0.1f, 0, UnityEngine.Random.Range(0f, 1f));
		if (animName == "walk")
		{
			GetComponent<Animator>().speed = moveSpeed * 1.2f;
		}
		else if (animName == "run")
		{
			GetComponent<Animator>().speed = moveSpeed / 3f;
		}
	}

	private void Update()
	{
		if (count > 30)
		{
			if (Physics.Raycast(base.transform.position + new Vector3(0f, 0.4f, 0f), -base.transform.up, out var hitInfo))
			{
				finishPos.y = hitInfo.point.y;
				base.transform.position = new Vector3(base.transform.position.x, hitInfo.point.y, base.transform.position.z);
			}
			count = 0;
		}
		count++;
		Vector3 vector = new Vector3(finishPos.x, base.transform.position.y, finishPos.z);
		WalkPath component = walkPath.GetComponent<WalkPath>();
		if (Vector3.Distance(base.transform.position, finishPos) < 0.2f && animName == "walk" && (loop || (!loop && targetPoint > 0 && targetPoint < targetPointsTotal)))
		{
			if (forward)
			{
				vector = ((targetPoint >= targetPointsTotal) ? component.getNextPoint(w, 0) : component.getNextPoint(w, targetPoint + 1));
				vector.y = base.transform.position.y;
			}
			else
			{
				vector = ((targetPoint <= 0) ? component.getNextPoint(w, targetPointsTotal) : component.getNextPoint(w, targetPoint - 1));
				vector.y = base.transform.position.y;
			}
		}
		if (Vector3.Distance(base.transform.position, finishPos) < 0.5f && animName == "run" && (loop || (!loop && targetPoint > 0 && targetPoint < targetPointsTotal)))
		{
			if (forward)
			{
				vector = ((targetPoint >= targetPointsTotal) ? component.getNextPoint(w, 0) : component.getNextPoint(w, targetPoint + 1));
				vector.y = base.transform.position.y;
			}
			else
			{
				vector = ((targetPoint <= 0) ? component.getNextPoint(w, targetPointsTotal) : component.getNextPoint(w, targetPoint - 1));
				vector.y = base.transform.position.y;
			}
		}
		Vector3 vector2 = vector - base.transform.position;
		if (vector2 != Vector3.zero)
		{
			Quaternion rotation = Quaternion.identity;
			if (animName == "walk")
			{
				rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector2), Time.deltaTime * 4f * moveSpeed);
			}
			else if (animName == "run")
			{
				rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vector2), Time.deltaTime * 1.3f * moveSpeed);
			}
			base.transform.rotation = rotation;
		}
		if (base.transform.position != finishPos)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, finishPos, Time.deltaTime * 1f * moveSpeed);
		}
		else if (base.transform.position == finishPos && forward)
		{
			if (targetPoint != targetPointsTotal)
			{
				targetPoint++;
				finishPos = component.getNextPoint(w, targetPoint);
			}
			else if (targetPoint == targetPointsTotal)
			{
				if (loop)
				{
					finishPos = component.getStartPoint(w);
					targetPoint = 0;
				}
				else
				{
					finishPos = component.getNextPoint(w, 1);
					base.transform.position = component.getNextPoint(w, 0);
					targetPoint = 1;
				}
			}
		}
		else
		{
			if (!(base.transform.position == finishPos) || forward)
			{
				return;
			}
			if (targetPoint > 0)
			{
				targetPoint--;
				finishPos = component.getNextPoint(w, targetPoint);
			}
			else if (targetPoint == 0)
			{
				if (loop)
				{
					finishPos = component.getNextPoint(w, targetPointsTotal);
					targetPoint = targetPointsTotal;
				}
				else
				{
					finishPos = component.getNextPoint(w, targetPointsTotal - 1);
					base.transform.position = component.getNextPoint(w, targetPointsTotal);
					targetPoint = targetPointsTotal - 1;
				}
			}
		}
	}
}
