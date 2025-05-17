using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace BB
{
	[Serializable, InlineProperty]
	public struct IntRange
	{
		[SerializeField, HorizontalGroup, LabelWidth(30)]
		int _min, _max;
		public IntRange(int min, int max)
		{
			_min = min;
			_max = max;
		}
		public readonly int GetRandom()
			=> RandomUtils.Range(_min, _max + 1);
		public readonly int Clamp(int value)
			=> Mathf.Clamp(value, _min, _max);
		public readonly int Clamp(int value, int offset)
			=> Mathf.Clamp(value, _min + offset, _max + offset);
		public readonly int Size
			=> _max - _min;
		public readonly int Mid
			=> (_max + _min) / 2;
		public readonly float Max => _max;
		public readonly float Min => _min;

		public static implicit operator IntRange(int minMax)
			=> new(minMax, minMax);
		public static implicit operator IntRange((int min, int max) v)
			=> new(v.min, v.max);
	}
}