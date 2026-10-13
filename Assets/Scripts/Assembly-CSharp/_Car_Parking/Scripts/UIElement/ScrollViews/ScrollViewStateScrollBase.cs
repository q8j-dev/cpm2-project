using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public abstract class ScrollViewStateScrollBase : ScrollViewState
	{
		private class VelocityData
		{
			private List<Vector2> _deltaPositions = new List<Vector2>();

			private List<float> _deltaTimes = new List<float>();

			private int _maxCount;

			public Vector2 Velocity
			{
				get
				{
					Vector2 zero = Vector2.zero;
					float num = 0f;
					for (int i = 0; i < _deltaPositions.Count; i++)
					{
						zero += _deltaPositions[i];
						num += _deltaTimes[i];
					}
					if (num > 0.001f)
					{
						return zero / num;
					}
					return Vector2.zero;
				}
			}

			public VelocityData(int maxCount)
			{
				_maxCount = maxCount;
			}

			public void AddData(Vector2 deltaPosition, float deltaTime)
			{
				_deltaPositions.Add(deltaPosition);
				_deltaTimes.Add(deltaTime);
			}

			public void Clear()
			{
				_deltaPositions.Clear();
				_deltaTimes.Clear();
			}
		}

		private Vector2 _currentMousePos;

		private VelocityData _velocityData;

		public ScrollViewStateScrollBase(ScrollViewBase scrollView)
			: base(scrollView)
		{
			_velocityData = new VelocityData(5);
		}

		public override void OnPointerUp()
		{
			base.OnPointerUp();
			SetStateIdle();
		}

		public override void OnItemUp()
		{
			base.OnItemUp();
			SetStateIdle();
		}

		public override void OnEnter()
		{
			base.OnEnter();
			_velocityData.Clear();
			_currentMousePos = GetMousePosition();
		}

		public override void OnExit()
		{
			base.OnExit();
			_velocityData.Clear();
		}

		public override void Update()
		{
			base.Update();
			Vector2 mousePosition = GetMousePosition();
			Vector2 vector = mousePosition - _currentMousePos;
			_velocityData.AddData(vector, Time.unscaledDeltaTime);
			scrollView.Move(vector);
			_currentMousePos = mousePosition;
			MoveDelta(vector);
		}

		protected virtual void MoveDelta(Vector2 delta)
		{
		}

		protected void SetStateIdle()
		{
			ScrollViewStateIdle state = scrollView.GetState<ScrollViewStateIdle>();
			IdleInertiaModule idleInertiaModule = null;
			if (state != null)
			{
				idleInertiaModule = state.GetModule<IdleInertiaModule>();
			}
			if (idleInertiaModule != null)
			{
				idleInertiaModule.Velocity = _velocityData.Velocity;
			}
			scrollView.SetState(StateType.Idle);
		}
	}
}
