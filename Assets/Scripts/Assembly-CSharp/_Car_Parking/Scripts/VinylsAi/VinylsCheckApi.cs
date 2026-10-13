using System;
using System.IO;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.VinylsAi
{
	[Preserve]
	[CreateAssetMenu(menuName = "Scriptable/VinylsCheck")]
	public class VinylsCheckApi : ScriptableObject
	{
		private const string ModelFolderURL = "https://olzhas1.b-cdn.net/iosModels/";

		private const int DefaultWebRequestTimeout = 300;

		private static readonly Color LogColor = new Color(0.83f, 0.6f, 1f);

		[SerializeField]
		private ModelAsset Model = default;

		[SerializeField]
		private int TextureSize = 244;

		[Header("EDITOR ONLY, TO TEST IN EDITOR WITHOUT REMOTE CONFIG VALUES")]
		[SerializeField]
		private bool EditorOnlyDontOverrideByRemote;

		public bool SecondMode = true;

		private Model _runtimeModel;

		[field: Header("If unsafe value more that that value, image is Unsafe")]
		[field: SerializeField]
		public float UnsafeLimitValue { get; private set; } = 0.5f;

		public bool Silent { get; set; }

		[Preserve]
		public VinylsCheckApi()
		{
		}

		private void Initialize()
		{
			if (Model == null)
			{
				Log("Model asset is not assigned!", isError: true);
				return;
			}
			_runtimeModel = ModelLoader.Load(Model);
			_runtimeModel.ProducerName = Model.name;
		}

		public async UniTask<float> GetProbability(RenderTexture rt)
		{
			if (_runtimeModel == null)
			{
				Log("Runtime model is not initialized. Call Initialize() first.", isError: true);
				return 0f;
			}
			Log("Model name is: " + _runtimeModel.ProducerName);
			await UniTask.Yield();
			if (!Silent)
			{
				Log("Start recalculate image ai: " + rt.name);
			}
			float startTime = Time.time;
			using Worker worker = new Worker(_runtimeModel, BackendType.CPU);
			Texture2D convertedTexture = rt.ToTexture2D();
			try
			{
				using Tensor input = TextureConverter.ToTensor(convertedTexture, TextureSize, TextureSize);
				worker.SetInput(0, input);
				float result = (await ExecuteModel(worker))[0];
				float timeOfOperation = Time.time - startTime;
				LogPerformanceMetrics(timeOfOperation, result);
				return result;
			}
			finally
			{
				if (convertedTexture != null)
				{
					UnityEngine.Object.Destroy(convertedTexture);
				}
			}
		}

		private async UniTask<float[]> ExecuteModel(Worker worker)
		{
			if (SecondMode)
			{
				return await ExecuteModelSecondMode(worker);
			}
			return await ExecuteModelFirstMode(worker);
		}

		private async UniTask<float[]> ExecuteModelFirstMode(Worker worker)
		{
			await worker.ScheduleIterable();
			if (!(worker.PeekOutput() is Tensor<float> tensor))
			{
				throw new InvalidOperationException("Model output is null or not a float tensor. Check output name/type.");
			}
			try
			{
				return tensor.DownloadToArray();
			}
			finally
			{
				tensor.Dispose();
			}
		}

		private async UniTask<float[]> ExecuteModelSecondMode(Worker worker)
		{
			worker.Schedule();
			Tensor<float> outputTensor = worker.PeekOutput() as Tensor<float>;
			if (outputTensor == null)
			{
				throw new InvalidOperationException("Model output is null or not a float tensor. Check output name/type.");
			}
			try
			{
				outputTensor.ReadbackRequest();
				await UniTask.WaitUntil(() => outputTensor.IsReadbackRequestDone());
				return outputTensor.DownloadToArray();
			}
			finally
			{
				outputTensor.Dispose();
			}
		}

		private void LogPerformanceMetrics(float timeOfOperation, float result)
		{
			Log($"AI GetProbability Time of checking: {timeOfOperation:F2} sec".Colored(LogColor));
			Log($"Second mode: {SecondMode}");
			if (!Silent)
			{
				Log($"AI Unsafe value: {result:F2}".Colored(LogColor));
			}
		}

		public void ApplyValues(ConfigValuesStorage configRemote)
		{
			Log("Vinyls checker settings overrided by Remote config values".Colored(Color.red));
			UnsafeLimitValue = configRemote.unsafeImagesThreshold;
			TextureSize = configRemote.textureSize;
			Initialize();
		}

		public async UniTask CheckAndUpdateModel(string expectedModelName)
		{
			Log("CheckAndUpdateModel expectedModelName = " + expectedModelName);
			if (_runtimeModel != null && _runtimeModel.ProducerName == expectedModelName)
			{
				return;
			}
			string localPath = Path.Combine(Application.persistentDataPath, expectedModelName + ".onnx");
			try
			{
				Log("CheckAndUpdateModel: localPath = " + localPath);
				if (!File.Exists(localPath))
				{
					await DownloadAndSaveModel(expectedModelName, localPath);
				}
				LoadModel(localPath, expectedModelName);
			}
			catch (Exception arg)
			{
				Log($"Failed to check and update model: {arg}", isError: true);
			}
		}

		private async UniTask DownloadAndSaveModel(string modelName, string savePath)
		{
			Log("DownloadAndSaveModel expectedModelName = " + modelName + ", path = " + savePath);
			string uri = "https://olzhas1.b-cdn.net/iosModels/" + modelName + ".onnx";
			using UnityWebRequest request = UnityWebRequest.Get(uri);
			request.timeout = 300;
			await request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.Success)
			{
				Log("Error downloading model: " + request.error, isError: true);
				return;
			}
			byte[] data = request.downloadHandler.data;
			if (data == null || data.Length == 0)
			{
				Log("Downloaded model data is empty", isError: true);
				return;
			}
			await File.WriteAllBytesAsync(savePath, data);
		}

		private void LoadModel(string path, string modelName)
		{
			_runtimeModel = ModelLoader.Load(path);
			Log("Downloaded ProducerName: " + _runtimeModel.ProducerName + " outputs: " + _runtimeModel.outputs.Beautify());
			_runtimeModel.ProducerName = modelName;
			Log("Model loaded and worker created.");
		}

		private void Log(string message, bool isError = false)
		{
		}
	}
}
