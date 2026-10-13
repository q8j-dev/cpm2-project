using UnityEngine;

public interface IMovableCollectible
{
	void MoveCollectible(Vector3 position, Quaternion rotation);

	void ChangeState(bool state);
}
