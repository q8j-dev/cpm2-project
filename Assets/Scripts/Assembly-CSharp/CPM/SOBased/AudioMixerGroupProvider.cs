using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CPM.SOBased
{
	[CreateAssetMenu(menuName = "Scriptable/Mixer Group Provider")]
	public class AudioMixerGroupProvider : SerializedSingletonSO<AudioMixerGroupProvider>
	{
		[SerializeField]
		private Dictionary<string, AudioMixerGroup> _mixerGroups = new Dictionary<string, AudioMixerGroup>();

		[SerializeField]
		private string _carMainVolume = "master";

		[SerializeField]
		private string _carMiscVolume = "carMiscVolume";

		[SerializeField]
		private string _voiceChatVolume = "voiceVolume";

		public static string CarMainVolumeVariable => SerializedSingletonSO<AudioMixerGroupProvider>.Instance._carMainVolume;

		public static string CarMiscVolumeVariable => SerializedSingletonSO<AudioMixerGroupProvider>.Instance._carMiscVolume;

		public static string VoiceChatVolumeVariable => SerializedSingletonSO<AudioMixerGroupProvider>.Instance._voiceChatVolume;

		public static AudioMixerGroup GetMixerGroup(string mixerName)
		{
			if (!SerializedSingletonSO<AudioMixerGroupProvider>.Instance._mixerGroups.ContainsKey(mixerName))
			{
				throw new UnityException("Mixer group not found");
			}
			return SerializedSingletonSO<AudioMixerGroupProvider>.Instance._mixerGroups[mixerName];
		}
	}
}
