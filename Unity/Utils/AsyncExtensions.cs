using System;
using System.Threading;
using Cysharp.Threading.Tasks;
namespace BB
{
	public static class AsyncExtensions
	{
		public static CancellationTokenSource Link(
			this CancellationTokenSource s1,
			CancellationTokenSource s2)
		{
			if (s1 is null)
				return s2;
			if (s2 is null)
				return s1;
			return CancellationTokenSource.CreateLinkedTokenSource(s1.Token, s2.Token);
		}
		public static CancellationTokenSource Link(
			this CancellationTokenSource s1,
			CancellationTokenSource s2,
			CancellationToken ct)
		{
			if (ct == default)
				return s1.Link(s2);
			if (s1 is null)
				return s2;
			if (s2 is null)
				return s1;
			return CancellationTokenSource.CreateLinkedTokenSource(s1.Token, s2.Token, ct);
		}
		public static void CancelAfter(this CancellationTokenSource token, UniTask task)
		{
			Cancel(token, task).Forget();
			static async UniTaskVoid Cancel(CancellationTokenSource token, UniTask task)
			{
				await task;
				token.Cancel();
			}
		}
		public static async UniTask<T> WaitForEvent<T>(
			this IEvent<T> e,
			Predicate<T> predicate,
			CancellationToken ct)
		{
			var task = await WaitForEvent(e, null, predicate, ct);
			return task._event;
		}
		public static async UniTask<EventAsyncResult<T>> WaitForEventWithTimeout<T>(
			this IEvent<T> e,
			float seconds,
			Predicate<T> predicate,
			CancellationToken ct)
		{
			if (seconds <= 0)
			{
				var result = await WaitForEvent(e, predicate, ct);
				return new(false, result);
			}
			var tokenSource = new CancellationTokenSource();
			tokenSource.CancelAfterSlim(TimeSpan.FromSeconds(seconds));
			return await WaitForEvent(e, tokenSource, predicate, ct);
		}
		static async UniTask<EventAsyncResult<T>> WaitForEvent<T>(
			this IEvent<T> e,
			CancellationTokenSource timeoutSource,
			Predicate<T> eventPredicate,
			CancellationToken externalCancelationToken)
		{
			T eventValue = default;

			void OnEventRaised(T t) => eventValue = t;
			e.Subscribe(OnEventRaised);
			while (true)
			{
				var source = timeoutSource.Link(
					e.CancellationTokenSource,
					externalCancelationToken);
				await UniTask.Never(source.Token).SuppressCancellationThrow();

				if (externalCancelationToken.IsCancellationRequested)
					throw new OperationCanceledException();
				if (timeoutSource is not null && timeoutSource.IsCancellationRequested)
					return ToResult(false);
				if (eventPredicate is null || eventPredicate(eventValue))
					return ToResult(true);
				EventAsyncResult<T> ToResult(bool worked)
				{
					e.Unsubscribe(OnEventRaised);
					return new(worked, eventValue);
				}
			}
		}
	}
}