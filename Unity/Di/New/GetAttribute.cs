using System;
namespace BB.Di
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class GetAttribute : Attribute { }
}