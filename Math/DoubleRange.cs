using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace BB
{
	[Serializable, InlineProperty]
	public struct DoubleRange
	{
		[SerializeField, HorizontalGroup, LabelWidth(30)]
		double _min, _max;
		public DoubleRange(double min, double max)
		{
			_min = min;
			_max = max;
		}
		public readonly double GetRandom()
			=> RandomUtils.Range(_min, _max);
		public readonly double MapToRange(double p)
			=> p.MapToRange(_min, _max);
		public readonly double Clamp(double value)
			=> value.Clamp(_min, _max);
		public readonly double Clamp(double value, double offset)
			=> value.Clamp(_min + offset, _max + offset);
		public readonly double Size
			=> _max - _min;
		public readonly double Mid
			=> (_max + _min) * 0.5f;
		public readonly double Max => _max;
		public readonly double Min => _min;

		public static implicit operator DoubleRange(double minMax)
			=> new(minMax, minMax);
		public static implicit operator DoubleRange((double min, double max) v)
			=> new(v.min, v.max);
	}
}