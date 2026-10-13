using Cysharp.Threading.Tasks;

public class Addressable<T>
{
	public delegate UniTask<bool> OnReleaseDelegate();

	public T Result { get; set; }

	public event OnReleaseDelegate OnRelease;

	public async UniTask<bool> Release()
	{
		return await OnRelease();
	}
}
