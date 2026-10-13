using UnityEngine;

public class triger : MonoBehaviour
{
	public GameObject gos;

	private int i;

	public string street;

	private GameObject parent;

	private void Start()
	{
		i = 0;
		parent = base.gameObject.transform.parent.gameObject;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("target_tag") && other.transform.parent.gameObject.name == street)
		{
			if (other.transform.name == "3" && other.transform.parent.name == "d1")
			{
				int num = Random.Range(1, 4);
				Debug.Log("3 " + num);
				street = "d" + num;
			}
			if (other.transform.name == "d3_d2" && other.transform.parent.name == "d3")
			{
				street = "d" + Random.Range(2, 4);
				Debug.Log(street);
			}
			if (other.transform.name == "d2_to_d3" && other.transform.parent.name == "d2")
			{
				street = "d" + 3;
				Debug.Log(street);
			}
			if (other.transform.name == "d4_d3" && other.transform.parent.name == "d4")
			{
				street = "d" + 3;
				Debug.Log(street);
			}
			if (other.transform.name == "d3_d4" && other.transform.parent.name == "d3")
			{
				street = "d" + Random.Range(3, 5);
				Debug.Log(street);
			}
			if (other.transform.name == "d3_to_d2" && other.transform.parent.name == "d3")
			{
				street = "d" + 2;
				Debug.Log(street);
			}
			if (other.transform.name == "d2_d5" && other.transform.parent.name == "d2")
			{
				int num2 = Random.Range(1, 3);
				street = "d" + ((num2 != 1) ? 5 : 2);
				Debug.Log(street);
			}
			if (other.transform.name == "d5_d3" && other.transform.parent.name == "d5")
			{
				int num3 = Random.Range(1, 3);
				street = "d" + ((num3 != 1) ? 5 : 3);
				Debug.Log(street);
			}
			if (other.transform.name == "d5_to_d1" && other.transform.parent.name == "d5")
			{
				street = "d" + 1;
			}
			gos = other.gameObject;
		}
	}

	private GameObject FindClosestEnemy(GameObject[] gos)
	{
		GameObject result = null;
		float num = float.PositiveInfinity;
		Vector3 position = base.transform.position;
		foreach (GameObject gameObject in gos)
		{
			if (gameObject != null)
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
