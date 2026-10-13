using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	public class SummaryOfEventsNavButton : Button
	{
		[SerializeField]
		private HorizontalScrollSnap ScrollSnap = default;

		[SerializeField]
		private RectTransform Content = default;

		[SerializeField]
		private RectTransform Viewport = default;

		[SerializeField]
		private Selectable UpTarget = default;

		protected override void Awake()
		{
			base.Awake();
			base.onClick.AddListener(ClickCenteredItem);
		}

		protected override void OnDestroy()
		{
			base.onClick.RemoveListener(ClickCenteredItem);
			base.OnDestroy();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if ((bool)ScrollSnap)
			{
				ScrollSnap.OnSelectionChangeEndEvent.AddListener(OnPageChangeEnd);
			}
		}

		protected override void OnDisable()
		{
			if ((bool)ScrollSnap)
			{
				ScrollSnap.OnSelectionChangeEndEvent.RemoveListener(OnPageChangeEnd);
			}
			base.OnDisable();
		}

		public override void OnMove(AxisEventData eventData)
		{
			switch (eventData.moveDir)
			{
			case MoveDirection.Left:
				if (CanPage())
				{
					ScrollSnap.PreviousScreen();
				}
				return;
			case MoveDirection.Right:
				if (CanPage())
				{
					ScrollSnap.NextScreen();
				}
				return;
			case MoveDirection.Up:
				if ((bool)UpTarget && UpTarget.gameObject.activeInHierarchy && UpTarget.interactable)
				{
					UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(UpTarget.gameObject);
					return;
				}
				break;
			}
			base.OnMove(eventData);
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			StopAutoScrollWhileSelected().Forget();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			if ((bool)ScrollSnap && ScrollSnap.IsInitialized)
			{
				ScrollSnap.OnSelectionChangeEndEvent.Invoke(ScrollSnap.CurrentPage);
			}
		}

		private void OnPageChangeEnd(int page)
		{
			StopAutoScrollWhileSelected().Forget();
		}

		private async UniTaskVoid StopAutoScrollWhileSelected()
		{
			await UniTask.Yield();
			if ((bool)this && base.isActiveAndEnabled && (bool)ScrollSnap)
			{
				UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
				if ((bool)current && current.currentSelectedGameObject == base.gameObject)
				{
					ScrollSnap.OnSelectionChangeStartEvent.Invoke();
				}
			}
		}

		private bool CanPage()
		{
			if ((bool)ScrollSnap && ScrollSnap.IsInitialized && (bool)Content)
			{
				return Content.childCount > 1;
			}
			return false;
		}

		private void ClickCenteredItem()
		{
			RectTransform rectTransform = FindCenteredItem();
			if ((bool)rectTransform)
			{
				Button componentInChildren = rectTransform.GetComponentInChildren<Button>(includeInactive: true);
				if ((bool)componentInChildren && componentInChildren.isActiveAndEnabled && componentInChildren.interactable)
				{
					ExecuteEvents.Execute(componentInChildren.gameObject, new BaseEventData(UnityEngine.EventSystems.EventSystem.current), ExecuteEvents.submitHandler);
				}
			}
		}

		private RectTransform FindCenteredItem()
		{
			if (!Content || !Viewport)
			{
				return null;
			}
			RectTransform result = null;
			float num = float.MaxValue;
			for (int i = 0; i < Content.childCount; i++)
			{
				if (Content.GetChild(i) is RectTransform rectTransform && rectTransform.gameObject.activeSelf)
				{
					float num2 = Mathf.Abs(Viewport.InverseTransformPoint(rectTransform.TransformPoint(rectTransform.rect.center)).x);
					if (num2 < num)
					{
						num = num2;
						result = rectTransform;
					}
				}
			}
			return result;
		}
	}
}
