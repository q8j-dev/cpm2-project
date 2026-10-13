using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

public class SceneLoadingBgloader : MonoBehaviour
{
	private int asd = 1;

	public int SceneID;

	public Image loadingImg;

	public Text progressText;

	public Text loadingText;

	public Animator animator;

	private float TimedeltaTime;

	private void Start()
	{
		TimedeltaTime = Time.deltaTime;
	}

	private async UniTask AsyncLoadAsync(CancellationToken ct = default(CancellationToken))
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(SceneID);
		while (!operation.isDone)
		{
			float num = operation.progress / 0.9f;
			loadingImg.fillAmount = num;
			progressText.text = $"{num * 100f:0}%";
			await UniTask.Yield(ct);
		}
	}

	private void StartLoader()
	{
		if (asd == 1)
		{
			loadingText.text = new LocalizedString("LevelLoader.Loading");
			AsyncLoadAsync(this.GetCancellationTokenOnDestroy()).Forget();
			asd++;
		}
	}

	private void Update()
	{
		TimedeltaTime += Time.deltaTime;
		if (TimedeltaTime > 1f)
		{
			StartLoader();
		}
	}
}
