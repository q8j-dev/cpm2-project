using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.WindowSystem.PropertyWindow
{
	public class PropertyWindow : WindowScript
	{
		[SerializeField]
		private VerticalLayoutGroup MainFraneGroup;

		[SerializeField]
		private List<PropertiesUISection> PropertiesSections;

		[SerializeField]
		private List<PropertyUIObject> PropertiesHandlers;

		[SerializeField]
		private List<float> FrameSpacings;

		private PropertiesUISection _currentSection;

		private GameObject _currentPropertyHandler;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			if (int.TryParse(data, out var result))
			{
				Init(result);
			}
		}

		private void Init(int index)
		{
			V.CoreGameManager.ManagerUI.OnBackButtonActivated?.Invoke(obj: true);
			foreach (PropertiesUISection propertiesSection in PropertiesSections)
			{
				propertiesSection.Selected += OnSelectSelection;
			}
			foreach (PropertyUIObject propertiesHandler in PropertiesHandlers)
			{
				propertiesHandler.GameObject.SetActive(value: false);
			}
			SetSection(PropertiesSections[index], withAnimation: false);
			SetHandler(PropertiesSections[index]);
		}

		private void OnSelectSelection(PropertiesUISection section)
		{
			if (!(_currentSection == section))
			{
				SetSection(section);
				SetHandler(section);
			}
		}

		private void SetSection(PropertiesUISection section, bool withAnimation = true)
		{
			if ((bool)_currentSection)
			{
				_currentSection.Select(value: false, withAnimation);
			}
			_currentSection = section;
			_currentSection.Select(value: true, withAnimation);
		}

		private void SetHandler(PropertiesUISection section)
		{
			if ((bool)_currentPropertyHandler)
			{
				_currentPropertyHandler.SetActive(value: false);
			}
			_currentPropertyHandler = GetPropertiesObjectByType(PropertiesHandlers, _currentSection.PropertyType);
			_currentPropertyHandler.SetActive(value: true);
			MainFraneGroup.spacing = FrameSpacings[PropertiesSections.IndexOf(section)];
		}

		private GameObject GetPropertiesObjectByType(List<PropertyUIObject> propertiesList, PropertyType propertyType)
		{
			return propertiesList.First((PropertyUIObject x) => x.PropertyType == propertyType).GameObject;
		}
	}
}
