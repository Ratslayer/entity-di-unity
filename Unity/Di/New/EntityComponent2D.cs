using BB.Di;
using UnityEngine;
namespace BB
{
	[RequireComponent(typeof(EntityGameObject2D))]
    public abstract class EntityComponent2D : BaseEntityComponent
    {
        [Get]
        public RectTransform Rt { get; private set; }
    }
}