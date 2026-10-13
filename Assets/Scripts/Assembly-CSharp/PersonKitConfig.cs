using UnityEngine;

[CreateAssetMenu(fileName = "PersonKitConfig", menuName = "Scriptable/PersonKitConfig")]
public class PersonKitConfig : ScriptableObject
{
	public PersonElements.PersonKitData[] hair;

	public PersonElements.PersonKitData[] face;

	public PersonElements.PersonKitData[] beard;

	public PersonElements.PersonKitData[] caps;

	public PersonElements.PersonKitData[] mask;

	public PersonElements.PersonKitData[] top;

	public PersonElements.PersonKitData[] gloves;

	public PersonElements.PersonKitData[] bag;

	public PersonElements.PersonKitData[] pants;

	public PersonElements.PersonKitData[] shoes;

	public PersonElements.PersonKitData[] glasses;

	private PersonElements.PersonKitData[][] _allKits;

	public PersonElements.PersonKitData[][] allKits
	{
		get
		{
			if (_allKits.IsNullOrEmpty())
			{
				SetAllKits();
			}
			return _allKits;
		}
	}

	private void SetAllKits()
	{
		_allKits = new PersonElements.PersonKitData[11][];
		_allKits[0] = hair;
		_allKits[1] = face;
		_allKits[2] = beard;
		_allKits[3] = caps;
		_allKits[4] = mask;
		_allKits[5] = top;
		_allKits[6] = gloves;
		_allKits[7] = bag;
		_allKits[8] = pants;
		_allKits[9] = shoes;
		_allKits[10] = glasses;
	}

	public PersonElements.PersonKitData[] GetKitsByTypeId(int index)
	{
		return index switch
		{
			0 => hair, 
			1 => face, 
			2 => beard, 
			3 => caps, 
			4 => mask, 
			5 => top, 
			6 => gloves, 
			7 => bag, 
			8 => pants, 
			9 => shoes, 
			10 => glasses, 
			_ => null, 
		};
	}
}
