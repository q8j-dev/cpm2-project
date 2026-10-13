using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace _Car_Parking.Scripts.Steam
{
	public class QuitDiagnostics : MonoBehaviour
	{
		private static QuitDiagnostics _instance;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap()
		{
			if (!(_instance != null))
			{
				GameObject obj = new GameObject("[QuitDiagnostics]");
				UnityEngine.Object.DontDestroyOnLoad(obj);
				_instance = obj.AddComponent<QuitDiagnostics>();
				Application.quitting += OnQuitting;
				UnityEngine.Debug.Log("[QuitDiag] hooked Application.quitting");
			}
		}

		private static void OnQuitting()
		{
			UnityEngine.Debug.Log("[QuitDiag] Application.quitting fired");
			DumpThreads("at-quitting");
		}

		private static void DumpThreads(string tag)
		{
			try
			{
				Process currentProcess = Process.GetCurrentProcess();
				ProcessThreadCollection threads = currentProcess.Threads;
				StringBuilder stringBuilder = new StringBuilder(2048);
				stringBuilder.AppendLine($"[QuitDiag] {tag}: pid={currentProcess.Id} threadCount={threads.Count}");
				foreach (ProcessThread item in threads)
				{
					string arg = "";
					try
					{
						if (item.ThreadState == ThreadState.Wait)
						{
							arg = $" waitReason={item.WaitReason}";
						}
					}
					catch
					{
					}
					stringBuilder.AppendLine($"  [Thread] id={item.Id} state={item.ThreadState}{arg}");
				}
				UnityEngine.Debug.Log(stringBuilder.ToString());
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("[QuitDiag] DumpThreads failed: " + ex.GetType().Name + ": " + ex.Message);
			}
		}
	}
}
