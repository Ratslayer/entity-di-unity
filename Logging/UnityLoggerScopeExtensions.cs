namespace BB
{
    public static class UnityLoggerScopeExtensions
    {
        public static ILoggerScope WithUnityObject(this ILoggerScope scope, UnityEngine.Object unityObject)
        {
            scope.AddToScope(UnityLoggerConstants.UnityObject, unityObject);
            return scope;
        }

        public static ILoggerScope WithTarget(this ILoggerScope scope, in Entity entity)
        {
            scope.AddToScope(UnityLoggerConstants.Target, entity);
            return scope;
        }
    }
}