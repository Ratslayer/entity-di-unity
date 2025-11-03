using BB.Di;
using System;
namespace BB
{
	public readonly struct EntitySpawnContext2D
    {
        public InstallerAdapter2D Installer { get; init; }
        public TransformOperation2D? Transform { get; init; }
        public Entity? Parent { get; init; }
        public string SerializationName { get; init; }

        public Entity Spawn()
        {
            if (Installer.Prefab)
            {

                var entity = GameObjectSpawnUtils.CreateDespawnedGameObjectEntity(
                    Installer.Prefab.gameObject,
                    Parent);

                var root = entity.Require<Root>();

                Transform?.Apply(root);

                var token = GameObjectSpawnUtils.EnableEntity(entity);
                if (!string.IsNullOrWhiteSpace(SerializationName))
                    EntitySerializationUtils.RegisterAsSerializedEntity(token, SerializationName);
                return token;
            }
            if (Installer.Installer)
                return new EntitySpawnContext
                {
                    Installer = Installer.Installer,
                    Parent = Parent,
                    SerializationName = SerializationName,
                }.Spawn();

            throw new ArgumentException($"Could not create an entity with this context.");
        }
        public T Spawn<T>() where T : class
        {
            var entity = Spawn();
            return GameObjectSpawnUtils.GameObjectEntityRequire<T>(entity);
        }
    }
}