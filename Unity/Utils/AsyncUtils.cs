using System;
using Cysharp.Threading.Tasks;
namespace BB
{
	public static class AsyncUtils
	{
		public static void InvokeAtEndOfFrame(Action action)
			=> InvokeAtEndOfFrameInternal(action).Forget();
		static async UniTaskVoid InvokeAtEndOfFrameInternal(Action action)
		{
			await UniTask.WaitForEndOfFrame();
			action();
		}
		
	}
}