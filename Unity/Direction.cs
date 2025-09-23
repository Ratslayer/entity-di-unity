using UnityEngine;

namespace BB
{
    public enum Direction
    {
        None,
        Forward,
        Back,
        Left,
        Right,
        Up,
        Down
    }
    public static class DirectionExtensionMethods
    {
        public static Direction ToHorizontalDirection(this Vector3 v)
        {
            var x = Mathf.Abs(v.x);
            var z = Mathf.Abs(v.z);
            if (z > x)
                return Mathf.Sign(v.z) > 0 ? Direction.Forward : Direction.Back;
            return Mathf.Sign(v.x) > 0 ? Direction.Right : Direction.Left;
        }
        public static Vector3 ToV3(this Direction dir)
            => dir switch
            {
                Direction.Forward => Vector3.forward,
                Direction.Back => Vector3.back,
                Direction.Left => Vector3.left,
                Direction.Right => Vector3.right,
                Direction.Up => Vector3.up,
                Direction.Down => Vector3.down,
                _ => Vector3.zero
            };
    }
}
