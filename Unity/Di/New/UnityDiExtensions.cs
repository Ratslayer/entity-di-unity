using System;
namespace BB.Di
{
	public static class UnityDiExtensions
    {
        public static void Component<T>(this IDiContainer container)
            => container.Component(typeof(T));
        public static void Component(this IDiContainer container, Type type)
        {
            container.AddComponent(new ComponentDiComponent(new()
            {
                ContractType = type,
                InstanceType = type,
            }));
        }
    }
}