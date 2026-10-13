using CPMEngine.Protected;
using Unity.Mathematics;
using UnityEngine;

namespace CPMEngine
{
	public interface IWheel
	{
		bool Enabled { get; set; }

		bool StaticMode { get; }

		bool OnGroundDown { get; }

		bool DiggerSurface { get; }

		bool SkidOnline { get; set; }

		bool Compression { get; }

		bool IsPowered { get; set; }

		bool IsFront { get; set; }

		bool NeedsOpenWheelCollider { get; set; }

		float TyreTemperature { get; set; }

		float GripTemperature { get; set; }

		float SurfaceTemperature { get; set; }

		bool CarDrifting { get; set; }

		float TyreHealth { get; }

		float TyreFriction { set; }

		float FrictionMultiplier { get; set; }

		float ToeIn { get; set; }

		float Caster { get; set; }

		Vector3 TopPos { get; set; }

		Vector3 CasterPoint { get; }

		Vector3 CasterDir { get; }

		float Radius { get; set; }

		float Mass { get; set; }

		float MaxSpringLen { get; set; }

		float SpringCoef { get; set; }

		float Camber { set; }

		float TyrePressure { get; set; }

		float2 BumpRate { get; set; }

		float2 ReboundRate { get; set; }

		float CurSpringLen { get; }

		float StaticSag { get; set; }

		float CurSpringLengthReceived { set; }

		float AddSteerAngle { get; set; }

		float RollAngle { get; }

		float AngularVelocity { get; set; }

		float SideVelocity { get; }

		float ForwardVelocity { get; }

		float WheelTireVelocity { get; }

		float GetSkidVolume { get; }

		float SlipAngleVolume { get; }

		float SlipRatioVolume { get; }

		float LongtitudeForce { get; }

		float LateralForce { get; }

		float NormalForce { get; }

		float LongitudinalSlip { get; }

		float BestSlipAngle { get; }

		Vector3 LongForce { get; }

		Vector3 LatForce { get; }

		Vector3 VertForce { get; }

		float AlignMom { get; }

		Vector3 SmokeVelocity { get; }

		PhysicMaterial CurrentPhysMat { get; }

		bool GetSmoke();

		bool GetPatch(out Vector3 left, out Vector3 right, out Vector3 cnt, out Vector3 n);

		bool GetMarkPatch(out Vector3 left, out Vector3 right, out Vector3 cnt, out Vector3 n);

		int GetRayCount();

		bool GetRayTracePoint(int index, out Vector3 p1, out Vector3 p2);

		void SetRayTraceResult(int index, bool result, RayInfo info);

		void SelectContactRay();

		bool GetDeform(out Vector3 patchCnt, out Vector3 latDeformVec, out Vector3 longDeformVec, out Plane vertDeformPlane);

		void GetPositionAndRotation(out Vector3 pos, out Quaternion rot);

		void GetRimColliderPositionAndRotation(out Vector3 pos, out Quaternion rot);

		void SetTyreHealth(float value, float defaultValue);
	}
}
