using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

[CreateAssetMenu(fileName = "DetailVendorsConfig", menuName = "Scriptable/DetailVendorsConfig", order = 15)]
[Preserve]
public class DetailVendorsConfigs : ScriptableObject
{
	public Vendor[] Vendors;

	[Preserve]
	public DetailVendorsConfigs()
	{
	}

	public Vendor GetVendorById(int id)
	{
		return Vendors.FirstOrDefault((Vendor t) => t.Id == id);
	}
}
