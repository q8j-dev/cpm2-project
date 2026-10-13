using System.IO;
using System.Linq;
using Service;
using UnityEngine;

public class NamesParser : MonoBehaviour
{
	public NamesScriptables namesSO;

	private void Start()
	{
		string[] source = File.ReadAllLines(Directory.GetCurrentDirectory() + "/Assets/_Car Parking/Scriptables/Services/names.txt");
		namesSO.names = source.ToList();
	}
}
