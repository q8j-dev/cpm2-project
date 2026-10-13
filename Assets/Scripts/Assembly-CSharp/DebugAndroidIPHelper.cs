using TMPro;
using UnityEngine;

public class DebugAndroidIPHelper : MonoBehaviour
{
	[SerializeField]
	private TMP_Text ipText;

	private static DebugAndroidIPHelper _instance;

	private void Awake()
	{
		Object.Destroy(this);
	}

	private void Start()
	{
	}
}
