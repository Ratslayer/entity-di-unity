using BB;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using BB.Di;

[CustomEditor(typeof(EntityGameObject))]
public sealed class EntityGameObjectEditor : BaseEditor
{
	bool _binds;
	ContainerDisplayData _displayData;
	protected override void InspectorGUI()
	{
		base.InspectorGUI();
		var t = (EntityGameObject)target;
		var entity = t.Entity._ref as IEntityDetails;
		if (entity is null)
			return;
		EditorGUILayout.LabelField($"Spawn Id: {entity.CurrentSpawnId} Actual Id: {t.Entity._id}");
		EditorGUILayout.LabelField($"State: {entity.State}");
		EditorGUILayout.LabelField($"Parent: {GetName(entity.Parent)}");
		EditorGUILayout.LabelField($"Attached to: " +
			$"{(t.Entity.AttachedToEntity ? GetName(t.Entity._ref) : "")}");
		_displayData ??= new(entity);
		using (LayoutUtils.Foldout("Binds", ref _binds))
		{
			if (_binds)
				EditorEntityUtils.DrawContainerEditor(_displayData);
		}
		static string GetName(IEntity e) => e is null ? "--"
			: $"{e.Name} - {e.CurrentSpawnId}";
	}
}
public sealed class ContainerDisplayData
{
	public readonly IEntityDetails _container;
	public readonly List<ContainerBindData> _datas;
	public bool
		_eventsFoldout,
		_systemsFoldout,
		_variablesFoldout,
		_behavioursFoldout,
		_othersFoldout,
		_statesFoldout;
	public ContainerDisplayData(IEntityDetails container)
	{
		_container = container;
		_datas = EditorEntityUtils.BuildDatas(_container);
	}
}
public sealed record ContainerBindData(
		Type ContractType,
		Type InstanceType,
		object Instance)
{
	public Type _eventType;
	public readonly List<string> _tags = new();
}
public static class EditorEntityUtils
{
	public const string
		System = "EntitySystem",
		Instance = "Instance",
		Event = "Event",
		Behaviour = "Behaviour",
		Variable = "Variable",
		State = "State";
	public static void DrawContainerEditor(ContainerDisplayData display)
	{
		EditorGUILayout.LabelField($"Num Elements: {display._datas.Count}");
		Foldout(
			ref display._systemsFoldout,
			"Systems",
			System,
			Contract);
		Foldout(
			ref display._eventsFoldout,
			"Events",
			Event,
			EventDisplay);
		Foldout(
			ref display._behavioursFoldout,
			"Behaviours",
			Behaviour,
			Contract);
		Foldout(
			ref display._variablesFoldout,
			"Variables",
			Variable,
			DrawVariable);
		Foldout(
			ref display._statesFoldout,
			"States",
			State,
			Value);
		Foldout(
			ref display._othersFoldout,
			"Others",
			Instance,
			Value);

		void Foldout(ref bool foldout,
			string name,
			string tag,
			Action<ContainerBindData> process)
		{
			var datas = GetDatas(tag);
			if (datas.Count() == 0)
				return;
			using (LayoutUtils.Foldout(name, ref foldout))
			{
				if (foldout)
					foreach (var data in datas)
						process(data);
			}
		}
		IEnumerable<ContainerBindData> GetDatas(string tag)
		{
			foreach (var data in display._datas)
				if (data._tags.Contains(tag))
					yield return data;
		}

	}
	static void Contract(ContainerBindData data)
			=> EditorGUILayout.LabelField(data.ContractType.Name);
	static void EventDisplay(ContainerBindData data)
			=> EditorGUILayout.LabelField(data.ContractType.GenericTypeArguments[0].Name);
	static void Value(ContainerBindData data)
		=> EditorGUILayout.LabelField($"{data.ContractType.Name}: {data.Instance}");
	static void DrawVariable(ContainerBindData data)
	{
		if (!ReflectionUtils.TryGetInheritedType(data.InstanceType, typeof(Variable<,>), out var genType))
		{
			Value(data);
			return;
		}
		var valueType = genType.GetGenericArguments()[1];
		var valueProperty = data.InstanceType.GetProperty("Value");
		var value = valueProperty.GetValue(data.Instance);
		if (typeof(Enum).IsAssignableFrom(valueType))
		{
			var enumValue = (Enum)value;
			using (LayoutUtils.Horizontal)
			{
				GUILayout.Label(data.ContractType.Name, GUILayout.Width(300));
				var newValue = EditorGUILayout.EnumPopup(enumValue);
				valueProperty.SetValue(data.Instance, newValue);
			}
			//var newValue = EditorGUILayout.EnumPopup(data.ContractType.Name, enumValue);
			return;
		}
		Value(data);
	}
	public static List<ContainerBindData> BuildDatas(IEntityDetails entity)
	{
		var result = new List<ContainerBindData>();
		foreach (var (contractType, instance) in entity.GetElements())
		{
			var instanceType = instance.GetType();
			var data = new ContainerBindData(
				contractType,
				instanceType,
				instance);
			result.Add(data);
			AddTag<IVariable>(Variable);
			AddTag<EntityBehaviour>(Behaviour);
			AddTag<IStackValue>(State);
			bool isEvent = instanceType.GetInterfaces().Any(x =>
				   x.IsGenericType &&
				   x.GetGenericTypeDefinition() == typeof(IEvent<>));
			if (isEvent)
			{
				data._eventType = instanceType.GetGenericArguments()[0];
				data._tags.Add(Event);
			}
			if (data._tags.Count == 0)
				data._tags.Add(Instance);
			void AddTag<T>(string tag)
			{
				if (typeof(T).IsAssignableFrom(instanceType))
					data._tags.Add(tag);
			}
		}
		return result;
	}

}