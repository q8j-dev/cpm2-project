using System;

namespace _Car_Parking.Scripts.Audio.Dummy
{
	public class AkCallbackInfoDummy : IDisposable
	{
		private IntPtr swigCPtr;

		protected bool swigCMemOwn;

		public IntPtr pCookie => swigCPtr;

		public ulong gameObjID => 0uL;

		internal AkCallbackInfoDummy(IntPtr cPtr, bool cMemoryOwn)
		{
			swigCMemOwn = cMemoryOwn;
			swigCPtr = cPtr;
		}

		internal static IntPtr getCPtr(AkCallbackInfoDummy obj)
		{
			return obj?.swigCPtr ?? IntPtr.Zero;
		}

		internal virtual void setCPtr(IntPtr cPtr)
		{
			Dispose();
			swigCPtr = cPtr;
		}

		~AkCallbackInfoDummy()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
