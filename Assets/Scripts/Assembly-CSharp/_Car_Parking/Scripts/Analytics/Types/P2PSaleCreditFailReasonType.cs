using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum P2PSaleCreditFailReasonType
	{
		[EnumMember(Value = "VALIDATION_ERROR")]
		ValidatorError = 1,
		[EnumMember(Value = "PHOTON_DESYNC")]
		PhotonDesync = 2,
		[EnumMember(Value = "SELLER_OFFLINE")]
		SellerOffline = 3,
		[EnumMember(Value = "FIREBASE_ERROR")]
		FirebaseError = 4,
		[EnumMember(Value = "INTERNAL_ERROR")]
		InternalError = 5,
		[EnumMember(Value = "UNKNOWN")]
		Unknown = 6
	}
}
