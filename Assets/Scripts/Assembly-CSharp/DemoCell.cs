using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.UI;

public class DemoCell : MonoBehaviour, ICell
{
	public Text nameLabel;

	public Text genderLabel;

	public Text idLabel;

	private ContactInfo _contactInfo;

	private int _cellIndex;

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(ButtonListener);
	}

	public void ConfigureCell(ContactInfo contactInfo, int cellIndex)
	{
		_cellIndex = cellIndex;
		_contactInfo = contactInfo;
		nameLabel.text = contactInfo.Name;
		genderLabel.text = contactInfo.Gender;
		idLabel.text = contactInfo.id;
	}

	private void ButtonListener()
	{
		Debug.Log("Index : " + _cellIndex + ", Name : " + _contactInfo.Name + ", Gender : " + _contactInfo.Gender);
	}
}
