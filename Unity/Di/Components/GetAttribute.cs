using System;
namespace BB
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class GetAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SystemAttribute : Attribute { }
}