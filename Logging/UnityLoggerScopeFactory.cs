using BB.Di;

namespace BB
{
    public sealed class UnityLoggerScopeFactory : ILoggerScopeFactory
    {
        public ILoggerScope GetScope()
        {
            return UnityLoggerScope.GetPooled();
        }

        public ILoggerScope GetScopeFromEntity(IEntity entity)
        {
            var scope = UnityLoggerScope.GetPooled();

            scope.AddToScope(LoggerConstants.EntityContextKey, entity.Name);
            if (entity.Has(out Root root))
                scope.AddToScope(UnityLoggerConstants.UnityObject, root.GameObject);
            if (entity.Has(out Root2D root2D))
                scope.AddToScope(UnityLoggerConstants.UnityObject, root2D.GameObject);

            return scope;
        }
    }
}