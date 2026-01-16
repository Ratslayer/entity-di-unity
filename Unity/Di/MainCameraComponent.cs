using UnityEngine;
namespace BB
{
	public sealed class MainCameraComponent : BaseComponent
    {
        public Camera _mainCamera;
		private void Awake()
		{
            new TransformOperation { DoNotDestroyOnLoad = true }.Apply(_mainCamera);
		}
	}

}
