using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Person;

public class RagdollTrigger : MonoBehaviour
{
	private static readonly uint BodyHitSwitch = AudioWrapper.GetId("P_Body_Hit");

	private static readonly uint HardHit = AudioWrapper.GetId("hard_hit");

	private static readonly uint SoftHit = AudioWrapper.GetId("soft_hit");

	private static readonly uint BodyFallHitEvent = AudioWrapper.GetId("Play_P_Body_Fall_Hit");

	private const float HardHitImpulse = 8f;

	private const float MinHitInterval = 0.3f;

	private const float MinBodySpeed = 1f;

	public RagdollCPM ragdollCpm;

	public float timeStartSount;

	private void OnCollisionEnter(Collision other)
	{
		if (ragdollCpm == null || !ragdollCpm.isRagdoll || Time.time - timeStartSount <= 0.3f || !ragdollCpm.mainRigidbody || ragdollCpm.mainRigidbody.velocity.magnitude <= 1f)
		{
			return;
		}
		timeStartSount = Time.time;
		if (!ragdollCpm.IsAudible())
		{
			return;
		}
		AudioWrapper.SetSwitch(BodyHitSwitch, (other.impulse.magnitude > 8f) ? HardHit : SoftHit, base.gameObject);
		if (base.gameObject.activeInHierarchy)
		{
			SoundBankLoadService soundBankLoadService = (V.CoreGameManager ? V.CoreGameManager.SoundBankLoadService : null);
			if (soundBankLoadService != null)
			{
				soundBankLoadService.PostEvent(BodyFallHitEvent, SoundBankType.SfxMain, base.gameObject).Forget();
			}
			else
			{
				AudioWrapper.PostEvent(BodyFallHitEvent, base.gameObject);
			}
		}
		ragdollCpm.PlaySoundGrunt();
	}
}
