using System;
using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace CPM.CarVisualModel
{
	[Serializable]
	public struct BodyKitMount
	{
		[Tooltip("Which complex kit family: Spoiler / AirIntakeHood / AirIntakeRoof / Trim")]
		public BodyKitType Type;

		[Tooltip("Kit id parsed from the mount transform name (Utils.ExtractIntFromName)")]
		public int Id;

		[Tooltip("RawLocal: mount localPosition as authored. Legacy: mount position relative to root")]
		public Vector3 LocalPosition;

		[Tooltip("RawLocal: mount localRotation as authored. Legacy: rotation relative to root")]
		public Vector3 LocalEuler;

		[Tooltip("RawLocal: mount localScale as authored (gameplay applies it to the kit). Legacy: lossyScale, unused")]
		public Vector3 LocalScale;

		[Tooltip("True = raw local data (new extraction, scale honored); false = legacy root-relative data")]
		public bool RawLocal;
	}
}
