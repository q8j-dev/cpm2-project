using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.ArtistTools.Aspects
{
	public interface IStartAspect : IAspect
	{
		UniTask StartImpl(TrackablesRegisterTool tool);
	}
}
