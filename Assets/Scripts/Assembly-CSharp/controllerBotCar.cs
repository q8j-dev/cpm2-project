using UnityEngine;

public class controllerBotCar : MonoBehaviour
{
	private int i;

	private bool first_start = true;

	private void Start()
	{
		i = 1;
	}

	private void OnTriggerEnter(Collider other)
	{
		_ = other.gameObject.name == "target";
		if (other.gameObject.CompareTag("target_tag"))
		{
			GameObject gameObject = base.transform.Find("New Game Object").GetComponent<triger>().gos;
			if (other == base.transform.Find("New Game Object").GetComponent<triger>().gos)
			{
				GetComponent<CarAIController>().target_1 = null;
			}
			else if (gameObject != null)
			{
				GetComponent<CarAIController>().target_1 = gameObject.transform;
			}
		}
	}

	private GameObject FindClosestEnemy(GameObject[] gos)
	{
		GameObject result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		foreach (GameObject gameObject in gos)
		{
			if (gameObject != null && gameObject.transform != GetComponent<CarAIController>().target_1)
			{
				Vector3 vector = base.transform.InverseTransformPoint(gameObject.transform.position);
				float num2 = Mathf.Atan2(vector.x, vector.z);
				num2 = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				float sqrMagnitude = (gameObject.transform.position - position).sqrMagnitude;
				if (num2 > -90f && num2 < 90f && sqrMagnitude < num)
				{
					result = gameObject;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}
}
