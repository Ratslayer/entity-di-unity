using Sirenix.OdinInspector;
using System;
using UnityEngine;
namespace BB
{
	[Serializable]
	public struct SerializedProviderField<T> : IProvider<T>
	{
		[SerializeField]
		SerializedProviderFieldType _type;
		[SerializeField, ShowIf(nameof(ShowValue))]
		T _value;
		[SerializeField, ShowIf(nameof(ShowValues))]
		T[] _values;
		[SerializeField, ShowIf(nameof(ShowAsset))]
		BaseProviderAsset<T> _asset;

		public readonly T GetValue()
			=> _type switch
			{
				SerializedProviderFieldType.Single => _value,
				SerializedProviderFieldType.OneOf => _values.RandomElement(),
				SerializedProviderFieldType.Asset when _asset => _asset.GetValue(),
				_ => default
			};

		bool ShowValue => _type == SerializedProviderFieldType.Single;
		bool ShowValues => _type == SerializedProviderFieldType.OneOf;
		bool ShowAsset => _type == SerializedProviderFieldType.Asset;
	}
}