using System.Collections;
using UnityEngine;

public static class AudioSourceExtention
{
	public static IEnumerator CrossFade(this AudioSource audioSource, AudioClip newSound, float finalVolume, float fadeTime)
	{
		yield return audioSource.FadeOut(fadeTime);
		audioSource.clip = newSound;
		yield return audioSource.FadeIn(fadeTime, finalVolume);
	}

	public static IEnumerator FadeOut(this AudioSource audioSource, float fadeTime)
	{
		float startVolume = audioSource.volume;
		while (audioSource.volume > 0f)
		{
			audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
			yield return null;
		}
		audioSource.Stop();
		audioSource.volume = 0f;
	}

	public static IEnumerator FadeIn(this AudioSource audioSource, float fadeTime, float finalVolume)
	{
		float startVolume = 0.2f;
		audioSource.volume = 0f;
		audioSource.Play();
		while (audioSource.volume < finalVolume)
		{
			audioSource.volume += startVolume * Time.deltaTime / fadeTime;
			yield return null;
		}
		audioSource.volume = finalVolume;
	}
}
