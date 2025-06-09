using System;
using UnityEngine;

public sealed class LocalizedString : StringAsset
{
	[SerializeField] string _value;
	public override string Value => _value;
}
[Serializable]
public struct SerializedString
{
	public enum Type
	{
		String,
		Key,
		Asset
	}
	public string _value;
}
