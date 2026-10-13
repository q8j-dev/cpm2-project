using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer.Unity;

namespace _Car_Parking.Scripts.DeepLink
{
	public class DeepLinkProvider : IInitializable
	{
		private readonly Queue<string> _deepLinkUrls = new Queue<string>();

		private HashSet<IDeepLinkConsumer> _consumers = new HashSet<IDeepLinkConsumer>();

		public void Subscribe(IDeepLinkConsumer consumer)
		{
			_consumers.Add(consumer);
		}

		public void Unsubscribe(IDeepLinkConsumer consumer)
		{
			_consumers.Remove(consumer);
		}

		public void Initialize()
		{
			Application.deepLinkActivated -= DeepLinkActivatedHandler;
			Application.deepLinkActivated += DeepLinkActivatedHandler;
			string absoluteURL = Application.absoluteURL;
			if (!string.IsNullOrEmpty(absoluteURL))
			{
				DeepLinkActivatedHandler(absoluteURL);
			}
		}

		private void DeepLinkActivatedHandler(string url)
		{
			_deepLinkUrls.Enqueue(url);
			RequestDeepLink();
		}

		public void RequestDeepLink()
		{
			if (_deepLinkUrls.Count != 0)
			{
				string linkToConsume = _deepLinkUrls.Peek();
				if (_consumers.Any((IDeepLinkConsumer consumer) => consumer.TryConsumeDeepLink(new DeepLinkData(linkToConsume))))
				{
					_deepLinkUrls.Dequeue();
				}
			}
		}
	}
}
