using BB.Di;
using System;
using System.Reflection;
namespace BB
{
    public sealed class OnClickAttribute : Attribute
    {

        public OnClickAttribute(string fieldOrPropertyName)
        {

        }
    }
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

            var getMembers = ReflectionUtils.GetAllMembersWithAttribute<GetAttribute>(GetType());
            foreach (var info in getMembers)
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