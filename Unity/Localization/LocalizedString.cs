using UnityEngine;

public sealed class LocalizedString : StringAsset
{
	[SerializeField] string _value;
	public override string Value => _value;
}
