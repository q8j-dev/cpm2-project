using CPM.Scripts.VinylDrawer;
using UnityEngine;

public interface IVinylDrawPerformer
{
	MonoBehaviour ComponentToCheck { get; }

	int GetVinylsCount(VinylsType type, bool isFrontPlate);

	void RenderVinyls(VinylsType type, bool isFrontPlate);
}
