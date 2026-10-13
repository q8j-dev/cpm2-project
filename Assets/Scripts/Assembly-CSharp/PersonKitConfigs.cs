using UnityEngine.Scripting;

[Preserve]
public class PersonKitConfigs
{
	public PersonKitConfig Male { get; }

	public PersonKitConfig Female { get; }

	public PersonKitConfigs(PersonKitConfig male, PersonKitConfig female)
	{
		Male = male;
		Female = female;
	}
}
