using CodeStage.AntiCheat.Storage;
using UnityEngine;

public class EasterEggDestroyChecker : MonoBehaviour
{
	private void Start()
	{
		if (int.TryParse(base.gameObject.name, out var result))
		{
			if (ObscuredPrefs.GetInt("egg" + result) == 1)
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
