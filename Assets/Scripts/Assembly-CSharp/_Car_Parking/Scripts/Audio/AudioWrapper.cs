using AK.Wwise;
using UnityEngine;

namespace _Car_Parking.Scripts.Audio
{
	public static class AudioWrapper
	{
		public static void SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID)
		{
			AkSoundEngine.SetRTPCValue(in_rtpcID, in_value, in_gameObjectID);
		}

		public static void SetRTPCValue(string in_pszRtpcName, float in_value)
		{
			AkSoundEngine.SetRTPCValue(in_pszRtpcName, in_value);
		}

		public static void SetRTPCValue(uint in_rtpcID, float in_value)
		{
			AkSoundEngine.SetRTPCValue(in_rtpcID, in_value);
		}

		public static uint GetId(string name)
		{
			return AkSoundEngine.GetIDFromString(name);
		}

		public static uint PostEvent(string eventName, GameObject go)
		{
			return AkSoundEngine.PostEvent(eventName, go);
		}

		public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback callback, object in_pCookie)
		{
			return AkSoundEngine.PostEvent(in_pszEventName, in_gameObjectID, in_uFlags, callback, in_pCookie);
		}

		public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback callback, object in_pCookie)
		{
			return AkSoundEngine.PostEvent(in_eventID, in_gameObjectID, in_uFlags, callback, in_pCookie);
		}

		public static uint PostEvent(uint eventId, GameObject go)
		{
			return AkSoundEngine.PostEvent(eventId, go);
		}

		public static void PostTrigger(string trigger, GameObject go)
		{
			AkSoundEngine.PostTrigger(trigger, go);
		}

		public static void PostTrigger(uint triggerId, GameObject go)
		{
			AkSoundEngine.PostTrigger(triggerId, go);
		}

		public static void SetSwitch(string group, string state, GameObject go)
		{
			AkSoundEngine.SetSwitch(group, state, go);
		}

		public static void SetSwitch(uint group, uint state, GameObject go)
		{
			AkSoundEngine.SetSwitch(group, state, go);
		}

		public static void PostProcess(AK.Wwise.Event evt, GameObject go)
		{
			evt?.Post(go);
		}

		public static void StopAll(GameObject go)
		{
			AkSoundEngine.StopAll(go);
		}

		public static void StopPlayingId(uint playingId, int fadeOutMs = 0)
		{
			AkSoundEngine.StopPlayingID(playingId, fadeOutMs, AkCurveInterpolation.AkCurveInterpolation_Log3);
		}

		public static bool IsInitialized()
		{
			return AkSoundEngine.IsInitialized();
		}

		public static AKRESULT SetState(uint in_stateGroup, uint in_state)
		{
			return AkSoundEngine.SetState(in_stateGroup, in_state);
		}

		public static AKRESULT SetState(string in_pszStateGroup, string in_pszState)
		{
			return AkSoundEngine.SetState(in_pszStateGroup, in_pszState);
		}

		public static uint GetEventIDFromPlayingID(uint in_playingID)
		{
			return AkSoundEngine.GetEventIDFromPlayingID(in_playingID);
		}

		public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID)
		{
			return AkSoundEngine.ResetRTPCValue(in_rtpcID, in_gameObjectID);
		}

		public static AKRESULT GetPlayingIDsFromGameObject(ulong in_GameObjId, ref uint io_ruNumIDs, uint[] out_aPlayingIDs)
		{
			return AkSoundEngine.GetPlayingIDsFromGameObject(in_GameObjId, ref io_ruNumIDs, out_aPlayingIDs);
		}

		public static AKRESULT GetPlayingIDsFromGameObject(GameObject in_GameObjId, ref uint io_ruNumIDs, uint[] out_aPlayingIDs)
		{
			return AkSoundEngine.GetPlayingIDsFromGameObject(in_GameObjId, ref io_ruNumIDs, out_aPlayingIDs);
		}
	}
}
