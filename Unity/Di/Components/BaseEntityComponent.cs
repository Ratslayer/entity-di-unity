using BB.Di;
using System;
using System.Linq;
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

            var childTypes = GetType()
                .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                .Where(t => t.HasAttribute<SystemAttribute>());
            foreach (var childType in childTypes)
                container.System(childType);
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

            var type = GetType();
            var getMembers = ReflectionUtils.GetAllMembersWithAttribute<GetAttribute>(type);
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