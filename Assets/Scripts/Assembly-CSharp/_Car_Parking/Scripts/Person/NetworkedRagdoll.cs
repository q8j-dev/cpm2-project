using System.Globalization;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

namespace _Car_Parking.Scripts.Person
{
	public static class NetworkedRagdoll
	{
		private const byte EnableRagdollRpcCode = 59;

		public static bool Enable(GameObject person, Vector3 force, bool autoStandUp = true)
		{
			if (!person || !person.TryGetComponent<DriverActionController>(out var component))
			{
				return false;
			}
			RagdollCPM ragdollCpm = component.ragdollCpm;
			if (!ragdollCpm || ragdollCpm.isRagdoll)
			{
				return false;
			}
			if (PhotonNetwork.IsConnected && person.TryGetComponent<PhotonView>(out var component2))
			{
				Send(component2.ViewID, force, person.transform.localPosition);
			}
			bool autoStandUp2 = autoStandUp;
			ragdollCpm.RagdollEnabler(force, default, autoStandUp2, default, autoStandUpPhysics: false).Forget();
			return true;
		}

		private static void Send(int personViewId, Vector3 force, Vector3 localPosition)
		{
			if ((bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null)
			{
				MultiPlayer multiPlayer = V.CoreGameManager.FreeDriveProvider.MultiPlayer;
				if ((bool)multiPlayer)
				{
					Vector3 vector = localPosition * 10f;
					string name = string.Join("?", Round(force.x), Round(force.y), Round(force.z), Round(vector.x), Round(vector.y), Round(vector.z));
					multiPlayer.SendValueStr(59, personViewId.ToString(), name);
				}
			}
		}

		private static string Round(float value)
		{
			return Mathf.RoundToInt(value).ToString(CultureInfo.InvariantCulture);
		}
	}
}
