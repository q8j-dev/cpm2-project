using UnityEngine;

namespace MTAssets.SkinnedMeshCombiner
{
	public class SkinnedMeshBonesManager : MonoBehaviour
	{
		[HideInInspector]
		public SkinnedMeshRenderer anotherBonesHierarchyCurrentInUse;

		private bool isValidTargetSkinnedMeshRendererBonesHierarchy(SkinnedMeshRenderer targetMeshRenderer, bool launchLogs)
		{
			bool flag = true;
			SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
			if (component == null)
			{
				flag = false;
			}
			if (targetMeshRenderer == null)
			{
				flag = false;
			}
			if (component.bones.Length != targetMeshRenderer.bones.Length)
			{
				flag = false;
			}
			if (launchLogs)
			{
				string message = "It is not possible for this Skinned Mesh Renderer to use the Skinned Mesh Renderer \"" + targetMeshRenderer.gameObject.name + "\" bone hierarchy, as the two hierarchies are not identical. Both Skinned Mesh Renderers must have an identical bone hierarchy to make it possible for this mesh to be animated by the bones of the desired Skinned Mesh Renderer.";
				if (!flag)
				{
					Debug.Log(message);
				}
			}
			return flag;
		}

		public bool UseAnotherBoneHierarchyForAnimateThis(SkinnedMeshRenderer meshRendererBonesToUse, bool useRootBoneToo)
		{
			if (!isValidTargetSkinnedMeshRendererBonesHierarchy(meshRendererBonesToUse, launchLogs: true))
			{
				return false;
			}
			SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
			component.bones = meshRendererBonesToUse.bones;
			if (useRootBoneToo)
			{
				component.rootBone = meshRendererBonesToUse.rootBone;
			}
			base.gameObject.transform.parent = meshRendererBonesToUse.transform.parent;
			anotherBonesHierarchyCurrentInUse = meshRendererBonesToUse;
			return true;
		}
	}
}
