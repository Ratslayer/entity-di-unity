using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace BB
{
	[Serializable, InlineProperty]
	public struct FloatRange
	{
		[SerializeField, HorizontalGroup, LabelWidth(30)]
		float _min, _max;
		public FloatRange(float min, float max)
		{
			_min = min;
			_max = max;
		}
		public readonly float GetRandom()
			=> RandomUtils.Range(_min, _max);
		public readonly float MapToRange(float p)
			=> p.MapToRange(_min, _max);
		public readonly float Clamp(float value)
			=> Mathf.Clamp(value, _min, _max);
		public readonly float Clamp(float value, float offset)
			=> Mathf.Clamp(value, _min + offset, _max + offset);
		public readonly float Size
			=> _max - _min;
		public readonly float Mid
			=> (_max + _min) * 0.5f;
		public readonly float Max => _max;
		public readonly float Min => _min;

		public static implicit operator FloatRange(float minMax)
			=> new(minMax, minMax);
		public static implicit operator FloatRange((float min, float max) v)
			=> new(v.min, v.max);
	}
}