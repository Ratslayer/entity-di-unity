using BB.Di;
using System;
namespace BB
{
    public sealed class PooledEventHandler<T> : ProtectedPooledObject<PooledEventHandler<T>>, IEventHandler<T>
    {
        Action<T> _action;
        public static PooledEventHandler<T> GetPooled(Action<T> action)
        {
            var handler = GetPooledInternal();
            handler._action = action;
            return handler;
        }
        public void OnEvent(T action) => _action.Invoke(action);
        public override void Dispose()
        {
            _action = null;
            base.Dispose();
        }
    }
}