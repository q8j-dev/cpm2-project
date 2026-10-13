using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem.InitializersWindows.PersonWindowInitializers
{
	[UnityEngine.Scripting.Preserve]
	public class FirstOpenGamePersonWindowInitializer : IWindowInitializer
	{
		[Inject]
		private ManagerUI _managerUI;

		public void Init(WindowScript windowScript)
		{
			if (windowScript is PersonWindow personWindow)
			{
				personWindow.Person = V.DriverPerson;
				personWindow.mainPerson = V.DriverPerson.GetComponent<MainPerson>();
				personWindow.SaveAndContinueButton.gameObject.SetActive(value: true);
				personWindow.CloseWindowsOnFirstStart();
				personWindow.BackButton.gameObject.SetActive(value: true);
				personWindow.BackButton.onClick.AddListener(personWindow.BackToPresets);
				personWindow.NiknameInputField.gameObject.SetActive(value: true);
				personWindow.NiknameInputField.SetTextWithoutNotify(V.CoreGameManager.NikNameInputText);
				personWindow.NiknameInputField.onValueChanged.AddListener((string name) =>
				{
					V.CoreGameManager.NikNameInputText = name;
				});
				personWindow.canvasGroup.alpha = 1f;
				personWindow.canvasGroup.interactable = true;
				RectTransform saveAndContinueButtonRect = personWindow.SaveAndContinueButton.GetComponent<RectTransform>();
				Observable.NextFrame().Subscribe((Unit _) =>
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(saveAndContinueButtonRect);
				}).AddTo(personWindow);
			}
		}
	}
}
