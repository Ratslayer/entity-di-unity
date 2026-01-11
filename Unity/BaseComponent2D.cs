using UnityEngine;

public abstract class BaseComponent2D : BaseComponent
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