using _Car_Parking.Scripts.Free_drive;

namespace _Car_Parking.Scripts.EventSystem
{
	public interface IEventZone
	{
		void Init(EventType eventType, int zoneIndex, SceneType sceneType);

		void OnCollect();

		void CreateCollectables();
	}
}
