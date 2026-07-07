#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace BB
{
    public static class HandleUtils
    {
        private const float BoxHandleSize = 0.08f;
        private const float MinBoxSize = 0.001f;
        private const float MoveEpsilon = 0.000001f;

        public static Color Color
        {
            get => Handles.color;
            set => Handles.color = value;
        }

        public static void DrawText(string text, Vector3 pos)
        {
            Handles.Label(pos, text);
        }

        public static void DrawCube(Vector3 center, Vector3 size, Quaternion rotation)
        {
            var c = center;
            var r = rotation * (size.x * 0.5f * Vector3.right);
            var u = rotation * (size.y * 0.5f * Vector3.up);
            var f = rotation * (size.z * 0.5f * Vector3.forward);

            var blb = c - u - r - f;
            var blf = c - u - r + f;
            var brb = c - u + r - f;
            var brf = c - u + r + f;
            var ulb = c + u - r - f;
            var ulf = c + u - r + f;
            var urb = c + u + r - f;
            var urf = c + u + r + f;
            DrawShape(blb, blf, brf, brb);
            DrawShape(ulb, ulf, urf, urb);
            DrawLine(blb, ulb);
            DrawLine(blf, ulf);
            DrawLine(brf, urf);
            DrawLine(brb, urb);
        }

        public static bool DrawEditableCube(
            Vector3 center,
            Vector3 size,
            Quaternion rotation,
            bool moveBottom,
            out Vector3 newCenter,
            out Vector3 newSize)
        {
            var up = rotation * Vector3.up;
            size = NormalizeBoxSize(size);
            var bounds = new Bounds(center + up * size.y * 0.5f, size);
            if (!DrawEditableBoxBounds(bounds, rotation, out var newBounds))
            {
                newCenter = default;
                newSize = default;
                return false;
            }

            if (!moveBottom)
            {
                var currentBottom = Vector3.Dot(center, up);
                var newBottom = Vector3.Dot(newBounds.center, up) - newBounds.size.y * 0.5f;
                newBounds.center += up * (currentBottom - newBottom);
            }

            newSize = newBounds.size;
            newCenter = newBounds.center - up * newSize.y * 0.5f;
            return true;
        }

        public static bool DrawEditableBoxBounds(
            Bounds bounds,
            Quaternion rotation,
            out Bounds newBounds)
        {
            var handleMatrix = Handles.matrix;
            var boxToHandle = Matrix4x4.TRS(bounds.center, rotation, Vector3.one);
            var boxToWorld = handleMatrix * boxToHandle;
            var worldToBox = boxToWorld.inverse;
            var size = NormalizeBoxSize(bounds.size);
            var halfSize = size * 0.5f;
            var min = -halfSize;
            var max = halfSize;

            newBounds = new Bounds(bounds.center, size);
            DrawCube(bounds.center, size, rotation);

            var changed = false;
            var editedBounds = newBounds;
            using (new Handles.DrawingScope(Handles.color, Matrix4x4.identity))
            {
                DrawCornerHandles(
                    bounds.center,
                    rotation,
                    boxToWorld,
                    worldToBox,
                    min,
                    max,
                    halfSize,
                    ref changed,
                    ref editedBounds);

                DrawEdgeHandles(
                    bounds.center,
                    rotation,
                    boxToWorld,
                    worldToBox,
                    min,
                    max,
                    halfSize,
                    ref changed,
                    ref editedBounds);

                DrawFaceHandles(
                    bounds.center,
                    rotation,
                    boxToWorld,
                    worldToBox,
                    min,
                    max,
                    halfSize,
                    ref changed,
                    ref editedBounds);
            }

            newBounds = editedBounds;
            return changed;
        }

        public static bool DrawEditableQuad(
            Vector3 center,
            Vector2 size,
            Quaternion rotation,
            bool drawCornerHandles,
            bool drawEdgeHandles,
            bool allow3dMove,
            out Vector3 newCenter,
            out Vector2 newSize)
        {
            newCenter = default;
            newSize = default;
            var changed = false;
            var right = rotation * Vector3.right;
            var up = rotation * Vector3.up;
            var forward = rotation * Vector3.forward;
            var x = size.x * 0.5f * right;
            var z = size.y * 0.5f * forward;
            var c1 = center + x + z;
            var c2 = center + x - z;
            var c3 = center - x - z;
            var c4 = center - x + z;

            var c = newCenter;
            var s = newSize;
            if (drawCornerHandles)
            {
                MovePoint(c1, 1, 1, 1, 1);
                MovePoint(c2, 1, 1, 1, -1);
                MovePoint(c3, 1, 1, -1, -1);
                MovePoint(c4, 1, 1, -1, 1);
            }

            if (drawEdgeHandles)
            {
                MovePoint(c1.Mid(c2), 1, 0, 1, 0);
                MovePoint(c2.Mid(c3), 0, 1, 0, -1);
                MovePoint(c3.Mid(c4), 1, 0, -1, 0);
                MovePoint(c4.Mid(c1), 0, 1, 0, 1);
            }

            newCenter = c;
            newSize = s;

            DrawShape(c1, c2, c3, c4);
            return changed;

            void MovePoint(Vector3 point, float centerGrowX, float centerGrowY, float sizeGrowX, float sizeGrowY)
            {
                if (!MoveDot(point, out var newPoint))
                    return;
                var diff = rotation.Inverse() * (newPoint - point);
                if (!allow3dMove)
                    diff.y = 0; //-= up * Vector3.Dot(diff, up);
                c = center + rotation * diff.Mul(new(centerGrowX, 1, centerGrowY)) * 0.5f;
                s = size + new Vector2(diff.x * sizeGrowX, diff.z * sizeGrowY);
                // + new Vector2(Vector3.Dot(diff, right), Vector3.Dot(diff, forward))
                // * new Vector2(growX, growY);
                changed = true;
            }
        }

        public static void DrawSegments(params Vector3[] segments)
        {
            Handles.DrawLines(segments);
        }

        public static void DrawLine(Vector3 p1, Vector3 p2, float thickness = 0)
            => Handles.DrawLine(p1, p2, thickness);

        public static void DrawText(Vector3 position, string txt)
        {
            Handles.Label(position, txt);
        }

        public static void DrawLinesContinuous(params Vector3[] points)
        {
            Handles.DrawPolyLine(points);
        }

        public static bool MoveDot(Vector3 pos, Transform space, Action<Vector3> onMove)
        {
            if (!MoveDot(pos, out var newPos))
                return false;
            var diff = newPos - pos;
            var localDiff = space.InverseTransformDirection(diff);
            onMove(localDiff);
            return true;
        }

        public static void DrawShape(params Vector3[] vertices)
        {
            if (vertices.Length < 2)
                return;
            if (vertices.Length == 2)
            {
                DrawSegments(vertices);
                return;
            }

            var indices = new int[vertices.Length * 2];
            for (var i = 0; i < vertices.Length - 1; i++)
            {
                indices[i * 2] = i;
                indices[i * 2 + 1] = i + 1;
            }

            indices[^2] = vertices.Length - 1;
            indices[^1] = 0;
            Handles.DrawLines(vertices, indices);
        }

        public static bool DotButton(Vector3 point, float dotSize = 0.1f)
            => Handles.Button(point, Quaternion.identity, dotSize, dotSize, Handles.DotHandleCap);

        public static Vector3 MoveDot(Vector3 point, float dotSize = 0.1f)
        {
            return Handles.FreeMoveHandle(point, dotSize, new(), Handles.DotHandleCap);
        }

        public static bool MoveDot(Vector3 point, out Vector3 newPoint, float dotSize = 0.1f)
        {
            newPoint = MoveDot(point, dotSize);
            return !newPoint.Approximately(point);
        }

        public static bool MoveHandle(Vector3 point, out Vector3 newPoint)
        {
            newPoint = Handles.PositionHandle(point, Quaternion.identity);
            return !newPoint.Approximately(point);
        }

        public static void DrawCircle(
            Vector3 pos,
            float radius,
            float thickness = 1f)
            => Handles.DrawWireArc(
                pos, Vector3.up, Vector3.forward, 360, radius, thickness);

        public static void DrawCapsule(
            Vector3 pos,
            float height,
            float radius,
            float skinWidth,
            Vector3 normal)
        {
            var args = CapsuleUtils.GetVerticalCapsuleArgs(pos, height, radius, skinWidth);
            DrawCapsule(args.P1, args.P2, normal, args.Radius);
        }

        public static void DrawCapsule(
            Vector3 p1,
            Vector3 p2,
            float radius,
            float thickness = 0f)
        {
            var dir = p2 - p1;
            var tangent = Vector3.Cross(
                SceneView.lastActiveSceneView.camera.transform.forward,
                dir);
            var normal = Vector3.Cross(dir, tangent);
            DrawCapsule(p1, p2, normal, radius, thickness);
        }

        public static void DrawCapsule(
            Vector3 p1,
            Vector3 p2,
            Vector3 normal,
            float radius,
            float thickness = 0f)
        {
            var dir = p2 - p1;
            var right = Vector3.Cross(normal, dir).normalized;
            SceneGUIUtils.DrawWireArc(p1, right, normal, 180, radius, thickness);
            SceneGUIUtils.DrawWireArc(p2, right, normal, -180, radius, thickness);
            var offset = right * radius;
            Handles.DrawLine(p1 + offset, p2 + offset, thickness);
            Handles.DrawLine(p1 - offset, p2 - offset, thickness);
        }

        private static void DrawCornerHandles(
            Vector3 center,
            Quaternion rotation,
            Matrix4x4 boxToWorld,
            Matrix4x4 worldToBox,
            Vector3 min,
            Vector3 max,
            Vector3 halfSize,
            ref bool changed,
            ref Bounds editedBounds)
        {
            for (var x = -1; x <= 1; x += 2)
            for (var y = -1; y <= 1; y += 2)
            for (var z = -1; z <= 1; z += 2)
            {
                var localPosition = new Vector3(
                    halfSize.x * x,
                    halfSize.y * y,
                    halfSize.z * z);
                var worldPosition = boxToWorld.MultiplyPoint3x4(localPosition);
                var newWorldPosition = MoveBoxCornerHandle(worldPosition);
                if (changed || !HasMoved(worldPosition, newWorldPosition))
                    continue;

                editedBounds = BuildEditedBoxBounds(
                    center,
                    rotation,
                    min,
                    max,
                    worldToBox.MultiplyPoint3x4(newWorldPosition),
                    x,
                    y,
                    z);
                changed = true;
            }
        }

        private static void DrawEdgeHandles(
            Vector3 center,
            Quaternion rotation,
            Matrix4x4 boxToWorld,
            Matrix4x4 worldToBox,
            Vector3 min,
            Vector3 max,
            Vector3 halfSize,
            ref bool changed,
            ref Bounds editedBounds)
        {
            DrawEdgeHandlesForAxis(0, center, rotation, boxToWorld, worldToBox, min, max, halfSize, ref changed, ref editedBounds);
            DrawEdgeHandlesForAxis(1, center, rotation, boxToWorld, worldToBox, min, max, halfSize, ref changed, ref editedBounds);
            DrawEdgeHandlesForAxis(2, center, rotation, boxToWorld, worldToBox, min, max, halfSize, ref changed, ref editedBounds);
        }

        private static void DrawEdgeHandlesForAxis(
            int edgeAxis,
            Vector3 center,
            Quaternion rotation,
            Matrix4x4 boxToWorld,
            Matrix4x4 worldToBox,
            Vector3 min,
            Vector3 max,
            Vector3 halfSize,
            ref bool changed,
            ref Bounds editedBounds)
        {
            var signAxisA = (edgeAxis + 1) % 3;
            var signAxisB = (edgeAxis + 2) % 3;
            for (var signA = -1; signA <= 1; signA += 2)
            for (var signB = -1; signB <= 1; signB += 2)
            {
                var signs = Vector3Int.zero;
                signs[signAxisA] = signA;
                signs[signAxisB] = signB;

                var localPosition = new Vector3(
                    halfSize.x * signs.x,
                    halfSize.y * signs.y,
                    halfSize.z * signs.z);
                var worldPosition = boxToWorld.MultiplyPoint3x4(localPosition);
                var edgeDirection = boxToWorld.MultiplyVector(GetAxis(edgeAxis)).normalized;
                var newWorldPosition = MoveBoxEdgeHandle(
                    worldPosition,
                    edgeDirection);

                if (changed || !HasMoved(worldPosition, newWorldPosition))
                    continue;

                var newLocalPosition = worldToBox.MultiplyPoint3x4(newWorldPosition);
                newLocalPosition[edgeAxis] = localPosition[edgeAxis];
                editedBounds = BuildEditedBoxBounds(
                    center,
                    rotation,
                    min,
                    max,
                    newLocalPosition,
                    signs.x,
                    signs.y,
                    signs.z);
                changed = true;
            }
        }

        private static void DrawFaceHandles(
            Vector3 center,
            Quaternion rotation,
            Matrix4x4 boxToWorld,
            Matrix4x4 worldToBox,
            Vector3 min,
            Vector3 max,
            Vector3 halfSize,
            ref bool changed,
            ref Bounds editedBounds)
        {
            DrawFaceHandlesForAxis(0, center, rotation, boxToWorld, worldToBox, min, max, halfSize, ref changed, ref editedBounds);
            DrawFaceHandlesForAxis(1, center, rotation, boxToWorld, worldToBox, min, max, halfSize, ref changed, ref editedBounds);
            DrawFaceHandlesForAxis(2, center, rotation, boxToWorld, worldToBox, min, max, halfSize, ref changed, ref editedBounds);
        }

        private static void DrawFaceHandlesForAxis(
            int faceAxis,
            Vector3 center,
            Quaternion rotation,
            Matrix4x4 boxToWorld,
            Matrix4x4 worldToBox,
            Vector3 min,
            Vector3 max,
            Vector3 halfSize,
            ref bool changed,
            ref Bounds editedBounds)
        {
            for (var sign = -1; sign <= 1; sign += 2)
            {
                var signs = Vector3Int.zero;
                signs[faceAxis] = sign;
                var localPosition = new Vector3(
                    halfSize.x * signs.x,
                    halfSize.y * signs.y,
                    halfSize.z * signs.z);
                var worldPosition = boxToWorld.MultiplyPoint3x4(localPosition);
                var normal = boxToWorld.MultiplyVector(GetAxis(faceAxis) * sign).normalized;
                var newWorldPosition = MoveBoxFaceHandle(worldPosition, normal);
                if (changed || !HasMoved(worldPosition, newWorldPosition))
                    continue;

                var newLocalPosition = worldToBox.MultiplyPoint3x4(newWorldPosition);
                for (var axis = 0; axis < 3; axis++)
                {
                    if (axis != faceAxis)
                        newLocalPosition[axis] = localPosition[axis];
                }

                editedBounds = BuildEditedBoxBounds(
                    center,
                    rotation,
                    min,
                    max,
                    newLocalPosition,
                    signs.x,
                    signs.y,
                    signs.z);
                changed = true;
            }
        }

        private static Vector3 MoveBoxCornerHandle(Vector3 point)
        {
            return Handles.FreeMoveHandle(
                point,
                GetBoxHandleSize(point),
                Vector3.zero,
                Handles.DotHandleCap);
        }

        private static Vector3 MoveBoxEdgeHandle(
            Vector3 point,
            Vector3 edgeDirection)
        {
            var rawPosition = Handles.FreeMoveHandle(
                point,
                GetBoxHandleSize(point),
                Vector3.zero,
                Handles.DotHandleCap);
            var delta = Vector3.ProjectOnPlane(rawPosition - point, edgeDirection);
            return point + delta;
        }

        private static Vector3 MoveBoxFaceHandle(Vector3 point, Vector3 normal)
        {
            var rawPosition = Handles.FreeMoveHandle(
                point,
                GetBoxHandleSize(point),
                Vector3.zero,
                Handles.DotHandleCap);
            var delta = rawPosition - point;
            return point + normal * Vector3.Dot(delta, normal);
        }

        private static float GetBoxHandleSize(Vector3 point)
        {
            return HandleUtility.GetHandleSize(point) * BoxHandleSize;
        }

        private static Bounds BuildEditedBoxBounds(
            Vector3 center,
            Quaternion rotation,
            Vector3 min,
            Vector3 max,
            Vector3 movedLocalPosition,
            int xSign,
            int ySign,
            int zSign)
        {
            ApplyMovedHandleAxis(ref min.x, ref max.x, movedLocalPosition.x, xSign);
            ApplyMovedHandleAxis(ref min.y, ref max.y, movedLocalPosition.y, ySign);
            ApplyMovedHandleAxis(ref min.z, ref max.z, movedLocalPosition.z, zSign);
            NormalizeMinMax(ref min, ref max);

            var localCenter = (min + max) * 0.5f;
            var size = max - min;
            return new Bounds(center + rotation * localCenter, size);
        }

        private static void ApplyMovedHandleAxis(
            ref float min,
            ref float max,
            float movedPosition,
            int sign)
        {
            if (sign > 0)
            {
                max = movedPosition;
                return;
            }

            if (sign < 0)
                min = movedPosition;
        }

        private static void NormalizeMinMax(ref Vector3 min, ref Vector3 max)
        {
            NormalizeMinMaxAxis(ref min.x, ref max.x);
            NormalizeMinMaxAxis(ref min.y, ref max.y);
            NormalizeMinMaxAxis(ref min.z, ref max.z);
        }

        private static void NormalizeMinMaxAxis(ref float min, ref float max)
        {
            if (min > max)
                (min, max) = (max, min);

            if (max - min >= MinBoxSize)
                return;

            var center = (min + max) * 0.5f;
            min = center - MinBoxSize * 0.5f;
            max = center + MinBoxSize * 0.5f;
        }

        private static Vector3 NormalizeBoxSize(Vector3 size)
        {
            size.x = Mathf.Max(MinBoxSize, Mathf.Abs(size.x));
            size.y = Mathf.Max(MinBoxSize, Mathf.Abs(size.y));
            size.z = Mathf.Max(MinBoxSize, Mathf.Abs(size.z));
            return size;
        }

        private static Vector3 GetAxis(int axis)
        {
            return axis switch
            {
                0 => Vector3.right,
                1 => Vector3.up,
                _ => Vector3.forward
            };
        }

        private static bool HasMoved(Vector3 current, Vector3 next)
        {
            return (next - current).sqrMagnitude > MoveEpsilon;
        }
    }
}
#endif
