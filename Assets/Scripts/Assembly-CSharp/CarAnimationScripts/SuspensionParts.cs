using System;
using UnityEngine;

namespace CarAnimationScripts
{
	[Serializable]
	public class SuspensionParts
	{
		public Transform UpperArm;

		public Transform MiddleArm;

		public Transform LowerArm;

		[Tooltip("Продольный рычаг (wishbone/реактивная тяга задней подвески): качается вокруг поперечной оси кузова.")]
		public Transform TrailingArm;

		public Transform Knuckle;

		public Transform UpperBallJoint;

		public Transform MiddleBallJoint;

		public Transform LowerBallJoint;

		[Tooltip("Прицел продольного рычага — референс-точка на кулаке. Нет ссылки — рычаг целится в кулак.")]
		public Transform TrailingJoint;

		public Transform SpringUnitBody;

		public Transform Spring;

		public Transform DamperLower;

		public Transform DamperUpper;

		public Transform Antiroll;

		public Transform DriveAxle;

		public Transform AntirollAttach;
	}
}
