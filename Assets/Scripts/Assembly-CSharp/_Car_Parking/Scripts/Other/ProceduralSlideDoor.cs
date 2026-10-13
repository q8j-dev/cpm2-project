using DG.Tweening;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.Other
{
	public class ProceduralSlideDoor : ProceduralDoor
	{
		public float MoveLength;

		private float _startX;

		protected override void Start()
		{
			base.Start();
			_startX = base.transform.localPosition.x;
		}

		protected override void InteractInternal()
		{
			float moveLength = MoveLength;
			AudioWrapper.PostEvent(base.IsOpen ? "Play_Slide_Door_Close" : "Play_Slide_Door_Open", base.gameObject);
			base.Tween = base.transform.DOLocalMoveX(base.IsOpen ? _startX : (_startX + moveLength), Duration).SetEase(Ease.OutQuart);
		}
	}
}
