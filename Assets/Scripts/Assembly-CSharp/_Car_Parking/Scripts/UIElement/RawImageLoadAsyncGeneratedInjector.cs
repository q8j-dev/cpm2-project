using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.CDN;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class RawImageLoadAsyncGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RawImageLoadAsync cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RawImageLoadAsync obj = (RawImageLoadAsync)instance;
			ICDNDownloader cdnDownloader = (ICDNDownloader)resolver.ResolveOrParameter(typeof(ICDNDownloader), "cdnDownloader", parameters);
			obj.Construct(cdnDownloader);
		}
	}
}
