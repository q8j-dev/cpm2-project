using CPMEngine;
using Photon.Realtime;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	public class RpcSendWheelsState : RpcObjectBase
	{
		public override byte Code => 70;

		public static void Send(MultiPlayerPosControl car, Player player)
		{
			if (player == null || !car || !car.photonView || !car.photonView.IsMine)
			{
				return;
			}
			Axles axles = ((car.CarComponents != null) ? car.CarComponents.Axles : null);
			if ((bool)axles && axles.AllWheels != null && axles.AllWheels.Count >= 4)
			{
				IWheel wheel = axles.AllWheels[0].Wheel;
				IWheel wheel2 = axles.AllWheels[1].Wheel;
				IWheel wheel3 = axles.AllWheels[2].Wheel;
				IWheel wheel4 = axles.AllWheels[3].Wheel;
				if (wheel != null && wheel2 != null && wheel3 != null && wheel4 != null)
				{
					string text = car.photonView.ViewID.ToString();
					string text2 = WheelStatePayload.Pack(wheel.CurSpringLen, wheel2.CurSpringLen, wheel3.CurSpringLen, wheel4.CurSpringLen, SurfaceOf(wheel), SurfaceOf(wheel2), SurfaceOf(wheel3), SurfaceOf(wheel4));
					car.photonView.RPC("SendValueStr", player, (byte)70, text, text2);
				}
			}
		}

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			if (!multiPlayerPosControl || !multiPlayerPosControl.IsThisCar(b) || !WheelStatePayload.TryParse(name, out var payload))
			{
				return;
			}
			Axles axles = ((multiPlayerPosControl.CarComponents != null) ? multiPlayerPosControl.CarComponents.Axles : null);
			if ((bool)axles && axles.AllWheels != null && axles.AllWheels.Count >= 4)
			{
				ApplyWheel(axles.AllWheels[0].Wheel, payload.FrontLeftSpring, payload.FrontLeftSurface);
				ApplyWheel(axles.AllWheels[1].Wheel, payload.FrontRightSpring, payload.FrontRightSurface);
				ApplyWheel(axles.AllWheels[2].Wheel, payload.RearLeftSpring, payload.RearLeftSurface);
				ApplyWheel(axles.AllWheels[3].Wheel, payload.RearRightSpring, payload.RearRightSurface);
				if (axles.Rear1Axle != null && axles.Rear1Axle.Wheels != null && axles.Rear1Axle.Wheels.Count == 2 && axles.AllWheels.Count >= 6)
				{
					ApplyWheel(axles.AllWheels[4].Wheel, payload.RearLeftSpring, payload.RearLeftSurface);
					ApplyWheel(axles.AllWheels[5].Wheel, payload.RearRightSpring, payload.RearRightSurface);
				}
			}
		}

		private static int SurfaceOf(IWheel wheel)
		{
			if (wheel.CurrentPhysMat == null)
			{
				return 0;
			}
			return (int)wheel.CurrentPhysMat.SurfaceType;
		}

		private static void ApplyWheel(IWheel wheel, float spring, int surfaceType)
		{
			if (wheel != null)
			{
				wheel.CurSpringLengthReceived = spring;
				if (wheel.CurrentPhysMat != null)
				{
					wheel.CurrentPhysMat.SurfaceType = (Surface)surfaceType;
				}
			}
		}
	}
}
