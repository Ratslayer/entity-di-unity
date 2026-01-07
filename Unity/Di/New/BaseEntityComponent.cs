using BB.Di;
using System.Reflection;
namespace BB
{
	public abstract class BaseEntityComponent : BaseComponent, IEntityProvider, IEntityBehaviour
    {
        public virtual void Install(IDiContainer container)
        {
            container.Component(GetType());
        }
        bool _getAttributeRead = false;
        [Get]
        BaseEntityGameObject _gameObject;
        public Entity Entity => _gameObject.Entity;
        public void Init()
        {
            if (_getAttributeRead)
                return;
            _getAttributeRead = true;
            var members = ReflectionUtils.GetAllMembersWithAttribute<GetAttribute>(GetType());
            foreach (var info in members)
                switch (info)
                {
                    case PropertyInfo prop:
                        if (prop.CanWrite)
                            prop.SetValue(this, GetComponent(prop.PropertyType));
                        break;
                    case FieldInfo field:
                        field.SetValue(this, GetComponent(field.FieldType));
                        break;
                }
        }
    }
}