using UnityEngine;

public abstract class BaseBehaviour2D : BaseBehaviour
{
	RectTransform _rt;
	public RectTransform RT
	{
		get
		{
			if (!_rt)
				_rt = GetComponent<RectTransform>();
			return _rt;
		}
	}
}