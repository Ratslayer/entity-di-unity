using Sirenix.OdinInspector;
using BB.Di;

public sealed class EntityRedirectBehaviour : BaseBehaviour
{
	[Required]
	public EntityBehaviour _redirectTo;
}
