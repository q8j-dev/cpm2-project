using UnityEngine;
using UnityEngine.UI;

namespace MTAssets.SkinnedMeshCombiner
{
	public class ExampleAPI : MonoBehaviour
	{
		public Text statusL;

		public Text statusR;

		public Text statusM;

		public Text buttonL;

		public Text buttonR;

		public Text buttonM;

		public SkinnedMeshCombiner charL;

		public SkinnedMeshCombiner charR;

		public SkinnedMeshCombiner charM;

		private void Update()
		{
			if (charL.isMeshesCombined())
			{
				buttonL.text = "Undo Merge of This Model!";
				statusL.text = "<color=green>Meshes Combined</color>";
			}
			else
			{
				buttonL.text = "Combine This Model!";
				statusL.text = "<color=red>Meshes NOT Combined</color>";
			}
			if (charR.isMeshesCombined())
			{
				buttonR.text = "Undo Merge of This Model!";
				statusR.text = "<color=green>Meshes Combined</color>";
			}
			else
			{
				buttonR.text = "Combine This Model!";
				statusR.text = "<color=red>Meshes NOT Combined</color>";
			}
			if (charM.isMeshesCombined())
			{
				buttonM.text = "Undo Merge of This Model!";
				statusM.text = "<color=green>Meshes Combined</color>";
			}
			else
			{
				buttonM.text = "Combine This Model!";
				statusM.text = "<color=red>Meshes NOT Combined</color>";
			}
		}

		public void ButtonL()
		{
			if (charL.isMeshesCombined())
			{
				charL.UndoCombineMeshes(runUnityGc: true, runMonoIl2CppGc: true);
			}
			else
			{
				charL.CombineMeshes();
			}
		}

		public void ButtonR()
		{
			if (charR.isMeshesCombined())
			{
				charR.UndoCombineMeshes(runUnityGc: true, runMonoIl2CppGc: true);
			}
			else
			{
				charR.CombineMeshes();
			}
		}

		public void ButtonM()
		{
			if (charM.isMeshesCombined())
			{
				charM.UndoCombineMeshes(runUnityGc: true, runMonoIl2CppGc: true);
			}
			else
			{
				charM.CombineMeshes();
			}
		}

		public void OnDone()
		{
			Debug.Log("Meshes merged.");
		}

		public void OnUndo()
		{
			Debug.Log("Meshes unmerged.");
		}
	}
}
