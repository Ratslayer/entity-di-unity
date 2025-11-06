using BB;
using UnityEngine;

public sealed class RootSerializerV1 : BaseSerializer<RootSerializerV1, Root, RootSerializerV1.Data>
{
    protected override void ApplyAfterSpawn(Root target, Data data)
    {
        target.Position = data.Position;
        target.Rotation = data.Rotation;
        target.NonUniformScale = data.Scale;
    }

    protected override Data Serialize(Root target)
    {
        return new()
        {
            Position = target.Position,
            Rotation = target.Rotation,
            Scale = target.NonUniformScale
        };
    }

    public sealed class Data
    {
        public V3Serialized Position { get; init; }
        public QuatSerialized Rotation { get; init; }
        public V3Serialized Scale { get; init; }
    }
    public struct V3Serialized
    {
        public float X { get; init; }
        public float Y { get; init; }
        public float Z { get; init; }
        public static implicit operator Vector3(V3Serialized v)
            => new()
            {
                x = v.X,
                y = v.Y,
                z = v.Z,
            };
        public static implicit operator V3Serialized(Vector3 v)
           => new()
           {
               X = v.x,
               Y = v.y,
               Z = v.z,
           };
    }
    public struct QuatSerialized
    {
        public float X { get; init; }
        public float Y { get; init; }
        public float Z { get; init; }
        public float W { get; init; }
        public static implicit operator Quaternion(QuatSerialized quat)
            => new()
            {
                x = quat.X,
                y = quat.Y,
                z = quat.Z,
                w = quat.W
            };
        public static implicit operator QuatSerialized(Quaternion quat)
           => new()
           {
               X = quat.x,
               Y = quat.y,
               Z = quat.z,
               W = quat.w
           };
    }
}
