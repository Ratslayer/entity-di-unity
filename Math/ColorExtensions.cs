using UnityEngine;

namespace BB
{
	public static class ColorExtensions
	{
		public static bool Approximately(
			this Color c1,
			Color c2)
		=> c1.r.Approximately(c2.r)
			&& c1.g.Approximately(c2.g)
			&& c1.b.Approximately(c2.b)
			&& c1.a.Approximately(c2.a);
	}
}