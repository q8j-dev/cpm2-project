using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Scopes;

public class tournirmentProgressBar : MonoBehaviour
{
	public Slider slider;

	public Transform[] labels;

	private int size;

	public float sizeMultiply;

	public float sizeMultiplyLocal;

	public int range;

	public Color highlited = Color.yellow;

	private Color firstColor;

	private DragRacingController _dragRacingController;

	[Inject]
	public void Construct(OptionalDependency<DragRacingController> dragRacingController)
	{
		_dragRacingController = dragRacingController.Value;
	}

	private void Awake()
	{
		Reset();
		DragRacingController.Restarted += Reset;
	}

	private void Reset()
	{
		slider.value = 0f;
	}

	private void OnEnable()
	{
		UpdateScrollAsync(this.GetCancellationTokenOnDestroy()).Forget();
	}

	private void OnDestroy()
	{
		DragRacingController.Restarted -= Reset;
	}

	private void setTextSize()
	{
	}

	private async UniTask UpdateScrollAsync(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			float target = (float)_dragRacingController.RaceWon * 0.2f;
			while (target - slider.value > 0f)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.009999999776482582), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				setTextSize();
				slider.value += 0.005f;
			}
			int currentIndex = (int)(slider.value * 5f);
			int count = 5;
			bool active = false;
			labels[currentIndex].DOScale(1.4f, 0.5f).OnComplete(() =>
			{
				labels[currentIndex].DOScale(1f, 0.5f);
			});
			while (count > 0)
			{
				active = !active;
				count--;
				await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}
}
